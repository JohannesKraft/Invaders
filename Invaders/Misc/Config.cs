using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Invaders.Misc
{
    static class Config
    {
        public static Vector2 Resolution;
        public static float ScalingFactor;
        public static bool Fullscreen = false;

        public static readonly Vector2 StaticResolution = new Vector2(1600, 900);

        public struct Graphics
        {
            public enum AnimationType
            {
                Projectile_Explosion,
                Projectile_Trail,
                Projectile_Muzzleflash,
                Player_Movement
            }

            public struct Fonts
            {
                public static SpriteFont DefaultFont;
                public static SpriteFont MenuFont_Text;
                public static SpriteFont MenuFont_Title;
            }
        }

        private static string[] ConfigFile = new string[0];

        public static void AddConfig(string path)
        {
            string[] arr = File.ReadAllLines(path);
            string[] target = new string[ConfigFile.Length + arr.Length];

            Array.Copy(ConfigFile, 0, target, 0, ConfigFile.Length);
            Array.Copy(arr, 0, target, ConfigFile.Length, arr.Length);

            ConfigFile = target;
            // format cfg
            for (int i = 0; i < ConfigFile.Length; i++)
            {
                ConfigFile[i] = ConfigFile[i].ToLower();

                if (ConfigFile[i].Contains("//"))
                {
                    ConfigFile[i] = string.Empty;
                }
            }
        }

        public static void LoadValues()
        {
            Resolution = GetValue("ResolutionWidth", "ResolutionHeight", "");
            ScalingFactor = Resolution.X / StaticResolution.X;
        }

        public static float GetValue(string identifier, string section = "")
        {
            identifier = identifier.ToLower();
            section = section.ToLower();

            float value = float.NaN;
            string targetStr = "";
            int sectionStart = -1;
            int sectionEnd = -1;
            int line = 0;

            if (section != "")
            {
                // get sectionStart
                section = "#" + section;
                for (int i = 0; i < ConfigFile.Length; i++)
                {
                    string str = ConfigFile[i];
                    string compare = str.Replace("\n", "").Replace("\t", "").Replace(" ", "");
                    compare = compare.Substring(0, Math.Min(section.Length, compare.Length));

                    if (compare == section && sectionStart == -1)
                    {
                        sectionStart = i;
                    }
                    else if(compare == section && sectionStart != -1)
                    {
                        throw new Exception("Redundant section identifier found: Line " + (i + 1));
                    }
                }

                // get sectionEnd
                for (int i = sectionStart + 1; i < ConfigFile.Length; i++)
                {
                    string str = ConfigFile[i];
                    if (str.Contains("#") && sectionEnd == -1)
                    {
                        sectionEnd = i;
                    }
                }

                if (sectionEnd == -1 )
                {
                    sectionEnd = ConfigFile.Length - 1;
                }

                if (sectionStart == -1)
                {   
                    sectionStart = 0;
                    return value;
                }
            }
            else
            {
                sectionStart = 0;
                sectionEnd = ConfigFile.Length - 1;
            }

            // get value in given bounds
            for (int i = sectionStart; i <= sectionEnd; i++)
            {
                string str = ConfigFile[i];
                string compare = str.Replace("\n", "").Replace("\t", "").Replace(" ", "");
                compare = compare.Substring(0, Math.Min(identifier.Length, compare.Length));

                if (compare == identifier && targetStr == "")
                {
                    targetStr = str;
                    line = i + 1;
                }
                else if (compare == identifier && targetStr != "")
                {
                    throw new Exception("Redundant value identifier found: Line " + (i + 1));
                }
            }
            if (targetStr != "")
            {
                try
                {
                    targetStr = targetStr.Replace("\n", "").Replace("\t", "").Replace(" ", "").Replace(identifier, "").Replace(":", "").Replace(".", ",");
                    value = Convert.ToSingle(targetStr);
                }
                catch (Exception e)
                {
                    throw new Exception("Value couldnt be converted to float: Line " + line + "\n" + e.ToString());
                }

            }

            return value;
        }

        public static Vector2 GetValue(string identifierX, string identifierY, string section = "")
        {
            return new Vector2(GetValue(identifierX, section), GetValue(identifierY, section));
        }

        public static Vector2 ScreenLocationToPoint(Vector2 loc)
        {
            return loc / Config.ScalingFactor;
        }

        public static Vector2 PointToScreenLocation(Vector2 loc)
        {
            return loc * Config.ScalingFactor;
        }
    }
}
