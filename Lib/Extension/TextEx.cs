using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lib.Extension
{
    public static class TextEx
    {
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
        public static string RemoveSpace(this string text)
        {
            if (text.IsNullOrWhiteSpace()) return null;

            string result = null;

            result = Regex.Replace(text, @"[<][a-z|A-Z|/](.|)*?[>]", "");
            string[] splited = result.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            result = string.Join(" ", splited);
            if (result.Contains("\r") == true) { result = result.Replace("\r", ""); }
            if (result.Contains("\n") == true) { result = result.Replace("\n", ""); }
            if (result.Contains("\t") == true) { result = result.Replace("\t", ""); }
            if (result.Contains("\u0000") == true) { result = result.Replace("\u0000", ""); }
            if (result.Contains("\0") == true) { result = result.Replace("\0", ""); }
            result = result.Trim();
            return result;
        }
    }
}
