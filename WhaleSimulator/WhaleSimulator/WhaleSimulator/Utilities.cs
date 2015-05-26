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
            int num = 1;
            string num1 = "";
            string num2 = "";
            string num3 = "";
            for (int i = 0; i < vector.Length; i++)
            {                
                if (vector[i] != ' ')
                {
                    if (vector[i] != ',')
                    {
                        switch (num)
                        {
                            case 1:
                                num1 += vector[i];
                                break;
                            case 2:
                                num2 += vector[i];
                                break;
                            case 3:
                                num3 += vector[i];
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
