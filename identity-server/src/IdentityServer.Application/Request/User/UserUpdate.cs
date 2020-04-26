using System;

namespace IdentityServer.Application.Request.User
{
    public class UserUpdate
    {
        public Guid Id { get; set; }
        public string Mail { get; set; }
        public bool IsEnable { get; set; }
    }
}