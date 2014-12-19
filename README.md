# OpenRasta OWIN support

OWIN is a set of interfaces for .net allowing for the decoupling of client and
server applications.

OpenRasta.Owin allows you to use all of the goodness of OWIN without any
dependency on IAppBuilder (or other propriotory vendor extensions), and can be
used with any OWIN-compliant web framework.

In other words, it's all the goodness of OWIN without any dependencies.

OpenRasta.Owin.Katana is an adapter for the Katana framework to make it
fully OWIN-compliant.

## Installing

Latest builds of Xix are available on the AppVeyor project feed, at `https://ci.appveyor.com/nuget/owin-h2pqkbp0lbr0`

To add a source, from the command line, use `nuget sources`.

`C:\> nuget sources add -name or.owin -source https://ci.appveyor.com/nuget/owin-h2pqkbp0lbr0`

You can then install from the package manager console.

```
install-package OpenRasta.Owin -pre
```

Or from any shell window, use `nuget.exe`.

```
nuget install OpenRasta.Owin -pre
```

## Work in progress

The API is currently in flux. The dependency on Microsoft.Owin and Owin is
temporary until the full-fledged object-model is released later on this year.

At terms, OpenRasta.Owin should have dependencies on nothing.

### Writing OWIN-compliant middleware using delegates


```csharp
...
public void Configure(BuildFunc app)
{
  app.Use(properties => next => async context => {
    Debug.WriteLine("Before");
    await nest(context);
    Debug.WriteLine("After");
  });
}
```

### Writing owin-compliant middleware using base classes

Using OpenRasta.Owin, it's easy to write OWIN-compliant middleware.

First, you need to write the middleware, and there's a base-class for that,
`AbstractMiddleware`.

```csharp
public class LoggingMiddleware : AbstractMiddleware
{
  public override async Task Invoke(IOwinContext env)
  {
    Debug.WriteLine("Before execution");
    await Next(env);
    Debug.WriteLine("After execution");
  }
}
```

To write an OWIN-compliant configuration extension method, it's also simple.

```csharp

using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Owin
{
  using BuildFunc = Action<Func<IDictionary<string, object>, Func<AppFunc,AppFunc>>>;
  public static class LoggingOwinConfiguration
  {
    public static BuildFunc UseLogger(this BuildFunc app)
    {˜
      app.Use(properties => new LoggingMiddleware());
    }
  }
}
```

## Making Katana fully owin-compliant


To make Katana's implementation of `IAppBuilder` fully owin-compliant,
install the `OpenRasta.Owin.Katana` package.

```csharp
  ...
  public void Configure(IAppBuilder appBuilder) {
    var app = appBuilder.Owin();

    // using a BuildFunc-compliant module.

    app.UseLogger();
  }
```
