using FluentAssertions;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace Autofac.Extras.FilteredInjection
{
    public class Test_PropertyFilteredInjectionModule
    {
        #region Public Methods

        [Fact]
        public void Ctor_CheckFilterNotNull()
        {
            Action testee = () => new PropertyFilteredInjectionModule(
                null,
                new Mock<Func<PropertyInfo, IComponentContext, object>>()
                    .Object
            );

            testee.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("filter");
        }

        [Fact]
        public void Ctor_CheckFactoryNotNull()
        {
            Action testee = () => new PropertyFilteredInjectionModule(
                new Mock<Func<PropertyInfo, bool>>().Object,
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

            builder.RegisterModule(new PropertyFilteredInjectionModule(
                p => p.DeclaringType == typeof(Foo1),
                (p, c) => p1
            ));

            builder.RegisterModule(new PropertyFilteredInjectionModule(
                p => p.DeclaringType == typeof(Foo2),
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

        #endregion Public Methods

        #region Public Classes

        public class Foo1
        {
            #region Public Properties

            public object P { get; set; }

            #endregion Public Properties
        }

        public class Foo2
        {
            #region Public Properties

            public object P { get; set; }

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}