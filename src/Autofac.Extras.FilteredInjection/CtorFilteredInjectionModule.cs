using Autofac.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Autofac.Extras.FilteredInjection
{
    public class CtorFilteredInjectionModule : Module
    {
        #region Private Fields

        private readonly Func<ParameterInfo, bool> _filter;
        private readonly Func<ParameterInfo, IComponentContext, object> _factory;

        #endregion Private Fields

        #region Public Constructors

        public CtorFilteredInjectionModule(Func<ParameterInfo, bool> filter, Func<ParameterInfo, IComponentContext, object> factory)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Preparing += OnComponentPreparing;
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Concat(new[] {
                new ResolvedParameter(
                    (pi, ctx) => _filter(pi),
                    (pi, ctx) => _factory(pi, ctx)
                )
            });
        }

        #endregion Private Methods
    }

    public class CtorFilteredInjectionModule<T> : CtorFilteredInjectionModule
        where T : class
    {
        #region Public Constructors

        public CtorFilteredInjectionModule(Func<ParameterInfo, bool> filter, Func<ParameterInfo, IComponentContext, T> factory) :
            base(pi => pi.ParameterType == typeof(T) && filter(pi), factory)
        {
        }

        #endregion Public Constructors
    }
}