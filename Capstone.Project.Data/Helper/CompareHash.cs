using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Capstone.Project.Data.Helper
{
    public static class CompareHash
    {
        private static readonly byte[] BitCounts =
          {
                0,1,1,2,1,2,2,3, 1,2,2,3,2,3,3,4, 1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5,
                1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5, 2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6,
                1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5, 2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6,
                2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6, 3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7,
                1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5, 2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6,
                2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6, 3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7,
                2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6, 3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7,
                3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7, 4,5,5,6,5,6,6,7, 5,6,6,7,6,7,7,8,
            };

        /// <summary>
        /// Returns a percentage-based similarity value between the two given hashes. The higher
        /// the percentage, the closer the hashes are to being identical.
        /// </summary>
        /// <param name="hash1">The first hash.</param>
        /// <param name="hash2">The second hash.</param>
        /// <returns>The similarity percentage.</returns>
        public static double Similarity(ulong hash1, ulong hash2)
        {
            return (64 - BitCount(hash1 ^ hash2)) * 100 / 64.0;
        }
        
        /// <summary>
        /// Returns a percentage-based similarity value between the two given hashes. The higher
        /// the percentage, the closer the hashes are to being identical.
        /// </summary>
        /// <param name="hash1">The first hash. Cannot be null and must have a length of 8.</param>
        /// <param name="hash2">The second hash. Cannot be null and must have a length of 8.</param>
        /// <returns>The similarity percentage.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash1"/> or <paramref name="hash2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="hash1"/> or <paramref name="hash2"/> has a length other than <c>8</c>.</exception>
        public static double SimilarityByteParam(byte[] hash1, byte[] hash2)
        {
            var h1 = BitConverter.ToUInt64(hash1, 0);
            var h2 = BitConverter.ToUInt64(hash2, 0);
            return Similarity(h1, h2);
        }
        public static double SimilarityByBinary(ulong hash1, ulong hash2)
        {

            var h1 = ConvertToBinary(hash1);
            var h2 = ConvertToBinary(hash2);
            var hammmingDistance = CalculateHammingDistance(h1, h2);
            return CalculatePercentageFor2BinaryHash(hammmingDistance);
        }
        public static double CalculatePercentageFor2BinaryHash(int hammingDistance)
        {
            return (64 - hammingDistance) * 100 / 64.0;
        }
        private static Int32 CalculateHammingDistance(string a, string b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException();
            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    count++;
            }
            return count;
        }
        private static string ConvertToBinary(ulong hash)
        {
            string result = string.Empty;
            var number = UInt64.Parse(hash.ToString());
            for (int i = 0; number > 0; i++)
            {
                result = number % 2 + result;
                number = number / 2;
            }
            return result;
        }
        /// <summary>Counts bits Utility function for similarity.</summary>
        /// <param name="num">The hash we are counting.</param>
        /// <returns>The total bit count.</returns>
        private static uint BitCount(ulong num)
        {
            uint count = 0;
            for (; num > 0; num >>= 8)
                count += BitCounts[num & 0xff];
            return count;
        }
    }
}
