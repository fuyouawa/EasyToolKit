using System;
using EasyToolKit.ThirdParty.xxHash;

namespace EasyToolKit.Core
{
    public static class PseudoRandom
    {
        public static int Range(string key, int min, int max, uint seed = 0)
        {
            if (max <= min)
            {
                throw new ArgumentException($"Max '{max}' must be greater than min '{min}'");
            }
            var hash = xxHash32.ComputeHash(key, seed);
            return (int)(hash % (uint)(max - min)) + min;
        }
    }
}
