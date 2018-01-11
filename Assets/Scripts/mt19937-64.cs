using System;
 
namespace MersenneTwister
{
    // Implementation porting from C version of mt19937-64
    // coded by Makoto Matsumoto and Takuji Nishimura
    //
    // Copyright (C) 2004, Makoto Matsumoto and Takuji Nishimura,
    // All rights reserved.
    //
    class mt19937_64
    {
        static readonly int NN = 312;
        static readonly int MM = 156;
        static readonly ulong MATRIX_A = 0xB5026F5AA96619E9UL;
        static readonly ulong UM = 0xFFFFFFFF80000000UL;
        static readonly ulong LM = 0x7FFFFFFFUL;
 
        static ulong[] mt = new ulong[NN];
        static int mti = NN + 1;
 
        private void init_genrand64(ulong seed)
        {
            mt[0] = seed;
 
            for (int mti = 1; mti < NN; ++mti)
            {
                mt[mti] = (6364136223846793005UL * (mt[mti - 1] ^ (mt[mti - 1] >> 62)) + (ulong)mti);
            }
        }
 
        public void init_by_array64(ulong[] init_key, ulong key_length)
        {
            ulong i = 1UL, j = 0UL, k = 0UL;
            init_genrand64(19650218UL);
 
            k = (ulong)NN > key_length ? (ulong)NN : key_length;
            for (; k > 0UL; --k)
            {
                mt[i] = (mt[i] ^ ((mt[i-1] ^ (mt[i-1] >> 62)) * 3935559000370003845UL)) + init_key[j] + j;
 
                ++i; ++j;
                if (i >= (ulong)NN)
                    mt[0] = mt[NN-1]; i=1; 
 
                if (j >= key_length)
                    j=0;
            }
            for (k = (ulong)NN - 1UL; k > 0UL; --k)
            {
                mt[i] = (mt[i] ^ ((mt[i-1] ^ (mt[i-1] >> 62)) * 2862933555777941757UL)) - i; /* non linear */
 
                ++i;
                if (i >= (ulong)NN)
                {
                    mt[0] = mt[NN-1];
                    i=1; 
                }
            }
 
            mt[0] = 1UL << 63;
        }
 
        static readonly ulong[] mag01 = {0UL, MATRIX_A};
        public ulong genrand64_int64()
        {
            int i;
            ulong x = 0UL;
 
            if (mti >= NN)
            {
                if (mti == NN + 1) 
                    init_genrand64(5489UL);
 
                for (i = 0; i < NN - MM; ++i) 
                {
                    x = (mt[i] & UM) | (mt[i+1] & LM);
                    mt[i] = mt[i + MM] ^ (x >> 1) ^ mag01[(int)(x & 1UL)];
                }
                for (; i < NN - 1; ++i) 
                {
                    x = (mt[i] & UM) | (mt[i+1] & LM);
                    mt[i] = mt[i + (MM - NN)] ^ (x >> 1) ^ mag01[(int)(x & 1UL)];
                }
                x = (mt[NN - 1] & UM) | (mt[0] & LM);
                mt[NN-1] = mt[MM - 1] ^ (x >> 1) ^ mag01[(int)(x & 1UL)];
 
                mti = 0;
            }
   
            x = mt[mti++];
 
            x ^= (x >> 29) & 0x5555555555555555UL;
            x ^= (x << 17) & 0x71D67FFFEDA60000UL;
            x ^= (x << 37) & 0xFFF7EEE000000000UL;
            x ^= (x >> 43);
 
            return x;
        }
 
        public long genrand64_int63()
        {
            return (long)(genrand64_int64() >> 1);
        }
 
        public double genrand64_real1()
        {
            return (genrand64_int64() >> 11) * (1.0/9007199254740991.0);
        }
 
        public double genrand64_real2()
        {
            return (genrand64_int64() >> 11) * (1.0/9007199254740992.0);
        }
 
        public double genrand64_real3()
        {
            return ((genrand64_int64() >> 12) + 0.5) * (1.0/4503599627370496.0);
        }
    }   
}