using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Security
{
    internal static class SaltGenerator
    {
        const int SaltLength = 4;
        public static string GenerateSalt()
        {
            RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[SaltLength / 2];
            cryptoServiceProvider.GetBytes(buffer);
            StringBuilder builder = new StringBuilder(SaltLength);
            foreach (var item in buffer)
            {
                builder.Append(Char.ConvertFromUtf32((((int)item) & 0x0F) << 4));
                builder.Append(Char.ConvertFromUtf32(((int)item) & 0xF0));
            }
            return builder.ToString();
        }
    }
}
