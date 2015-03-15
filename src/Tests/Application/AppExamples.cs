using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenRasta.Owin;

namespace Tests.Application
{
    public static class AppExamples
    {
        public static readonly Func<IDictionary<string, object>, Task> OK = env => env.Ok();

        public static MidFunc ResponseHeader(string headerName, string value)
        {
            return next => env =>
            {
                env.Response.Headers.AppendValues(headerName, value);
                return next(env);
            };
        }
    }
}