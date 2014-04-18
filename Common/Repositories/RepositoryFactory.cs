using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.DataAccess;

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
        static RepositoryFactory()
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
            Container.RegisterType<IReleaseRepository, ReleaseRepository>(new TransientLifetimeManager());
            Container.RegisterType<IEnum_ReleaseStatusRepository, Enum_ReleaseStatusRepository>(new TransientLifetimeManager());
            Container.RegisterType<IWorkItemRepository, WorkItemRepository>(new TransientLifetimeManager());
            Container.RegisterType<IUltimateUnitOfWork, UltimateUnitOfWork>(new TransientLifetimeManager());
            Container.RegisterType<IHistoryRepository, HistoryRepository>(new TransientLifetimeManager());
            Container.RegisterType<IUltimateContext, UltimateContext>(new TransientLifetimeManager());
            Container.RegisterType<IMeetingRepository, MeetingRepository>(new TransientLifetimeManager());   

            Container.RegisterType<IUserRightsRepository, UserRightsRepository>(new TransientLifetimeManager());
            Container.RegisterType<IUserRolesRepository, UserRolesRepository>(new TransientLifetimeManager());
            Container.RegisterType<IPersonRepository, PersonRepository>(new TransientLifetimeManager());
            Container.RegisterType<IMeetingRepository, MeetingRepository>(new TransientLifetimeManager());
            Container.RegisterType<ICommunityRepository, CommunityRepository>(new TransientLifetimeManager());
            Container.RegisterType<IUrlRepository, UrlRepository>(new TransientLifetimeManager());
            Container.RegisterType<IWorkPlanFileRepository, WorkPlanFileRepository>(new TransientLifetimeManager());
            Container.RegisterType<ISpecificationRepository, SpecificationRepository>(new TransientLifetimeManager());
        }
    }
}
