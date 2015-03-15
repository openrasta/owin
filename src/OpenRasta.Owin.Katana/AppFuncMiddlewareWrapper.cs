using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin.Katana
{
    public class AppFuncMiddlewareWrapper : OwinMiddleware
    {
        readonly Func<IDictionary<string, object>, Task> next;

        public AppFuncMiddlewareWrapper(Func<IDictionary<string, object>, Task> next) : base(null)
        {
            this.next = next;
        }

        public override Task Invoke(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return next(context.Environment);
        }
    }
}