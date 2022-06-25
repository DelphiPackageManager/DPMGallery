using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPMGallery.Extensions
{
    public static class StringExtensions
    {
        public static string GetHashSha256(this string unhashed)
        {
            StringBuilder hash = new();
            using (SHA256 crypt = SHA256.Create())
            {
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(unhashed), 0, Encoding.UTF8.GetByteCount(unhashed));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }

        public static string ToSentenceCase(this string value)
        {
            // start by converting entire string to lower case
            var lowerCase = value.ToLower();
            // matches the first sentence of a string, as well as subsequent sentences
            var r = new Regex(@"(^[a-z])|\.(.)", RegexOptions.ExplicitCapture); //todo make static
                                                                                   // MatchEvaluator delegate defines replacement of setence starts to uppercase
            return r.Replace(lowerCase, s => s.Value.ToUpper());
        }
    }
}
