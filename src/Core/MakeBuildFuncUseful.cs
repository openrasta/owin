using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using OpenRasta.Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

// ReSharper disable once CheckNamespace
namespace Owin
{
    using MidFunc = Func<AppFunc,AppFunc>;
    using BuildFunc = Action<Func<IDictionary<string, object>, Func<AppFunc,AppFunc>>>;
    public static class MakeBuildFuncUseful
    {
        public static AppDelegate AsTyped(this AppFunc appfunc)
        {
            return env => appfunc(env.Environment);
        }

        public static AppFunc AsUntyped(this AppDelegate typedAppfunc)
        {
            return env => typedAppfunc(new OwinContext(env));
        }

        public static Func<AppFunc, AppFunc> AsUntyped(this MidDelegate midDelegate)
        {
            return next => midDelegate(next.AsTyped()).AsUntyped();
        }

        public static Action<Func<IDictionary<string, object>, Func<AppFunc, AppFunc>>> Use(this BuildFunc appBuilder, Func<IDictionary<string,object>, IMiddleware> middleware)
        {
            appBuilder(properties =>
            {
                MidDelegate blah = middleware(properties).Compose;

                return blah.AsUntyped();
            });
            return appBuilder;
        }
        public static Action<Func<IDictionary<string, object>, Func<AppFunc, AppFunc>>> Use(this BuildFunc appBuilder, Func<IDictionary<string,object>, MidDelegate> middleware)
        {
            appBuilder(properties => middleware(properties).AsUntyped());
            return appBuilder;
        }
    }
}