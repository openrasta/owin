using System.Net;
using NUnit.Framework;
using OpenRasta.Owin;
using Owin;

namespace Tests.Application
{
    public class one_app : context
    {
        public one_app()
        {
            given_application(new OwinApplication(AppExamples.OK));
            when_getting_root();
        }

        [Test]
        public void app_responds()
        {
            Assert.That(http_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(http_response.ReasonPhrase, Is.EqualTo("OwinOK"));
        }
    }
    public class two_middleware : context
    {
        public two_middleware()
        {
            given_application(new OwinApplication(app => app.Use(_=>AppExamples.ResponseHeader("Via", "first"))
                                                            .Use(_=>AppExamples.ResponseHeader("Via", "second"))));
            when_getting_root();
        }

        [Test]
        public void middlewares_called_in_registration_order()
        {
            Assert.That(http_response.Headers.GetValues("Via"), Is.EquivalentTo(new[]{"first", "second"}));
        }
    }
}