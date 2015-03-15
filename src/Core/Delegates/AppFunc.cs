using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin
{
    /// <summary>
    /// An application delegate.
    /// </summary>
    /// <param name="env">The environment properties.</param>
    /// <returns>An awaitable Task that signals the end of the processing.</returns>
    public delegate Task AppFunc(IOwinContext env);
}