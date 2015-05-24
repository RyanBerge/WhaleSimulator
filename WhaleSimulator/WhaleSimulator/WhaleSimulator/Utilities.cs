using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WhaleSimulator
{
    public static class Utilities
    {
        public static Vector3 Parse(string vector)
        {
            CharEnumerator sit = vector.GetEnumerator();
            //sit.MoveNext();
            int num = 1;
            string num1 = "";
            string num2 = "";
            string num3 = "";
            while (sit.MoveNext())
            {
                if (sit.Current != ' ')
                {
                    if (sit.Current != ',')
                    {
                        switch (num)
                        {
                            case 1:
                                num1 += sit.Current;
                                break;
                            case 2:
                                num2 += sit.Current;
                                break;
                            case 3:
                                num3 += sit.Current;
                                break;
                        }
                    }
                    else
                        num++;
                }
            }
            return new Vector3(float.Parse(num1), float.Parse(num2), float.Parse(num3));
        }
    }
}
