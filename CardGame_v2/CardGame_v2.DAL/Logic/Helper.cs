using System.Text;
using System.Security.Cryptography;

namespace CardGame_v2.DAL.Logic
{
    public class Helper
    {
        public static string GenerateHash(string s)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);
            using (SHA512 sha = new SHA512Managed())
            {
                var hash = sha.ComputeHash(bytes);

                return GetHexNotation(hash);
            }
        }

        public static string GenerateSalt()
        {
            var salt = new byte[64];

            var rng = new RNGCryptoServiceProvider();

            rng.GetNonZeroBytes(salt);

            return GetHexNotation(salt);
        }

        private static string GetHexNotation(byte[] bytes)
        {
            var hashStringBuilder = new StringBuilder(128);

            foreach (var b in bytes)
            {
                hashStringBuilder.Append(b.ToString("X2"));
            }
            return hashStringBuilder.ToString();
        }
    }
}
