using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InfoSearcher.Tools
{
    public static class HashHelper
    {
        private static readonly HashAlgorithm Algorithm = MD5.Create();

        public static string GetHashString(this string inputString)
        {
            var sb = new StringBuilder();
            foreach (var b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private static IEnumerable<byte> GetHash(string inputString)
        {
            return Algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }
}