using System;
using System.Collections.Generic;
using System.Linq;
using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.Messages
{
    public class LoginMessage
    {
        public LoginMessage(string host, string account, IReadOnlyCollection<Library> libraries)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(host));
            }

            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(account));
            }

            if (libraries == null || !libraries.Any())
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(libraries));
            }

            Host = host;
            Account = account;
            Libraries = libraries;
        }

        public string Host { get; }
        public string Account { get; }
        public IReadOnlyCollection<Library> Libraries { get; }
    }
}
