using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace PropollyGDS_UI_Scripts.Constants
{
    public static class Constants
    {
        public static class Fonts
        {
            private const string FONT_DIRECTORY = "Fonts";
            private const string WORK_SANS = "/WorkSans";
            
            private const string EXTRA_LIGHT = FONT_DIRECTORY +"/ExtraLight";
            private const string LIGHT = FONT_DIRECTORY +"/Light";
            private const string MEDIUM = FONT_DIRECTORY +"/Medium";
            private const string REGULAR = FONT_DIRECTORY +"/Regular";
            private const string SEMI_BOLD = FONT_DIRECTORY +"/SemiBold";
            private const string BOLD = FONT_DIRECTORY +"/Bold";

            public enum Font
            {
                WorkSans
            }

            public enum Style
            {
                ExtraLight,
                Light,
                Medium,
                Regular,
                SemiBold,
                Bold
            }

            private static readonly Dictionary<Style, string> stylePaths = new()
            {
                { Style.ExtraLight, EXTRA_LIGHT },
                { Style.Light, LIGHT },
                { Style.Medium, MEDIUM },
                { Style.Regular, REGULAR },
                { Style.SemiBold, SEMI_BOLD },
                { Style.Bold, BOLD }
            };

            private static readonly Dictionary<Font, string> fontDirectories = new()
            {
                { Font.WorkSans, WORK_SANS }
                // Add more fonts here like: { Font.OtherFont, "Fonts/OtherFontDirectory" }
            };

            public static string GetFontPath(Font font, Style style)
            {
                if (!fontDirectories.TryGetValue(font, out var fontDirectory))
                {
                    throw new ArgumentException("Font not supported.", nameof(font));
                }

                if (!stylePaths.TryGetValue(style, out var stylePath))
                {
                    throw new ArgumentException("Style not supported.", nameof(style));
                }

                return $"{FONT_DIRECTORY}/{fontDirectory}/{stylePath}";
            }
        }

        public static class Icons
        {
            
        }

        public static class UITemplates
        {
            
        }

        
    }
}