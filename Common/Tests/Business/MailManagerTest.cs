using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Business
{
    class MailManagerTest : BaseTest
    {

        [Test]
        public void SendMail_NominalCase()
        {
            var toAddress = "ad1@etsi.org";
            var subject = "Sub";
            var content = "Blah";

            var mailMock = MockRepository.GenerateMock<Etsi.Ultimate.Utils.WcfMailService.ISendMail>();
            mailMock.Stub(mock => mock.SendEmailWithBcc(
                Arg<string>.Is.Equal(ConfigVariables.EmailDefaultFrom), 
                Arg<List<string>>.Matches(l => l.Contains(toAddress)), 
                Arg<List<string>>.Is.Equal(null), 
                Arg<List<string>>.Matches(l => l.Count == 1), 
                Arg<string>.Is.Equal(subject), 
                Arg<string>.Is.Equal(content), 
                Arg<string>.Is.Equal(string.Empty))).Return(true);

            var mailSvc = MailManager.Instance;
            mailSvc.MailClient = mailMock;
            Assert.IsTrue(mailSvc.SendEmail(ConfigVariables.EmailDefaultFrom, new List<string>() {toAddress}, null, new List<string>() { ConfigVariables.EmailDefaultBcc }, subject, content));

            mailMock.VerifyAllExpectations();

        }

        [Test]
        public void SendMail_FillsInFrom()
        {
            var mailMock = MockRepository.GenerateMock<Etsi.Ultimate.Utils.WcfMailService.ISendMail>();
            mailMock.Stub(mock => mock.SendEmailWithBcc(
                Arg<string>.Is.Equal(ConfigVariables.EmailDefaultFrom),
                Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything)).Return(true);

            var mailSvc = MailManager.Instance;
            mailSvc.MailClient = mailMock;
            Assert.IsTrue(mailSvc.SendEmail(String.Empty, new List<string>() {"test"}, null, null, "test", "test"));
            mailMock.VerifyAllExpectations();

        }

        [Test]
        public void SendMail_FillsInBcc()
        {
            var mailMock = MockRepository.GenerateMock<Etsi.Ultimate.Utils.WcfMailService.ISendMail>();
            mailMock.Stub(mock => mock.SendEmailWithBcc(
                Arg<string>.Is.Anything,
                Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Matches(l => l.Contains(ConfigVariables.EmailDefaultBcc)),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything)).Return(true);

            var mailSvc = MailManager.Instance;
            mailSvc.MailClient = mailMock;
            Assert.IsTrue(mailSvc.SendEmail(String.Empty, new List<string>() { "test" }, null, null, "test", "test"));
            mailMock.VerifyAllExpectations();

        }
    }
}
