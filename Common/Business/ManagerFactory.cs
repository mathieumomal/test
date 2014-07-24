using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.SpecVersionBusiness;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Business
{
    public class ManagerFactory
    {
        private static UnityContainer _container;

        /// <summary>
        /// Public reference to the unity container which will 
        /// allow the ability to register instrances or take 
        /// other actions on the container.
        /// </summary>
        public static UnityContainer Container
        {
            get
            {
                if (_container == null)
                    _container = new UnityContainer();
                return _container;
            }
            private set
            {
                _container = value;
            }
        }

        /// <summary>
        /// Static constructor for DependencyFactory which will 
        /// initialize the unity container.
        /// </summary>
        static ManagerFactory()
        {
            SetDefaultDependencies();

        }

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

        public static void SetDefaultDependencies()
        {
            //Map of relation between Interfaces and classes
            Container.RegisterType<IRightsManager, RightsManager>(new TransientLifetimeManager());
            Container.RegisterType<IPersonManager, PersonManager>(new TransientLifetimeManager());
            Container.RegisterType<IWorkItemCsvParser, WorkItemCsvParser>(new TransientLifetimeManager());
            Container.RegisterType<ISpecificationManager, SpecificationManager>(new TransientLifetimeManager());
            Container.RegisterType<ICommunityManager, CommunityManager>(new TransientLifetimeManager());
            Container.RegisterType<ITranspositionManager, TranspositionManager>(new TransientLifetimeManager());
            Container.RegisterType<IReleaseManager, ReleaseManager>(new TransientLifetimeManager());
            Container.RegisterType<ISpecVersionManager, SpecVersionsManager>(new TransientLifetimeManager());
            Container.RegisterType<ISpecificationTechnologiesManager, SpecificationTechnologiesManager>(new TransientLifetimeManager());
        }
    }
}
