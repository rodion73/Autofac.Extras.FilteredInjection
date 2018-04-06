using Autofac.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Autofac.Extras.FilteredInjection
{
    public class PropertyFilteredInjectionModule : Module
    {
        #region Private Fields

        private readonly Func<PropertyInfo, bool> _filter;
        private readonly Func<PropertyInfo, IComponentContext, object> _factory;

        #endregion Private Fields

        #region Public Constructors

        public PropertyFilteredInjectionModule(Func<PropertyInfo, bool> filter, Func<PropertyInfo, IComponentContext, object> factory)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion Public Constructors

        #region Protected Methods

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

    public class PropertyFilteredInjectionModule<T> : PropertyFilteredInjectionModule
        where T : class
    {
        #region Public Constructors

        public PropertyFilteredInjectionModule(Func<PropertyInfo, bool> filter, Func<PropertyInfo, IComponentContext, T> factory)
            : base(pi => pi.PropertyType == typeof(T) && filter(pi), factory)
        {
        }

        #endregion Public Constructors
    }
}