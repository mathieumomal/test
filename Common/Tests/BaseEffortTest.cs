using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effort.DataLoaders;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests
{
    public abstract class BaseEffortTest
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

        public bool isTestReadOnly;

        /// <summary>
        /// Initialization of the context by a set of csv files
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            
            //CSV files are stored in a specific folder : Data_For_EffortUnitTests
            string dir = new StringBuilder()
                .Append(Environment.CurrentDirectory)
                .Append(CsvPath)
                .ToString();
            
            // If Context is not null, it means that we have not cleaned the context, because
            // the previous test said it was readonly ==> we don't re-create it.
            if (Context == null)
            {

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
            }
            
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), Context);
            RepositoryFactory.Container.RegisterType<IUltimateUnitOfWork,EffortUnitOfWork>(new TransientLifetimeManager() );
            UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            // By default, a test is not readonly
            isTestReadOnly = false;
        }


        [TearDown]
        public virtual void TearDown()
        {
            if (!isTestReadOnly)
            {
                Context.Dispose();
                Context = null;
            }
        }
    }
}
