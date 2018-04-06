
# Autofac.Extras.FilteredInjection

A couple of very simple generic Autofac modules that allows to set up dependency injections basing on arbitrary conditions.

## Sample usage

Set up injection of different implementation of the same service for different target components (constructor injection):

```
public interface IFoo
{
}

public class Foo1: IFoo
{
}

public class Foo2: IFoo
{
}

public class Bar1
{
    public Bar1(IFoo foo)
    {
    }
}

public class Bar2
{
    public Bar2(IFoo foo)
    {
    }
}

var builder = new ContainerBuilder();

builder.RegisterType<Foo1>().Named<IFoo>("Foo1");
builder.RegisterType<Foo2>().Named<IFoo>("Foo2");

builder.RegisterModule(new CtorFilteredInjecftionModule<IFoo>(
    p => p.Member.DeclaringType == typeof(Bar1),
    (p, c) => c.ResolveNamed<IFoo>("Foo1")
));

builder.RegisterModule(new CtorFilteredInjecftionModule<IFoo>(
    p => p.Member.DeclaringType == typeof(Bar2),
    (p, c) => c.ResolveNamed<IFoo>("Foo2")
));

builder.RegisterType<Bar1>();
builder.RegisterType<Bar2>();

using(var container = builder.Build())
{
    // Foo1 will be injected
    var bar1 = container.Resolve<Bar1>();
    
    // Foo2 will be injected
    var bar2 = container.Resolve<Bar2>();
}
```

Set up injection of different implementation of the same service for different target components (property injection):

```
public interface IFoo
{
}

public class Foo1: IFoo
{
}

public class Foo2: IFoo
{
}

public class Bar1
{
    public IFoo Foo {get; set;}
}

public class Bar2
{
    public IFoo Foo {get; set;}
}

var builder = new ContainerBuilder();

builder.RegisterType<Foo1>().Named<IFoo>("Foo1");
builder.RegisterType<Foo2>().Named<IFoo>("Foo2");

builder.RegisterModule(new PropertyFilteredInjecftionModule<IFoo>(
    p => p.DeclaringType == typeof(Bar1),
    (p, c) => c.ResolveNamed<IFoo>("Foo1")
));

builder.RegisterModule(new PropertyFilteredInjecftionModule<IFoo>(
    p => p.DeclaringType == typeof(Bar2),
    (p, c) => c.ResolveNamed<IFoo>("Foo2")
));

builder.RegisterType<Bar1>();
builder.RegisterType<Bar2>();

using(var container = builder.Build())
{
    // Foo1 will be injected
    var bar1 = container.Resolve<Bar1>();
    
    // Foo2 will be injected
    var bar2 = container.Resolve<Bar2>();
}
```

