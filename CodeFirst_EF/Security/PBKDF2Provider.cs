﻿using System;
using System.Security.Cryptography;

namespace CodeFirst_EF.Security
{
    internal sealed class InvalidHashException : Exception
    {
        public InvalidHashException() { }
        public InvalidHashException(string message)
            : base(message) { }
        public InvalidHashException(string message, Exception inner)
            : base(message, inner) { }
    }

    internal sealed class CannotPerformOperationException : Exception
    {
        public CannotPerformOperationException() { }
        public CannotPerformOperationException(string message)
            : base(message) { }
        public CannotPerformOperationException(string message, Exception inner)
            : base(message, inner) { }
    }

    public sealed class HashResult
    {
        public string Hash;
        public string Salt;

        public HashResult(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }
    }

    /// <summary>
    /// Taken from https://github.com/defuse/password-hashing used as proof of concept of hashing not to be considered
    /// secure. Modified to allow separate storage of salt from hash to allow for use of same salt for a known word
    /// </summary>
    internal sealed class PBKDF2Provider : IHashProvider
    {
        // These constants may be changed without breaking existing hashes.
        public const int SALT_BYTES = 24;
        public const int HASH_BYTES = 18;
        public const int PBKDF2_ITERATIONS = 64000;

        // These constants define the encoding and may not be changed.
        public const int HASH_SECTIONS = 4;
        public const int HASH_ALGORITHM_INDEX = 0;
        public const int ITERATION_INDEX = 1;
        public const int HASH_SIZE_INDEX = 2;
        public const int PBKDF2_INDEX = 3;

        public HashResult CreateHash(string field, string existingSalt = null)
        {
            var password = string.Copy(field);
            string suppliedSalt = null;
            if (existingSalt != null)
            {
                suppliedSalt = string.Copy(existingSalt);
            }

            var salt = new byte[SALT_BYTES];
            if (suppliedSalt == null)
            {
                // Generate a random salt
                try
                {
                    using (var csprng = new RNGCryptoServiceProvider())
                    {
                        csprng.GetBytes(salt);
                    }
                }
                catch (CryptographicException ex)
                {
                    throw new CannotPerformOperationException(
                        "Random number generator not available.",
                        ex
                    );
                }
                catch (ArgumentNullException ex)
                {
                    throw new CannotPerformOperationException(
                        "Invalid argument given to random number generator.",
                        ex
                    );
                }
            }
            else
            {
                salt = Convert.FromBase64String(suppliedSalt);
            }

            var hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTES);

            // format: algorithm:iterations:hashSize:salt:hash
            var parts = "sha1:" +
                PBKDF2_ITERATIONS +
                ":" +
                hash.Length +
                ":" +
                Convert.ToBase64String(hash);
            
            return new HashResult(parts, Convert.ToBase64String(salt)); ;
        }

        public bool VerifyPassword(string password, string goodHash, string goodSalt)   //TODO: Move method to test class
        {
            char[] delimiter = { ':' };
            var split = goodHash.Split(delimiter);

            if (split.Length != HASH_SECTIONS)
            {
                throw new InvalidHashException(
                    "Fields are missing from the password hash."
                );
            }

            // We only support SHA1 with C#.
            if (split[HASH_ALGORITHM_INDEX] != "sha1")
            {
                throw new CannotPerformOperationException(
                    "Unsupported hash type."
                );
            }

            int iterations;
            try
            {
                iterations = int.Parse(split[ITERATION_INDEX]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Could not parse the iteration count as an integer.",
                    ex
                );
            }
            catch (OverflowException ex)
            {
                throw new InvalidHashException(
                    "The iteration count is too large to be represented.",
                    ex
                );
            }

            if (iterations < 1)
            {
                throw new InvalidHashException(
                    "Invalid number of iterations. Must be >= 1."
                );
            }

            byte[] salt;
            try
            {
                salt = Convert.FromBase64String(goodSalt);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Base64 decoding of salt failed.",
                    ex
                );
            }

            byte[] hash;
            try
            {
                hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Base64 decoding of pbkdf2 output failed.",
                    ex
                );
            }

            int storedHashSize;
            try
            {
                storedHashSize = int.Parse(split[HASH_SIZE_INDEX]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Could not parse the hash size as an integer.",
                    ex
                );
            }
            catch (OverflowException ex)
            {
                throw new InvalidHashException(
                    "The hash size is too large to be represented.",
                    ex
                );
            }

            if (storedHashSize != hash.Length)
            {
                throw new InvalidHashException(
                    "Hash length doesn't match stored hash length."
                );
            }

            var testHash = PBKDF2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}
