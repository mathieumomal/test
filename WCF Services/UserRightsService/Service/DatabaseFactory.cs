using System;
using Etsi.Dsdb.DataAccess;
using Microsoft.Practices.Unity;
using Etsi.UserRights.DNN3GPPDataAccess;

namespace Etsi.UserRights.Service
{
    /// <summary>
    /// Factory class to inject & resolve dependencies related to repository layer
    /// </summary>
    public class DatabaseFactory
    {
        #region Variables

        private static UnityContainer _container;

        #endregion

        #region Properties

        /// <summary>
        /// Public reference to the unity container which will 
        /// allow the ability to register instrances or take 
        /// other actions on the container.
        /// </summary>
        public static UnityContainer Container
        {
            get { return _container ?? (_container = new UnityContainer()); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor for DependencyFactory which will 
        /// initialize the unity container.
        /// </summary>
        static DatabaseFactory()
        {
            SetDefaultDependencies();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves the type parameter T to an instance of the appropriate type.
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        public static T Resolve<T>()
        {
            T ret = default(T);

            if (Container.IsRegistered(typeof(T)))
            {
                ret = Container.Resolve<T>();
            }

            return ret;
        }

        /// <summary>
        /// Set Default Dependencies
        /// </summary>
        public static void SetDefaultDependencies()
        {
            //Map of relation between Interfaces and classes
            Container.RegisterType<IDSDBContext, DSDBContext>(new TransientLifetimeManager());
            Container.RegisterType<DSDBContext>(new InjectionConstructor());
            Container.RegisterType<IDnn3gppContext, DNN3GPPContext>(new TransientLifetimeManager());
            Container.RegisterType<DNN3GPPContext>(new InjectionConstructor());
        }

        #endregion
    }
}
