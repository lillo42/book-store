using Microsoft.AspNetCore.Http;
using Serilog;

namespace IdentityServer.Web
{
    public class LogHelper
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        { 
            // Set all the common properties available for every request
            diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
            diagnosticContext.Set("RequestId", httpContext.Connection.Id);
        }
    }
}