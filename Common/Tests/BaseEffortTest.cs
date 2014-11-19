using System;
using System.Data.Entity.Core.EntityClient;
using System.Text;
using Effort.DataLoaders;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests
{
    public abstract class BaseEffortTest: BaseTest
    {
        /// <summary>
        /// Context
        /// </summary>
        public IUltimateContext Context { get; set; }

        /// <summary>
        /// UoW
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// CSVs folder path
        /// </summary>
        public string CsvPath = "\\Data_For_EffortUnitTests\\";
        /// <summary>
        /// Context name (present in the App.config)
        /// </summary>
        public string ContextName = "UltimateContext";

        /// <summary>
        /// Initialization of the context by a set of csv files
        /// </summary>
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            //CSV files are stored in a specific folder : Data_For_EffortUnitTests
            string dir = new StringBuilder()
                .Append(Environment.CurrentDirectory)
                .Append(CsvPath)
                .ToString();

            //Use Effort framework
            IDataLoader loader = new Effort.DataLoaders.CsvDataLoader(dir);
            try
            {
                EntityConnection connection = Effort.EntityConnectionFactory.CreateTransient(
                    new StringBuilder().Append("name=").Append(ContextName).ToString(),
                    loader);
                //Context creation
                Context = new UltimateContext(connection);
            }
            catch (Exception e)
            {
                var error = new StringBuilder()
                    .Append("An error occured when effort try to import CSVs files and/or load the context... Please check the next logs :")
                    .Append(" MESSAGE : ")
                    .Append(e.Message)
                    .Append(" INNEREXCEPTION : ")
                    .Append(e.InnerException)
                    .ToString();
                throw new Exception(error);
            }
            
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), Context);
            RepositoryFactory.Container.RegisterType<IUltimateUnitOfWork,EffortUnitOfWork>(new TransientLifetimeManager() );
            UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
        }


        [TearDown]
        public virtual void TearDown()
        {
            Context.Dispose();
        }
    }
}
