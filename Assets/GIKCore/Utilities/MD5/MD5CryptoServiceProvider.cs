using System.Text;
using FlowGroup.Crypto;

namespace GIKCore.Utilities
{
    public class MD5CryptoServiceProvider : MD5
    {
        public MD5CryptoServiceProvider() : base() { }

        public static byte[] GetMd5Bytes(string source)
        {
            MD5 md = new MD5CryptoServiceProvider();

            //Create a new instance of UTF8Encoding to 
            //convert the string into an array of Unicode bytes.
            UTF8Encoding enc = new UTF8Encoding();

            //Convert the string into an array of bytes.
            byte[] buffer = enc.GetBytes(source);

            //Create the hash value from the array of bytes.
            byte[] hash = md.ComputeHash(buffer);

            return hash;
        }

        public static string GetMd5String(string source, bool checkNullOrEmpty = false)
        {
            if (checkNullOrEmpty && string.IsNullOrEmpty(source))
                return "";

            byte[] hash = GetMd5Bytes(source);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
