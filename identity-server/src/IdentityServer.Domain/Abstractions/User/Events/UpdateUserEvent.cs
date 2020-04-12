using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.User.Events
{
    public class UpdateUserEvent : Event
    {
        public UpdateUserEvent(string mail, bool isEnable)
        {
            Mail = mail ?? throw new ArgumentNullException(nameof(mail));
            IsEnable = isEnable;
        }

        public string Mail { get; }
        public bool IsEnable { get; }
    }
}