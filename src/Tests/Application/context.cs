using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Owin;

namespace Tests.Application
{
    public abstract class context : IDisposable
    {
        private static TestServer server;
        protected HttpResponseMessage http_response;

        protected void when_getting_root()
        {
            http_response = server.HttpClient.GetAsync("/").Result;
        }

        protected static void given_application(Func<IDictionary<string, object>, Task> application)
        {
            server = TestServer.Create(msbuilder => msbuilder.Owin()(properties=>next=>application));
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}