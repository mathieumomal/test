using Etsi.Dsdb.DataAccess;
using Etsi.UserRights.DNN3GPPDataAccess;
using Etsi.UserRights.Service;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UserRightsService.Tests
{
    public class BaseEffortTest
    {
        /// <summary>
        /// Initialization of the context by a set of csv files
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            DatabaseFactory.Container.RegisterType<IDSDBContext, DsdbEffortContext>(new TransientLifetimeManager());
            DatabaseFactory.Container.RegisterType<DSDBContext>(new InjectionConstructor());
            DatabaseFactory.Container.RegisterType<IDnn3gppContext, Dnn3gppEffortContext>(new TransientLifetimeManager());
            DatabaseFactory.Container.RegisterType<DNN3GPPContext>(new InjectionConstructor());
        }
    }
}
