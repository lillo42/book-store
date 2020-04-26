namespace IdentityServer.Application.Request.Resource
{
    public class ResourceCreate
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsEnable { get; set; }
    }
}