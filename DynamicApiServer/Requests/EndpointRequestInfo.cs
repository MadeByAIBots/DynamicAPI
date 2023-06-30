using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace DynamicApiServer.Requests
{
    public class EndpointRequestInfo
    {
        public HttpContext Context;
        private string _body;

        public EndpointRequestInfo(HttpContext httpContext)
        {
            Context = httpContext;
        }

        public async Task<string> Body()
        {
            if (_body == null)
            {
                using var reader = new StreamReader(Context.Request.Body);
                _body = await reader.ReadToEndAsync();
            }

            return _body;
        }
    }
}