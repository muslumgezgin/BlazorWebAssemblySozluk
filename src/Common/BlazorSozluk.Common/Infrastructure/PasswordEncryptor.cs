using System;
using System.Security.Cryptography;
using System.Text;

namespace BlazorSozluk.Common.Infrastructure
{
    public class PasswordEncryptor
    {
        public static string Encrypt(string password)
        {
            using var md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(password);

            byte[] hasBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hasBytes);
        }
    }
}

