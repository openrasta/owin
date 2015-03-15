using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;


namespace OpenRasta.Owin.Katana
{
    using MidFunc = Func<AppFunc, AppFunc>;
    using MidFactory = Func<IDictionary<string, object>, Func<AppFunc, AppFunc>>;
    using BuildFunc = Action<Func<IDictionary<string, object>, Func<AppFunc, AppFunc>>>;

    public class AppBuilderFaker : IAppBuilder
    {
        readonly BuildFunc builder;
        readonly List<string> seen = new List<string>();

        public AppBuilderFaker(IDictionary<string, object> properties, BuildFunc builder)
        {
            this.builder = builder;
            Properties = properties;
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            var middlewareTYpe = middleware as Type;
            if (middlewareTYpe != null) RegisterMiddleware(middlewareTYpe, args);
            else throw new InvalidOperationException("Compat layer only supports passing a type.");
            return this;
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

        void RegisterMiddleware(Type middlewareType, object[] args)
        {
            var stack = WalkStackToFindCaller();
            var katanaFactory = Factory(middlewareType, args);
            var currentBuilder = Builder();
            currentBuilder = stack.Aggregate(currentBuilder, (current, frame) => Builder(frame, current));

            currentBuilder(katanaFactory);
        }

        Action<MidFactory> Builder(string name, Action<MidFactory> parent)
        {
            if (seen.Contains(name)) return parent;
            seen.Add(name);
            return midFactory => builder(properties =>
            {
                properties["middleware.Name"] = name;
                parent(midFactory);
                return _ => _;
            });
        }

        Action<MidFactory> Builder()
        {
            return midFactory => builder(midFactory);
        }

        MidFactory Factory(Type middlewareType, object[] args)
        {
            return properties =>
            {
                properties["middleware.Name"] = middlewareType.Name;
                return next => typeof (OwinMiddleware).IsAssignableFrom(middlewareType)
                    ? OwinMiddlewareClass(middlewareType, args, next)
                    : NakedMdidlewareClass(middlewareType, args, next);
            };
        }

        IEnumerable<string> WalkStackToFindCaller()
        {
            var stack = new StackTrace();
            var stackFrames = stack.GetFrames();
            if (stackFrames == null) return Enumerable.Empty<string>();
            return (from frame in stackFrames
                    let method = frame.GetMethod()
                    where method.GetCustomAttributes(typeof (ExtensionAttribute)).Any()
                    let parameters = method.GetParameters()
                    where parameters.Length > 0 && parameters.First().ParameterType == typeof (IAppBuilder)
                    let methodName = method.Name
                    where methodName != "Use"
                    select methodName).ToList();
        }

        Func<IDictionary<string, object>, Task> NakedMdidlewareClass(Type middlewareIsType, object[] args,
                                                                     Func<IDictionary<string, object>, Task> next)
        {
            var middleware = Activator.CreateInstance(middlewareIsType, PrependArgs(args, next));
            var method = middlewareIsType.GetMethod("Invoke");
            return env => (Task)method.Invoke(middleware, new object[] {env});
        }

        static Func<IDictionary<string, object>, Task> OwinMiddlewareClass(Type middlewareIsType, object[] args,
                                                                           Func<IDictionary<string, object>, Task> next)
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
    }
}