using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Dynamics365.Testautomation.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string Random(this string @this, int length)
        {
            var rand = new Random();
            const string chars = "aäbcdefghijklmnoöpqrstuüvwxyzAÄBCDEFGHIJKLMNOÖPQRSTUÜVWXYZ0123456789 ,.-/!$%&()";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rand.Next(s.Length)]).ToArray());
        }
        public static string Random(int length)
        {
            var rand = new Random();
            const string chars = "aäbcdefghijklmnoöpqrstuüvwxyzAÄBCDEFGHIJKLMNOÖPQRSTUÜVWXYZ0123456789 ,.-/!$%&()";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
