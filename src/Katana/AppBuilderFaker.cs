using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace OpenRasta.Owin.Katana
{
    public class AppBuilderFaker : IAppBuilder
    {
        Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> builder;

        public AppBuilderFaker(Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> builder)
        {
            this.builder = builder;
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            Type middlewareIsType = middleware as Type;
            if (middlewareIsType != null) RegisterMiddleware(middlewareIsType, args);
            else throw new InvalidOperationException("Compat layer only supports passing a type currently");
            return this;
        }
        
        void RegisterMiddleware(Type middlewareIsType, object[] args)
        {
            builder(properties => next =>
            {
                Properties = properties;
                if (typeof(OwinMiddleware).IsAssignableFrom(middlewareIsType))
                    return OwinMiddlewareClass(middlewareIsType, args, next);
                return NakedMdidlewareClass(middlewareIsType, args, next);
            });
        }

        Func<IDictionary<string, object>, Task> NakedMdidlewareClass(Type middlewareIsType, object[] args, Func<IDictionary<string, object>, Task> next)
        {
            var middleware = Activator.CreateInstance(middlewareIsType, PrependArgs(args, next));
            var method = middlewareIsType.GetMethod("Invoke");
            return env => (Task)method.Invoke(middleware, new object[1]{env});
        }
        static Func<IDictionary<string, object>, Task> OwinMiddlewareClass(Type middlewareIsType, object[] args, Func<IDictionary<string, object>, Task> next)
        {
            var finalArgs = PrependArgs(args, new AppFuncMiddlewareWrapper(next));
            var middleware = (OwinMiddleware)Activator.CreateInstance(middlewareIsType, finalArgs);

            return env => middleware.Invoke(new OwinContext(env));
        }

        static object[] PrependArgs(object[] args, object wrappedMiddleware)
        {
            var finalArgs = new object[args.Length + 1];
            finalArgs[0] = wrappedMiddleware;
            Array.Copy(args, 0, finalArgs, 1, args.Length);
            return finalArgs;
        }

        public object Build(Type returnType)
        {
            throw new InvalidOperationException();
        }

        public IAppBuilder New()
        {
            throw new InvalidOperationException();
        }

        public IDictionary<string, object> Properties { get; private set; }
    }
}