using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class TitleWithHiddenIdMateria
    {
        const string IDMATERIA_SEPARATOR_START = "((IDMATERIA=";
        static readonly string[] IDMATERIA_SEPARATOR_END = new string[] { ")) " };
        
        public static string GetIdMateria(string title)
        {
            if (title == null) return null;

            if (title.StartsWith(IDMATERIA_SEPARATOR_START))
            {
                var components = title.Split(IDMATERIA_SEPARATOR_END, 2, StringSplitOptions.None);

                if (components != null && components.Length == 2)
                    return components[0].Replace(IDMATERIA_SEPARATOR_START, "");
            }

            return null;
        }

        public static string CleanIdMateria(string title)
        {
            if (title == null) return null;

            if (title.StartsWith(IDMATERIA_SEPARATOR_START))
            {
                var components = title.Split(IDMATERIA_SEPARATOR_END, 2, StringSplitOptions.None);

                if (components != null && components.Length == 2)
                    return components[1];
            }

            return title;
        }

        public static string GetHiddenText(string text)
        {
            string id = text.Replace("<!", "").Replace(">", "").Replace("ID", "").Replace("-0","").Trim();

            return $"((IDMATERIA={id})) ";
        }
    }
}
