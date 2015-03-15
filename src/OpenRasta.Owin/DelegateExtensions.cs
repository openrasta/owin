using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using OpenRasta.Owin;
using BuildFunc = System.Action<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>>>;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

// ReSharper disable once CheckNamespace
namespace Owin
{
    public static class DelegateExtensions
    {
        public static OpenRasta.Owin.AppFunc AsAppDelegate(this AppFunc appfunc)
        {
            return env => appfunc(env.Environment);
        }

        public static AppFunc AsAppFunc(this OpenRasta.Owin.AppFunc typedAppfunc)
        {
            return env => typedAppfunc(new OwinContext(env));
        }

        public static Func<AppFunc, AppFunc> AsUntyped(this MidFunc midFunc)
        {
            return next => midFunc(next.AsAppDelegate()).AsAppFunc();
        }

        public static BuildFunc Use(this BuildFunc appBuilder, MidFunc middleware)
        {
            appBuilder(properties => middleware.AsUntyped());
            return appBuilder;
        }

        public static BuildFunc Use(this BuildFunc appBuilder, AppFunc singleApp)
        {
            appBuilder(properties => next => singleApp);
            return appBuilder;
        }
        public static BuildFunc Use(this BuildFunc appBuilder, Func<IDictionary<string,object>, IMiddleware> middleware)
        {
            appBuilder(properties =>
            {
                MidFunc blah = middleware(properties).Compose;

                return blah.AsUntyped();
            });
            return appBuilder;
        }
        public static BuildFunc Use(this BuildFunc appBuilder, Func<IDictionary<string,object>, MidFunc> middleware)
        {
            appBuilder(properties => middleware(properties).AsUntyped());
            return appBuilder;
        }
    }
}