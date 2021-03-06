﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class ServicesFactory
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
        static ServicesFactory()
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
            Container.RegisterType<IReleaseService, ReleaseService>(new TransientLifetimeManager());
            Container.RegisterType<IWorkItemService, WorkItemService>(new TransientLifetimeManager());
            Container.RegisterType<IWorkPlanFileService, WorkPlanFileService>(new TransientLifetimeManager());
            Container.RegisterType<IMeetingService, MeetingService>(new TransientLifetimeManager());
            Container.RegisterType<IUrlService, UrlService>(new TransientLifetimeManager());
            Container.RegisterType<IPersonService, PersonService>(new TransientLifetimeManager());
            Container.RegisterType<ISpecificationService, SpecificationService>(new TransientLifetimeManager());
            Container.RegisterType<ICommunityService, CommunityService>(new TransientLifetimeManager());
            Container.RegisterType<IRightsService, RightsService>(new TransientLifetimeManager());
            Container.RegisterType<ISpecVersionService, SpecVersionService>(new TransientLifetimeManager());
            Container.RegisterType<IChangeRequestService, ChangeRequestService>(new TransientLifetimeManager());
            Container.RegisterType<ICrCategoriesService, CrCategoriesService>(new TransientLifetimeManager());
            Container.RegisterType<IItuRecommendationService, ItuRecommendationService>(new TransientLifetimeManager());
            Container.RegisterType<IRemarkService, RemarkService>(new TransientLifetimeManager());
            Container.RegisterType<IContributionService, ContributionService>(new TransientLifetimeManager());
            Container.RegisterType<IFinalizeApprovedDraftsService, FinalizeApprovedDraftsService>(new TransientLifetimeManager());
            Container.RegisterType<ICrPackService, CrPackService>(new TransientLifetimeManager());

            //Offline Interfaces
            Container.RegisterType<IOfflineService<SpecVersion>, SpecVersionService>(new TransientLifetimeManager());
            Container.RegisterType<IOfflineService<Remark>, RemarkService>(new TransientLifetimeManager());

        }
    }
}
