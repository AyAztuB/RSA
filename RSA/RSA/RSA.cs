using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace RSA
{
    public class RSA
    {
        public int rsaLength { get; set; }
        public string commonKeyParameter { get; set; }
        public string pubKey { get; set; }
        public string privKey { get; set; }

        public RSA(int rsaLength = 256, int nbRounds = 30) 
        {
            this.commonKeyParameter = "";
            this.pubKey = "";
            this.privKey = "";
            this.rsaLength = rsaLength;
            this.KeyGenerator(nbRounds);
        }


        #region PrimeNumberGenerator
        private BigInteger GenRandomNumber()
        {
            Random rnd = new Random();
            byte[] nb = new byte[rsaLength];
            rnd.NextBytes(nb);
            BigInteger number = new BigInteger(nb);
            return number;
        }

        private bool MillerRabin(BigInteger nbToTest, int round)
        {
            BigInteger s = nbToTest - 1;
            int r = 0;
            while (s % 2 == 0)
            {
                s >>= 1;
                r += 1;
            }

            Random rnd = new Random();
            for (int i = 0; i < round; i++)
            {
                int numberOfBytes;
                BigInteger a;
                do
                {
                    numberOfBytes = rnd.Next((nbToTest - 1).ToByteArray().Length);
                    byte[] bytes = new byte[numberOfBytes];
                    rnd.NextBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a >= nbToTest - 1 || a < 2);

                BigInteger x = BigInteger.ModPow(a, s, nbToTest);
                if (x == 1 || x == nbToTest - 1)
                    continue;
                bool doINeedToContinue = false;
                for (int k = 1; k < r; k++)
                {
                    x = BigInteger.ModPow(x, new BigInteger(2), nbToTest);
                    if (x == nbToTest - 1)
                        doINeedToContinue = true;
                }

                if (!doINeedToContinue)
                    return false;
            }

            return true;
        }

        private BigInteger PrimeNumberGenerator(int nbTestForMillerRabin)
        {
            BigInteger res;
            do
            {
                res = GenRandomNumber();
            } while (res < 10 || res % 2 == 0 || !MillerRabin(res, nbTestForMillerRabin));

            return res;
        }

        #endregion

        #region RsaKeysGenerator

        private static (BigInteger, BigInteger, BigInteger) Bezout(BigInteger a, BigInteger b)
        {
            var (r, u, v, rPrim, uPrim, vPrim) =
                (a, BigInteger.One, BigInteger.Zero, b, BigInteger.Zero, BigInteger.One);
            while (rPrim != 0)
            {
                BigInteger q = r / rPrim;
                (r, u, v, rPrim, uPrim, vPrim) = (rPrim, uPrim, vPrim, r - (q * rPrim),
                    u - (q * uPrim), v - (q * vPrim));
            }

            return (u, v, r);
        }
        
        // return (publicKey, privateKey)
        private ((BigInteger, BigInteger), (BigInteger, BigInteger)) KeyGenerator(BigInteger p, BigInteger q)
        {
            Random rnd = new Random();
            BigInteger n = p * q;
            BigInteger phi_n = (p - 1) * (q - 1);
            BigInteger e;
            (BigInteger, BigInteger, BigInteger) bezout;
            do
            {
                do
                {
                    int numberOfBytes = rnd.Next((phi_n - 1).ToByteArray().Length);
                    byte[] bytes = new byte[numberOfBytes];
                    rnd.NextBytes(bytes);
                    e = new BigInteger(bytes);
                } while (e <= 0 || e >= phi_n || BigInteger.GreatestCommonDivisor(e, phi_n) != 1);

                bezout = Bezout(e, phi_n);
            } while (bezout.Item1 < 0);

            this.commonKeyParameter = ToBinary(n);
            this.pubKey = ToBinary(e);
            this.privKey = ToBinary(bezout.Item1);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully generate RSA key pair !");
            Console.ForegroundColor = ConsoleColor.White;

            return ((n, e), (n, bezout.Item1));
        }

        // return (publicKey, privateKey)
        public ((BigInteger, BigInteger), (BigInteger, BigInteger)) KeyGenerator(int nbRounds)
        {
            BigInteger p = PrimeNumberGenerator(nbRounds);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully generate first prime number");
            BigInteger q = PrimeNumberGenerator(nbRounds);
            Console.WriteLine("Successfully generate first prime number");
            Console.ForegroundColor = ConsoleColor.White;
            return KeyGenerator(p, q);
        }
        
        #endregion

        #region ToAndFromBinary

        public static string ToBinary(BigInteger nb)
        {
            byte[] bytes = nb.ToByteArray();
            string res = "";
            foreach (var Byte in bytes)
            {
                res += Convert.ToChar(Byte);
            }

            return res;
        }

        public static BigInteger FromBinary(string nb)
        {
            List<Byte> res = new List<byte>();
            foreach (var chr in nb)
            {
                res.Add(Convert.ToByte(chr));
            }

            return new BigInteger(res.ToArray());
        }

        #endregion

        #region EncryptAndDecrypt

        private static BigInteger Encrypt(BigInteger message, (BigInteger, BigInteger) pubKey)
        {
            return BigInteger.ModPow(message, pubKey.Item2, pubKey.Item1);
        }

        private static BigInteger Decrypt(BigInteger message, (BigInteger, BigInteger) privKey)
        {
            return BigInteger.ModPow(message, privKey.Item2, privKey.Item1);
        }

        public static (string, bool) EncryptMessage(string message, (BigInteger, BigInteger) pubKey)
        {
            BigInteger nb = FromBinary(message);
            if (nb >= pubKey.Item1)
                return ("", false);
            return (ToBinary(Encrypt(nb, pubKey)), true);
        }

        public static string DecryptMessage(string message, (BigInteger, BigInteger) privKey)
        {
            BigInteger nb = FromBinary(message);
            return ToBinary(Decrypt(nb, privKey));
        }

        public static (string, bool) EncryptMessage(string message, string commonKey, string pubKey) => EncryptMessage(message, (FromBinary(commonKey), FromBinary(pubKey)));

        public static string DecryptMessage(string message, string commonKey, string privKey) => DecryptMessage(message, (FromBinary(commonKey), FromBinary(privKey)));

        public (string, bool) EncryptMessage(string message) => EncryptMessage(message, this.commonKeyParameter,this.pubKey);

        public string DecryptMessage(string message) => DecryptMessage(message, this.commonKeyParameter, this.privKey);

        #endregion
        
        #region BreakRSA

        private static BigInteger CompositeOf(BigInteger n, int length, int rounds)
        {
            BigInteger p = BigInteger.Pow(BigInteger.Pow(2, 8), length - 1) + 1;
            if (p % 2 == 0)
                p += 1;

            while(n % p != 0)
            {
                p += 2;
            }

            return p;
        }

        private static (BigInteger, BigInteger) GetTwoPrimeNumbers(BigInteger n, int rounds)
        {
            int lengthOfPQ = n.ToByteArray().Length / 2;
            BigInteger p = CompositeOf(n, lengthOfPQ ,rounds);
            BigInteger q = n / p;
            return (p, q);
        }

        public static (BigInteger, BigInteger) GetPrivateKey((BigInteger, BigInteger) pubKey, int nbRounds = 45)
        {
            BigInteger n = pubKey.Item1;
            var (p, q) = GetTwoPrimeNumbers(n, nbRounds);
            BigInteger phi_n = (p - 1) * (q - 1);
            BigInteger e = pubKey.Item2;
            (BigInteger, BigInteger, BigInteger) bezout;
            do
            {
                bezout = Bezout(e, phi_n);
            } while (bezout.Item1 < 0);

            return (n, bezout.Item1);
        }

        #endregion
    }
}