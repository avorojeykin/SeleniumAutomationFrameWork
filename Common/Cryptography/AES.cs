using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Common.Cryptography
{
    public class AES
    {
        private byte[] _initializationVectorBytes;
        private byte[] _keyBytes;
        private byte[] _saltBytes;
        private string _password;
        private int _passwordIteration = 2;
        private const int KEY_SIZE = 256;

        public AES(string password, string salt, string initializationVector)
        {
            _password = password;
            _saltBytes = Encoding.ASCII.GetBytes(salt);
            _initializationVectorBytes = Encoding.ASCII.GetBytes(CreateInitializationVector16Chars(initializationVector)); // Encoding.ASCII.GetBytes(createInitializationVector16Chars(Application.ProductName))
            Rfc2898DeriveBytes derivedPassword = new Rfc2898DeriveBytes(_password, _saltBytes, _passwordIteration);
            _keyBytes = derivedPassword.GetBytes(KEY_SIZE / 8);
        }

        public string EncryptData(string plainText)
        {
            RijndaelManaged symmetricKey = new RijndaelManaged();
            // symmetricKey.Padding = PaddingMode.Zeros
            symmetricKey.Mode = CipherMode.CBC;
            byte[] cipherBytes;
            byte[] plainTextBytes = Encoding.ASCII.GetBytes(plainText);
            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(_keyBytes, _initializationVectorBytes))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherBytes = memoryStream.ToArray();
                        memoryStream.Close();
                        cryptoStream.Close();
                    }
                }
            }
            symmetricKey.Clear();
            return Convert.ToBase64String(cipherBytes);
        }

        public string DecryptData(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            // symmetricKey.Padding = PaddingMode.Zeros
            symmetricKey.Mode = CipherMode.CBC;
            byte[] plainTextBytes = new byte[cipherBytes.Length - 1 + 1];
            int byteCount = 0;
            using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(_keyBytes, _initializationVectorBytes))
            {
                using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memoryStream.Close();
                        cryptoStream.Close();
                    }
                }
            }
            symmetricKey.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }

        private string CreateInitializationVector16Chars(string baseString)
        {
            string result = string.Empty;
            int letterIndex = 0;
            for (int vectorSizeCounter = 1; vectorSizeCounter <= 16; vectorSizeCounter++)
            {
                if (letterIndex > baseString.Length - 1)
                    letterIndex = 0;
                result += baseString.Substring(letterIndex, 1);
                letterIndex += 1;
            }
            return result;
        }
    }
}
