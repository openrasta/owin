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

Latest builds of Xix are available on the AppVeyor project feed, at `https://ci.appveyor.com/nuget/xix-k703x7hdb0fb`

To add a source, from the command line, use `nuget sources`.

`C:\> nuget sources add -name or.owin -source https://ci.appveyor.com/nuget/owin-5jvo5rb83cxf`

You can then install from the package manager console.

```
install-package OpenRasta.Owin -pre
```

Or from any shell window, use `nuget.exe`.

```
nuget install OpenRasta.Owin -pre
```

## Making Katana fully owin-compliant

The API is currently in flux.

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

### Writing owin-compliant middleware

Using OpenRasta.Owin, it's easy to write OWIN-compliant middleware.

First, you need to write the middleware, and there's a base-class for that,
`AbstractMiddleware`.

```
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

Now you can use that middleware on any OWIN-compatible builder.

```
  public class 
