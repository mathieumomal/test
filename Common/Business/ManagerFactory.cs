﻿using System.IO;
using Etsi.Ultimate.Business.ExternalContributionService;
using Etsi.Ultimate.Business.ItuRecommendation;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Specifications;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.Business.UserRightsService;
using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.Business.Versions.QualityChecks;
using Microsoft.Office.Interop.Word;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Business
{
    public class ManagerFactory
    {
        private static UnityContainer _container;

        /// <summary>
        ///     Static constructor for DependencyFactory which will
        ///     initialize the unity container.
        /// </summary>
        static ManagerFactory()
        {
            SetDefaultDependencies();
        }

        /// <summary>
        ///     Public reference to the unity container which will
        ///     allow the ability to register instrances or take
        ///     other actions on the container.
        /// </summary>
        public static UnityContainer Container
        {
            get
            {
                if (_container == null)
                    _container = new UnityContainer();
                return _container;
            }
            private set { _container = value; }
        }

        #region resolvers
        /// <summary>
        ///     Resolves the type parameter T to an instance of the appropriate type.
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        public static T Resolve<T>()
        {
            T ret = default(T);

            if (Container.IsRegistered(typeof (T)))
            {
                ret = Container.Resolve<T>();
            }

            return ret;
        }

        /// <summary>
        /// Resolves the type parameter T to an instance of the appropriate type and give U parameter to the implementation of class constructor
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="myVar"></param>
        /// <returns></returns>
        public static T ResolveWithString<T>(string myVar)
        {
            T ret = default(T);

            if (Container.IsRegistered(typeof(T)))
            {
                ret = Container.Resolve<T>(new ParameterOverride("myVar", myVar));
            }

            return ret;
        }

        public static T ResolveWithIDocDocumentManager<T>(IDocDocumentManager myVar)
        {
            T ret = default(T);

            if (Container.IsRegistered(typeof(T)))
            {
                ret = Container.Resolve<T>(new ParameterOverride("myVar", myVar));
            }

            return ret;
        }
        #endregion

        public static void SetDefaultDependencies()
        {
            //Map of relation between Interfaces and classes
            Container.RegisterType<IUserRightsService, UserRightsServiceClient>(new TransientLifetimeManager(),
                new InjectionConstructor("UserRightsHttpEndpoint"));
            Container.RegisterType<IExtContributionService, ExtContributionServiceClient>(new TransientLifetimeManager(), new InjectionConstructor("ExternalContributionServiceHttpEndpoint"));
            Container.RegisterType<IRightsManager, RightsManager>(new TransientLifetimeManager());
            Container.RegisterType<IPersonManager, PersonManager>(new TransientLifetimeManager());
            Container.RegisterType<IWorkItemCsvParser, WorkItemCsvParser>(new TransientLifetimeManager());
            Container.RegisterType<ISpecificationManager, SpecificationManager>(new TransientLifetimeManager());
            Container.RegisterType<ICommunityManager, CommunityManager>(new TransientLifetimeManager());
            Container.RegisterType<ITranspositionManager, TranspositionManager>(new TransientLifetimeManager());
            Container.RegisterType<IReleaseManager, ReleaseManager>(new TransientLifetimeManager());
            Container.RegisterType<ISpecVersionManager, SpecVersionsManager>(new TransientLifetimeManager());
            Container.RegisterType<ISpecificationTechnologiesManager, SpecificationTechnologiesManager>(
                new TransientLifetimeManager());
            Container.RegisterType<IChangeRequestManager, ChangeRequestManager>(new TransientLifetimeManager());
            Container.RegisterType<ICrCategoriesManager, CrCategoriesManager>(new TransientLifetimeManager());
            Container.RegisterType<IChangeRequestStatusManager, ChangeRequestStatusManager>(
                new TransientLifetimeManager());
            Container.RegisterType<IContributionManager, ContributionManager>(new TransientLifetimeManager());
            Container.RegisterType<IFtpFoldersManager, FtpFoldersManager>(new TransientLifetimeManager());
            Container.RegisterType<IFinalizeApprovedDraftsManager, FinalizeApprovedDraftsManager>(new TransientLifetimeManager());
            Container.RegisterType<ISpecReleaseManager, SpecReleaseManager>(new TransientLifetimeManager());
            Container.RegisterType<IWorkItemImporter, WorkItemImporter>(new TransientLifetimeManager());
            Container.RegisterType<IWorkPlanExporter, WorkPlanExporter>(new TransientLifetimeManager());
            Container.RegisterType<IVersionFilenameManager, VersionFilenameManager>(new TransientLifetimeManager());
            Container.RegisterType<IQualityChecks, DocXQualityChecks>(new TransientLifetimeManager());
            Container.RegisterType<IDocDocumentManager, DocDocumentManager>(new TransientLifetimeManager());
            Container.RegisterType<ICrPackManager, CrPackManager>(new TransientLifetimeManager());
            Container.RegisterType<ISpecVersionUploadManager, SpecVersionUploadManager>(new TransientLifetimeManager());

            // For ITU recommendations
            Container.RegisterType<ISeedFileParser, SeedFileParser>(new TransientLifetimeManager());
            Container.RegisterType<IItuRecommendationExporter, ItuRecommendationExporter>(new TransientLifetimeManager());
            Container.RegisterType<ISpecToItuRecordConverter, SpecToItuRecordConverter>(new TransientLifetimeManager());
            Container.RegisterType<IItuPreliminaryDataExtractor, ItuPreliminaryDataExtractor>(new TransientLifetimeManager());
            Container.RegisterType<IItuPreliminaryExporter, ItuPreliminaryExporter>(new TransientLifetimeManager());
            Container.RegisterType<ISpecVersionNumberValidator, SpecVersionNumberValidator>(new TransientLifetimeManager());

            //Actions
            Container.RegisterType<IGetNextReleaseAction, GetNextReleaseAction>(new TransientLifetimeManager());
            Container.RegisterType<ISpecVersionAllocateAction, SpecVersionAllocateAction>(new TransientLifetimeManager());
            
        }
    }
}