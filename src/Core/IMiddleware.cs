using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin
{
    public interface IMiddleware
    {
        AppFunc Compose(AppFunc nextApplication);
        Task Invoke(IOwinContext env);
    }
}