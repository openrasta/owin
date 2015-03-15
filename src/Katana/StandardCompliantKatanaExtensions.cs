using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Owin;
using OpenRasta.Owin.Katana;
using BuildFunc = System.Action<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>>>;

// ReSharper disable once CheckNamespace
namespace Owin
{
    public static class StandardCompliantKatanaExtensions
    {
        public static BuildFunc Owin(this IAppBuilder appBuilder)
        {
            return middlewareBuilder => appBuilder.Use(middlewareBuilder(appBuilder.Properties));
        }

        public static BuildFunc Use<T>(this BuildFunc appBuilder, Func<IDictionary<string,object>,OwinMiddleware, T> middleware) where T:OwinMiddleware
        {
            appBuilder(properties => next => env => middleware(properties, new AppFuncMiddlewareWrapper(next)).Invoke(new OwinContext(env)));
            return appBuilder;
        }

        public static BuildFunc UseAppBuilder(this BuildFunc builder, Action<IAppBuilder> legacyBuilder,
            [CallerMemberName]string callerMethod = "")
        {
            builder(properties =>
            {
                properties["middleware.Name"] = callerMethod.StartsWith("Use") ? callerMethod : "UseAppBuilder";
                legacyBuilder(new AppBuilderFaker(properties, builder));
                return next=>next;
            });
            return builder;
        }
        
    }
}