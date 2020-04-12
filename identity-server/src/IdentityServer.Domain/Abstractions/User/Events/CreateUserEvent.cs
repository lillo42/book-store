using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.User.Events
{
    public class CreateUserEvent : Event
    {
        public CreateUserEvent(string mail, string password, bool isEnable)
        {
            Mail = mail ?? throw new ArgumentNullException(nameof(mail));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            IsEnable = isEnable;
        }

        public string Mail { get; }
        public string Password { get; }
        public bool IsEnable { get; }
    }
}