using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LambdaAppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using LambdaMidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>;
using LambdaMidFactory = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>>;
using LambdaBuildFunc = System.Action<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>>>;

namespace OpenRasta.Owin
{
    public class OwinApplication
    {
        private Func<LambdaAppFunc> builder;
        public OwinApplication(LambdaAppFunc app)
        {
            builder = ()=>app;
        }
        public OwinApplication(Action<LambdaBuildFunc> app)
        {
            ICollection<LambdaMidFactory> middlewareFactories = new LinkedList<LambdaMidFactory>();
            app(middlewareFactories.Add);
            builder = ()=>middlewareFactories.Build(StartupProperties);
        }

        public IDictionary<string,object> StartupProperties { get; set; }

        public static implicit operator Func<IDictionary<string, object>, Task>(OwinApplication app)
        {
            return app.builder();
        }

    }
}