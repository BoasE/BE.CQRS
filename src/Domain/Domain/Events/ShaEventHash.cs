using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BE.CQRS.Domain.Events
{
    public sealed class ShaEventHash : IEventHash
    {
        private readonly SHA256 algorithm = SHA256.Create();

        private readonly string secret;

        public ShaEventHash(string secret)
        {
            this.secret = secret;
        }

        public byte[] Hash(string body)
        {
            var content = Encoding.UTF8.GetBytes(body + secret);
            return algorithm.ComputeHash(content);
        }

        public string HashString(string body)
        {
            var b = Hash(body);

            return BitConverter.ToString(b).Replace("-", "");
        }
    }
}