using System.Threading.Tasks;
using Microsoft.Owin;

namespace OpenRasta.Owin
{
    public delegate Task AppDelegate(IOwinContext env);
}