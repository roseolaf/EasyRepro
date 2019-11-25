using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;

namespace Draeger.Testautomation.CredentialsManagerCore.Extensions
{
    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string @string)
        {
            var secureString = new SecureString();

            if (@string.Length > 0)
            {
                foreach (var c in @string.ToCharArray())
                    secureString.AppendChar(c);
            }

            return secureString;
        }

        public static string ToUnsecureString(this SecureString secureString)
        {
            IntPtr unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        public static string ToLowerString(this string value)
        {
            return value.Trim()
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace(Environment.NewLine, string.Empty)
                .ToLower();
        }
        public static bool Contains(this string source, string value, StringComparison compare)
        {
            return source.IndexOf(value, compare) >= 0;
        }

        public static string ToSecretName(this string source)
        {
            var retVar = source;
            var matcher = new Regex("^[0-9a-zA-Z-]+$");
            var remover = new Regex("[^0-9^a-z^A-Z^-]");
            if (!matcher.IsMatch(source))
            {
                retVar = remover.Replace(retVar, "");
            }
            return retVar;
        }
    }
}