using FluentAssertions;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace Autofac.Extras.FilteredInjection
{
    public interface IFoo
    {
    }

    public class Test_CtorFilteredInjectionModule
    {
        #region Public Methods

        [Fact]
        public void Ctor_CheckFilterNotNull()
        {
            Action testee = () => new CtorFilteredInjectionModule(
                null,
                new Mock<Func<ParameterInfo, IComponentContext, object>>()
                    .Object
            );

            testee.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("filter");
        }

        [Fact]
        public void Ctor_CheckFactoryNotNull()
        {
            Action testee = () => new CtorFilteredInjectionModule(
                new Mock<Func<ParameterInfo, bool>>().Object,
                null
            );

            testee.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("factory");
        }

        [Fact]
        public void Injection_Works()
        {
            var p1 = new object();
            var p2 = new object();

            var builder = new ContainerBuilder();
            builder.RegisterType<Foo1>();
            builder.RegisterType<Foo2>();

            builder.RegisterModule(new CtorFilteredInjectionModule(
                p => p.Member.DeclaringType == typeof(Foo1),
                (p, c) => p1
            ));

            builder.RegisterModule(new CtorFilteredInjectionModule(
                p => p.Member.DeclaringType == typeof(Foo2),
                (p, c) => p2
            ));

            using (var container = builder.Build())
            {
                var foo1 = container.Resolve<Foo1>();
                var foo2 = container.Resolve<Foo2>();

                foo1.P.Should().Be(p1);
                foo2.P.Should().Be(p2);
            }
        }

        [Fact]
        public void Injection_ByAssembly()
        {
            var builder = new ContainerBuilder();

            var foo = new Foo();

            builder.RegisterModule(new CtorFilteredInjectionModule<IFoo>(
                pi => pi.Member.DeclaringType.Assembly == typeof(Foo).Assembly,
                (pi, c) => foo
            ));

            builder.RegisterType<Bar>();

            using (var container = builder.Build())
            {
                var bar = container.Resolve<Bar>();
                bar.Foo.Should().BeSameAs(foo);
            }
        }

        #endregion Public Methods
    }

    public class Foo1
    {
        #region Public Constructors

        public Foo1(object p)
        {
            P = p;
        }

        #endregion Public Constructors

        #region Public Properties

        public object P { get; }

        #endregion Public Properties
    }

    public class Foo2
    {
        #region Public Constructors

        public Foo2(object p)
        {
            P = p;
        }

        #endregion Public Constructors

        #region Public Properties

        public object P { get; }

        #endregion Public Properties
    }

    public class Foo : IFoo
    {
    }

    public class Bar
    {
        #region Public Constructors

        public Bar(IFoo foo)
        {
            Foo = foo;
        }

        #endregion Public Constructors

        #region Public Properties

        public IFoo Foo { get; }

        #endregion Public Properties
    }
}