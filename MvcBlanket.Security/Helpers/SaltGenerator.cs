using System.Security.Cryptography;

namespace MvcBlanket.Security.Helpers
{
    internal static class SaltGenerator
    {
        const int SaltLength = 8;
        public static byte[] GenerateSalt()
        {
            var cryptoServiceProvider = new RNGCryptoServiceProvider();
            var buffer = new byte[SaltLength];
            cryptoServiceProvider.GetBytes(buffer);
            return buffer;
        }
    }
}
