using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Business
{
    public class ConfigVariableTest: BaseTest
    {
        [Test]
        public void ConfigVariable_DefaultFrom()
        {
            Assert.IsNotNullOrEmpty(ConfigVariables.EmailDefaultFrom);
        }

        [Test]
        public void ConfigVariable_DefaultBcc()
        {
            Assert.IsNotNullOrEmpty(ConfigVariables.EmailDefaultBcc);
        }
    }
}
