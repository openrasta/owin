using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

// ReSharper disable once CheckNamespace
namespace OpenRasta.Owin.Katana
{
    public static class MakeKatanaUnderstandOwin
    {
        public static Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> Owin(this IAppBuilder appBuilder)
        {
            return middlewareBuilder => appBuilder.Use(middlewareBuilder(appBuilder.Properties));
        }

        public static Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> Use<T>(this Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> appBuilder, Func<IDictionary<string,object>,OwinMiddleware, T> middleware) where T:OwinMiddleware
        {
            appBuilder(properties => next => env => middleware(properties, new AppFuncMiddlewareWrapper(next)).Invoke(new OwinContext(env)));
            return appBuilder;
        }

        public static Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> UseLegacyAppBuilder(this Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> builder, Action<IAppBuilder> legacyBuilder)
        {
            legacyBuilder(new AppBuilderFaker(builder));
            return builder;
        }
    }
}