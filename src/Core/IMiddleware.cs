using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin
{
    public interface IMiddleware
    {
        AppDelegate Compose(AppDelegate nextApplication);
        Task Invoke(IOwinContext env);
    }
}