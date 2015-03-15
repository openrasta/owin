using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin
{
    public abstract class AbstractMiddleware : IMiddleware
    {
        public IDictionary<string, object> Properties { get; set; }
        protected AppFunc Next { get; private set; }

        public AbstractMiddleware(IDictionary<string,object> properties)
        {
            Properties = properties;
            Properties["middleware.Name"] = GetType().Name;
        }

        public virtual AppFunc Compose(AppFunc nextApplication)
        {
            Next = nextApplication;
            return InvokeIfCondition;
        }

        Task InvokeIfCondition(IOwinContext env)
        {
            return CanInvoke(env) ? Invoke(env) : Next(env);
        }

        protected virtual bool CanInvoke(IOwinContext env)
        {
            return true;
        }

        public abstract Task Invoke(IOwinContext env);
    }
}