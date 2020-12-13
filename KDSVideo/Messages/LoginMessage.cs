using SynologyRestDAL.Vs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KDSVideo.Messages
{
    public class LoginMessage
    {
        public LoginMessage(string account, IReadOnlyCollection<Library> libraries)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(account));
            }

            if (libraries == null || !libraries.Any())
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(libraries));
            }

            Account = account;
            Libraries = libraries;
        }

        public string Account { get; }
        public IReadOnlyCollection<Library> Libraries { get; }
    }
}
