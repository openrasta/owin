using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRasta.Owin
{
    public static class BuilderExtensions
    {
        private static readonly Func<IDictionary<string, object>, Task> Last = env => Task.FromResult(0);
        public static Func<IDictionary<string, object>, Task> Build(this IEnumerable<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>> factories, IDictionary<string,object> serverProperties)
        {
            var startupProperties = serverProperties;
            var builtMidFuncs = factories.Select(factory => factory(startupProperties)).ToList().AsEnumerable();
            
            return builtMidFuncs
                .Reverse()
                .Aggregate(Last, (app, middleware) => middleware(app));
        }
    }
}