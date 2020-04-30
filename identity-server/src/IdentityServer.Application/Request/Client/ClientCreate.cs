namespace IdentityServer.Application.Request.Client
{
    public class ClientCreate
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsEnable { get; set; }
    }
}