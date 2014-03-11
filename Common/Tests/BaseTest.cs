using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests
{
    public class BaseTest
    {
        [SetUp]
        public virtual void Setup()
        {
            ManagerFactory.SetDefaultDependencies();
            ServicesFactory.SetDefaultDependencies();
            RepositoryFactory.SetDefaultDependencies();
        }
    }
}
