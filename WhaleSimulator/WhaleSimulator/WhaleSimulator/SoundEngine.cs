using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace WhaleSimulator
{
    public class SoundEngine
    {
        private ContentManager Content;
        private Dictionary<string, Sound> Sounds;

        public SoundEngine(ContentManager Content)
        {
            this.Content = Content;
        }

        public void LoadSounds(string filepath)
        {
            XDocument doc;

            try
            {
                using (Stream stream = TitleContainer.OpenStream(filepath))
                {
                    doc = XDocument.Load(stream);
                    stream.Close();
                }

                IEnumerable<XElement> elements = doc.Root.Elements("Sound");
                Sounds = new Dictionary<string, Sound>();

                foreach (XElement e in elements)
                {
                    string name;
                    int count;
                    SoundEffect[] soundEffects;

                    name = e.Element("Name").Value;
                    count = int.Parse(e.Element("Count").Value);

                    IEnumerable<XElement> instances = e.Elements("Instance");
                    int i = 0;
                    foreach (XElement x in instances)
                        i++;

                    soundEffects = new SoundEffect[i];
                    i = 0;
                    foreach (XElement x in instances)
                    {
                        soundEffects[i] = Content.Load<SoundEffect>("sounds/" + x.Value);
                        i++;
                    }

                    Sounds.Add(name, new Sound(name, soundEffects, count));
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    
        public void Play(string name, bool loop, bool overwrite)
        {
            Sound sound;
            Sounds.TryGetValue(name, out sound);
            if (sound != null)
                sound.Play(loop, overwrite);
        }

        public Sound GetSound(string name)
        {
            Sound sound;
            Sounds.TryGetValue(name, out sound);
            return sound;
        }
    }

    public class Sound
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public float Volume { get { return volume; } set { volume = value; if (currentlyPlaying != null) currentlyPlaying.Volume = value; } }
        public bool IsPlaying { get { if (currentlyPlaying == null) return false; else return currentlyPlaying.State == SoundState.Playing; } }

        private SoundEffectInstance[][] soundEffects;
        private bool[][] hasPlayed;

        private SoundEffectInstance currentlyPlaying;
        

        private float volume = 1f;

        public Sound(string name, SoundEffect[] instances, int count)
        {
            Volume = 1f;
            Name = name;
            Count = count;
            soundEffects = new SoundEffectInstance[instances.Length][];
            hasPlayed = new bool[instances.Length][];

            for (int i = 0; i < instances.Length; i++)
            {
                soundEffects[i] = new SoundEffectInstance[count];
                hasPlayed[i] = new bool[count];
                for (int j = 0; j < count; j++)
                {
                    soundEffects[i][j] = instances[i].CreateInstance();
                    hasPlayed[i][j] = false;
                }
            }
        }

        public void Play(bool loop, bool overwrite)
        {
            int index = Map.Randomizer.Next(soundEffects.Length);


            for (int i = 0; i < soundEffects[index].Length; i++)
            {
                if (soundEffects[index][i].State == SoundState.Stopped)
                {

                    currentlyPlaying = soundEffects[index][i];

                    if (currentlyPlaying.Volume == 0)
                        currentlyPlaying.Volume = Volume;

                    if (!hasPlayed[index][i])
                        currentlyPlaying.IsLooped = loop;

                    currentlyPlaying.Play();
                    hasPlayed[index][i] = true;
                    break;
                }

                if (overwrite && soundEffects[index][i].State == SoundState.Playing)
                    soundEffects[index][i].Volume = 0;
            }
        }

        public void Stop()
        {
            if (currentlyPlaying != null)
                currentlyPlaying.Stop();
        }
    }
}
