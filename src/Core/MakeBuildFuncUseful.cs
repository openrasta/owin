using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

// ReSharper disable once CheckNamespace
namespace OpenRasta.Owin
{
    public static class MakeBuildFuncUseful
    {
        public static AppDelegate AsTyped(this Func<IDictionary<string, object>, Task> appfunc)
        {
            return env => appfunc(env.Environment);
        }

        public static Func<IDictionary<string, object>, Task> AsUntyped(this AppDelegate typedAppfunc)
        {
            return env => typedAppfunc(new OwinContext(env));
        }

        public static Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>> AsUntyped(this MidDelegate midDelegate)
        {
            return next => midDelegate(next.AsTyped()).AsUntyped();
        }

        public static Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> Use(this Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> appBuilder, Func<IDictionary<string,object>, IMiddleware> middleware)
        {
            appBuilder(properties =>
            {
                MidDelegate blah = middleware(properties).Compose;

                return blah.AsUntyped();
            });
            return appBuilder;
        }
        public static Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> Use(this Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> appBuilder, Func<IDictionary<string,object>, MidDelegate> middleware)
        {
            appBuilder(properties => middleware(properties).AsUntyped());
            return appBuilder;
        }
    }
}