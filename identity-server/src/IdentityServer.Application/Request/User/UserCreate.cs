namespace IdentityServer.Application.Request.User
{
    public class UserCreate
    {
        public string Mail { get; set; }
        public string Password { get; set; }
        public bool IsEnable { get; set; }
    }
}