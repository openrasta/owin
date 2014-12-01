using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin
{
    public abstract class AbstractMiddleware : IMiddleware
    {
        private AppDelegate Next { get; set; }

        public virtual AppDelegate Compose(AppDelegate nextApplication)
        {
            Next = nextApplication;
            return Invoke;
        }

        public abstract Task Invoke(IOwinContext env);
    }
}