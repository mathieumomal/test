using System;
using System.IO;
using Etsi.Ultimate.Business.Versions.QualityChecks;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Business.QualityChecks
{
    public class DocxQualityChecksTests : BaseEffortTest
    {
        private readonly string _uploadPath = Environment.CurrentDirectory + "\\TestData\\QualityChecks\\";


        [Test]
        public void IsAutomaticNumberingPresent_OnlyBullet_ShouldSuccess()
        {
            using (Stream fileStream = new FileStream(_uploadPath + "DOCX_BULLET.docx", FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    var biz = new DocXQualityChecks(memoryStream);

                    var result = biz.IsAutomaticNumberingPresent();

                    Assert.IsFalse(result);
                }
            }
        }

        [Test]
        public void IsAutomaticNumberingPresent_BulletAndNumber_ShouldFail()
        {
            using (Stream fileStream = new FileStream(_uploadPath + "DOCX_BOTH.docx", FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    var biz = new DocXQualityChecks(memoryStream);

                    var result = biz.IsAutomaticNumberingPresent();

                    Assert.IsTrue(result);
                }
            }
        }
    }
}
