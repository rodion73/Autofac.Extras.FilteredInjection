using Autofac.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Autofac.Extras.FilteredInjection
{
    /// <summary>
    /// Autofac module to inject properties basing on condition.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class PropertyFilteredInjectionModule : Module
    {
        #region Private Fields

        private readonly Func<PropertyInfo, bool> _filter;
        private readonly Func<PropertyInfo, IComponentContext, object> _factory;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilteredInjectionModule"/> class.
        /// </summary>
        /// <param name="filter">The property filter. When returns true the property will be injected with specified factory.</param>
        /// <param name="factory">The factory to create injected service.</param>
        /// <exception cref="ArgumentNullException">
        /// filter or factory is null
        /// </exception>
        public PropertyFilteredInjectionModule(Func<PropertyInfo, bool> filter, Func<PropertyInfo, IComponentContext, object> factory)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Override to attach module-specific functionality to a
        /// component registration.
        /// </summary>
        /// <param name="componentRegistry">The component registry.</param>
        /// <param name="registration">The registration to attach functionality to.</param>
        /// <remarks>
        /// This method will be called for all existing <i>and future</i> component
        /// registrations - ordering is not important.
        /// </remarks>
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Activated += OnActivated;
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnActivated(object sender, ActivatedEventArgs<object> e)
        {
            foreach (var prop in e.Instance.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0 && _filter(p)))
            {
                prop.SetValue(e.Instance, _factory(prop, e.Context));
            }
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Autofac module to inject properties basing on condition.
    /// </summary>
    /// <typeparam name="T">The type of service to inject.</typeparam>
    /// <seealso cref="Autofac.Module" />
    public class PropertyFilteredInjectionModule<T> : PropertyFilteredInjectionModule
        where T : class
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilteredInjectionModule{T}"/> class.
        /// </summary>
        /// <param name="filter">The property filter. When returns true the property will be injected with specified factory.</param>
        /// <param name="factory">The factory to create injected service.</param>
        /// <exception cref="ArgumentNullException">
        /// filter or factory is null
        /// </exception>
        public PropertyFilteredInjectionModule(Func<PropertyInfo, bool> filter, Func<PropertyInfo, IComponentContext, T> factory)
            : base(pi => pi.PropertyType == typeof(T) && filter(pi), factory)
        {
        }

        #endregion Public Constructors
    }
}