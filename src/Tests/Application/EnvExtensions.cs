using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Application
{
    public static class EnvExtensions
    {
        public static Task Ok(this IDictionary<string, object> env)
        {
            env["owin.ResponseStatusCode"] = 200;
            env["owin.ResponseReasonPhrase"] = "OwinOK";
            return Task.FromResult(0);
        }
    }
}