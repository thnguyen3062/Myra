using System.Text.RegularExpressions;

namespace UIEngine.Extensions
{
    public class UIText
    {
        public static string StringColor(string source, string colorHTMLformat)
        {
            if (string.IsNullOrEmpty(colorHTMLformat)) return source;
            string result = "<color=\"" + colorHTMLformat + "\">" + source + "</color>";
            return result;
        }

        public static string StringSize(string source, int fontSize)
        {
            string result = "<size=" + fontSize + ">" + source + "</size>";
            return result;
        }

        public static string StringBoldface(string source)
        {
            string result = "<b>" + source + "</b>";
            return result;
        }

        public static string StringItalics(string source)
        {
            string result = "<i>" + source + "</i>";
            return result;
        }

        public static string RichText(string source, string colorHTMLformat = "", int fontSize = -1, bool boldface = false, bool italics = false)
        {
            string ret = source;
            if (!string.IsNullOrEmpty(colorHTMLformat)) ret = StringColor(ret, colorHTMLformat);
            if (fontSize >= 0) ret = StringSize(ret, fontSize);
            if (boldface) ret = StringBoldface(ret);
            if (italics) ret = StringItalics(ret);

            return ret;
        }

        /// <summary>
        /// Remove '\n', '\r' character in start and end of string then replace multi space character to only one space character
        /// </summary>
        public static string CleanString(string dirtyString)
        {
            string trim1 = dirtyString.TrimStart('\r', '\n').TrimEnd('\r', '\n');
            string trim2 = Regex.Replace(trim1, @"\s+", " ");

            return trim2;
        }

        /// <summary>
        /// Runs through the specified string and remove all html tags that Unity support
        /// </summary>
        public static string StripHTMLTags(string p)
        {
            string s1 = StripBoldTag(p);
            string s2 = StripItalicsTag(s1);
            string s3 = StripColorTag(s2);
            string s4 = StripSizeTag(s3);

            return s4;
        }

        /// <summary>
        /// Runs through the specified string and remove all boldface-encoding symbols
        /// </summary>
        public static string StripBoldTag(string p)
        {
            return StripTag(p, "b>", "/b>");
        }

        /// <summary>
        /// Runs through the specified string and remove all italics-encoding symbols
        /// </summary>
        public static string StripItalicsTag(string p)
        {
            return StripTag(p, "i>", "/i>");
        }

        /// <summary>
        /// Runs through the specified string and remove all color-encoding symbols
        /// </summary>
        public static string StripColorTag(string p)
        {
            return StripTag(p, "color=", "/color>");
        }

        /// <summary>
        /// Runs through the specified string and remove all size-encoding symbols
        /// </summary>
        public static string StripSizeTag(string p)
        {
            return StripTag(p, "size=", "/size>");
        }

        private static string StripTag(string p, string start, string end)
        {
            char[] array = new char[p.Length];
            int idx = 0;
            bool inside = false;

            for (int i = 0; i < p.Length; i++)
            {
                char c = p[i];
                if (c == '<')
                {
                    if (i + start.Length < p.Length)
                    {
                        string sub = p.Substring(i + 1, start.Length);
                        if (sub.Equals(start))
                        {
                            inside = true;
                            continue;
                        }
                    }

                    if (i + end.Length < p.Length)
                    {
                        string sub = p.Substring(i + 1, end.Length);
                        if (sub.Equals(end))
                        {
                            inside = true;
                            continue;
                        }
                    }
                }

                if (c == '>')
                {
                    if (inside)
                    {
                        inside = false;
                        continue;
                    }
                }

                if (!inside)
                {
                    array[idx] = c;
                    idx++;
                }
            }

            return new string(array, 0, idx);
        }
    }
}
