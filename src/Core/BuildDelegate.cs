using System.Collections.Generic;

namespace OpenRasta.Owin
{
    delegate void BuildDelegate(IDictionary<string, object> properties, MidDelegate middleware);
}