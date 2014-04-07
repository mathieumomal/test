using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;

namespace Etsi.Ultimate.Tests.Business
{
    class WorkPlanExporterTest : BaseTest
    {
        #region Tests

        [Test, TestCaseSource("WorkItemData")]
        public void ExportWorkPlan(WorkItemFakeDBSet workItemData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiExporter = new WorkPlanExporter(uow);

            Assert.IsTrue( wiExporter.ExportWorkPlan(".") );
        }

        #endregion

        #region Test Data

        /// <summary>
        /// Get the WorkItem Data from csv
        /// </summary>
        public IEnumerable<WorkItemFakeDBSet> WorkItemData
        {
            get
            {
                var workItemList = GetAllTestRecords<WorkItem>(Directory.GetCurrentDirectory() + "\\TestData\\WorkItems\\WorkItem.csv");
                WorkItemFakeDBSet workItemFakeDBSet = new WorkItemFakeDBSet();
                workItemList.ForEach(x => workItemFakeDBSet.Add(x));

                yield return workItemFakeDBSet;
            }
        }

        #endregion
    }
}
