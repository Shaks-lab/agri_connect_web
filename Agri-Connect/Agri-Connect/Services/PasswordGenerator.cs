using System.Security.Cryptography;

namespace Agri_Connect.Services
{
    /// <summary>Generates one-time passwords for accounts an Employee creates for a farmer.</summary>
    public static class PasswordGenerator
    {
        private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string Lower = "abcdefghijkmnopqrstuvwxyz";
        private const string Digits = "23456789";

        public static string Generate(int length = 14)
        {
            var all = Upper + Lower + Digits;
            var chars = new List<char> { Pick(Upper), Pick(Lower), Pick(Digits) };
            while (chars.Count < length)
            {
                chars.Add(Pick(all));
            }

            // Fisher-Yates shuffle so the guaranteed characters aren't always first.
            for (var i = chars.Count - 1; i > 0; i--)
            {
                var j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars.ToArray());
        }

        private static char Pick(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];
    }
}
