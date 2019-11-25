using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Dynamics365.Testautomation.Common.Helper
{
    class Generator
    {
        public static string GenerateRandomName(int length = 7)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z", "sch" };
            string[] vowels = { "a", "e", "i", "o", "u" };
            string[] arrayName = new string[length];
            bool Cons;
            int a = r.Next(0, 1);
            if (a == 1)
                Cons = true;
            else
                Cons = false;
            int i = 0;
            while (i < length)
            {
                if (Cons)
                {
                    arrayName[i] = consonants[r.Next(consonants.Length)];
                    Cons = false;
                    i++;
                }
                else
                {
                    arrayName[i] = vowels[r.Next(vowels.Length)];
                    Cons = true;
                    i++;
                }
            }
            arrayName[0] = arrayName[0].ToUpper();
            return string.Join("", arrayName);
        }
    }
}
