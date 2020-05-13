using Serilog.Formatting.Elasticsearch;

namespace IdentityServer.Web.Formatter
{
    public class KibanFormatter : ExceptionAsObjectJsonFormatter
    {
        public KibanFormatter()
            : base(renderMessage: true)
        {
            
        }
    }
}