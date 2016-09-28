using System.Linq;
using Etsi.Ultimate.Utils;
using NUnit.Framework;
using System.Configuration;

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
