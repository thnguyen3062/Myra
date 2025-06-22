using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace GIKCore.Utilities
{
    public class ICrypto
    {
        public static string EncryptNewPwd(string password, string seedCode)
        {            
            int trans = int.Parse(seedCode[0] + "");

            string tmp = password;
            tmp = TransBase64String(tmp, trans);

            string ePwd = Base64Encode(tmp);
            ePwd = TransBase64String(ePwd, trans);
            return ePwd;
        }
        public static string TransBase64String(string input, int trans)
        {
            string ret = input;
            if (input.Length != 4)
            {
                int length = input.Length - 2;
                char[] charTrans = new char[length];

                for (int i = 0; i < length; i++)
                {
                    char ch = ret[i];
                    int newPos = (i + trans) % length;
                    if (newPos < 0)
                        newPos = newPos + length;
                    charTrans[newPos] = ch;
                }

                ret = new string(charTrans) + input.Substring(length);
            }

            return ret;
        }
        public static string Base64Encode(string plainText)
        {
            try
            {
                byte[] encodeBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(encodeBytes);
            }
            catch (System.Exception e) { return ""; }
        }
        public static string Base64Decode(string encodedText)
        {
            try
            {
                byte[] decodeBytes = System.Convert.FromBase64String(encodedText);
                return System.Text.Encoding.UTF8.GetString(decodeBytes);
            }
            catch (System.Exception e) { return ""; }
        }

        public static string TripleDESEncrypt(string plainText, string passPhrase)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            try
            {
                byte[] keydata = System.Text.Encoding.UTF8.GetBytes(passPhrase);
                byte[] keyhash = new MD5CryptoServiceProvider().ComputeHash(keydata);
                string md5String = System.BitConverter.ToString(keyhash).Replace("-", "").ToLower();
                byte[] tripleDesKey = System.Text.Encoding.UTF8.GetBytes(md5String.Substring(0, 24));

                TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
                tripdes.Key = tripleDesKey;
                tripdes.Mode = CipherMode.ECB;

                ICryptoTransform encrypt = tripdes.CreateEncryptor();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(plainText);
                byte[] block = encrypt.TransformFinalBlock(buffer, 0, buffer.Length);

                return System.Convert.ToBase64String(block);
            }
            catch (System.Exception e)
            {
                return plainText;
            }
        }

        public static string TripleDESDecrypt(string cipherText, string passPhrase, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(cipherText)) return defaultValue;
            try
            {
                byte[] keydata = System.Text.Encoding.UTF8.GetBytes(passPhrase);
                byte[] keyhash = new MD5CryptoServiceProvider().ComputeHash(keydata);
                string md5String = System.BitConverter.ToString(keyhash).Replace("-", "").ToLower();
                byte[] tripleDesKey = System.Text.Encoding.UTF8.GetBytes(md5String.Substring(0, 24));

                TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
                tripdes.Key = tripleDesKey;
                tripdes.Mode = CipherMode.ECB;

                ICryptoTransform decrypt = tripdes.CreateDecryptor();
                byte[] buffer = System.Convert.FromBase64String(cipherText);
                byte[] block = decrypt.TransformFinalBlock(buffer, 0, buffer.Length);

                return System.Text.Encoding.UTF8.GetString(block);
            }
            catch (System.Exception e)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
        /// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rijndaelmanaged?view=netframework-4.8
        /// </summary>    

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;
        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;
        public static string RijndaelEncrypt(string plainText, string passPhrase)
        {
            try
            {
                // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                byte[] saltStringBytes = Generate256BitsOfRandomEntropy();
                byte[] ivStringBytes = Generate256BitsOfRandomEntropy();
                byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

                using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    byte[] keyBytes = password.GetBytes(Keysize / 8);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                    byte[] cipherTextBytes = saltStringBytes;
                                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return System.Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                return plainText;
            }
        }
        public static string RijndaelDecrypt(string cipherText, string passPhrase, string defaultValue = "")
        {
            try
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                byte[] cipherTextBytesWithSaltAndIv = System.Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    byte[] keyBytes = password.GetBytes(Keysize / 8);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// https://www.codeproject.com/Tips/1156169/Encrypt-Strings-with-Passwords-AES-SHA
        /// https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.aes?view=netframework-4.8
        /// </summary>
        public static string AESEncrypt(string plainText, string passPhrase)
        {
            try
            {
                byte[][] hashKeys = GenerateHashKey(passPhrase);
                using (Aes aesAlg = Aes.Create())
                {// Create an Aes object with the specified key and IV.
                    aesAlg.Key = hashKeys[0];
                    aesAlg.IV = hashKeys[1];

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            byte[] encrypted = msEncrypt.ToArray();
                            return System.Convert.ToBase64String(encrypted);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                return plainText;
            }
        }
        public static string AESDecrypt(string cipherText, string passPhrase, string defaultValue = "")
        {
            try
            {
                byte[][] hashKeys = GenerateHashKey(passPhrase);
                byte[] cipherTextBytes = System.Convert.FromBase64String(cipherText);
                using (Aes aesAlg = Aes.Create())
                {// Create an Aes object with the specified key and IV.
                    aesAlg.Key = hashKeys[0];
                    aesAlg.IV = hashKeys[1];

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.                            
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {// Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        private static byte[][] GenerateHashKey(string key)
        {
            byte[][] ret = new byte[2][];
            using (SHA256 sha256 = new SHA256CryptoServiceProvider())
            {
                byte[] rawKey = System.Text.Encoding.UTF8.GetBytes(key);
                byte[] rawIV = System.Text.Encoding.UTF8.GetBytes(key);
                byte[] hashKey = sha256.ComputeHash(rawKey);
                byte[] hashIV = sha256.ComputeHash(rawIV);

                System.Array.Resize(ref hashIV, 16);

                ret[0] = hashKey;
                ret[1] = hashIV;

                return ret;
            }
        }
    }

}