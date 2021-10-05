using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DPMGallery.Extensions
{
    public static class StringExtensions
    {
        public static string GetHashSha256(this string unhashed)
        {
            StringBuilder hash = new();
            using (SHA256Managed crypt = new())
            {
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(unhashed), 0, Encoding.UTF8.GetByteCount(unhashed));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }

    }
}
