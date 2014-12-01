using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin
{
    public abstract class AbstractMiddleware : IMiddleware
    {
        public AppDelegate Next { get; private set; }

        public virtual AppDelegate Compose(AppDelegate nextApplication)
        {
            this.Next = nextApplication;
            return Invoke;
        }

        public abstract Task Invoke(IOwinContext env);
    }
}