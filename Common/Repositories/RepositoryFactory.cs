using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Repositories
{
    public class RepositoryFactory
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
        static RepositoryFactory()
        {
            var container = new UnityContainer();

            //Map of relation between Interfaces and classes
            container.RegisterType<IReleaseRepository, ReleaseRepository>(new TransientLifetimeManager());
            container.RegisterType<IWorkItemRepository, WorkItemRepository>(new TransientLifetimeManager());
            container.RegisterType<IUltimateUnitOfWork, UltimateUnitOfWork>(new TransientLifetimeManager());
            
            container.RegisterType<IUserRightsRepository, UserRightsRepository>(new TransientLifetimeManager());
            container.RegisterType<IUserRolesRepository, UserRolesRepository>(new TransientLifetimeManager());
            container.RegisterType<IPersonRepository, PersonRepository>(new TransientLifetimeManager());
            container.RegisterType<IMeetingRepository, MeetingRepository>(new TransientLifetimeManager());
            container.RegisterType<ICommunityRepository, CommunityRepository>(new TransientLifetimeManager());

            _container = container;
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
    }
}
