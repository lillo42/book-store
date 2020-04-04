using Serilog.Formatting.Elasticsearch;

namespace Users.Web.Formatter
{
    public class KibanFormatter : ExceptionAsObjectJsonFormatter
    {
        public KibanFormatter()
            : base(renderMessage: true)
        {
            
        }
    }
}