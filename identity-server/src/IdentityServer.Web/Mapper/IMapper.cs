namespace IdentityServer.Web.Mapper
{
    public interface IMapper<in TSource, out TDestiny>
    {
        TDestiny Map(TSource source);
    }
}