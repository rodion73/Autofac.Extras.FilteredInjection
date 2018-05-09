using Autofac.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Autofac.Extras.FilteredInjection
{
    /// <summary>
    /// Autofac module to inject constructor parameters basing on condition.
    /// </summary>
    /// <seealso cref="Autofac.Module"/>
    public class CtorFilteredInjectionModule : Module
    {
        #region Private Fields

        private readonly Func<ParameterInfo, bool> _filter;
        private readonly Func<ParameterInfo, IComponentContext, object> _factory;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CtorFilteredInjectionModule"/> class.
        /// </summary>
        /// <param name="filter">
        /// The constructor parameter filter. When returns true the constructor
        /// parameter will be injected with specified factory.
        /// </param>
        /// <param name="factory">The factory to create injected service.</param>
        /// <exception cref="ArgumentNullException">
        /// filter or factory is null
        /// </exception>
        public CtorFilteredInjectionModule(Func<ParameterInfo, bool> filter, Func<ParameterInfo, IComponentContext, object> factory)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Override to attach module-specific functionality to a component registration.
        /// </summary>
        /// <param name="componentRegistry">The component registry.</param>
        /// <param name="registration">
        /// The registration to attach functionality to.
        /// </param>
        /// <remarks>
        /// This method will be called for all existing <i>and future</i>
        /// component registrations - ordering is not important.
        /// </remarks>
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

    /// <summary>
    /// Autofac module to inject constructor parameters basing on condition.
    /// </summary>
    /// <typeparam name="T">The type of service to inject.</typeparam>
    /// <seealso cref="Autofac.Module"/>
    public class CtorFilteredInjectionModule<T> : CtorFilteredInjectionModule
        where T : class
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CtorFilteredInjectionModule{T}"/> class.
        /// </summary>
        /// <param name="filter">
        /// The constructor parameter filter. When returns true the constructor
        /// parameter will be injected with specified factory.
        /// </param>
        /// <param name="factory">The factory to create injected service.</param>
        /// <exception cref="ArgumentNullException">
        /// filter or factory is null
        /// </exception>
        public CtorFilteredInjectionModule(Func<ParameterInfo, bool> filter, Func<ParameterInfo, IComponentContext, T> factory) :
            base(pi => pi.ParameterType == typeof(T) && filter(pi), factory)
        {
        }

        #endregion Public Constructors
    }
}