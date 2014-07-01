using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils.WcfMailService;

namespace Etsi.Ultimate.Utils
{
    /// <summary>
    /// Default implementation of the IMail Manager. This service uses the Mail Service developped during the NGPP project to send the mails
    /// outside.
    /// </summary>
    public class MailManager : IMailManager
    {
     
        #region IMailManager Members

        /// <summary>
        /// See Interface definition.
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddresses"></param>
        /// <param name="ccAddresses"></param>
        /// <param name="bccAddresses"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendEmail(string fromAddress, List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, string subject, string body)
        {
            // Initialize from if needed
            if (string.IsNullOrEmpty(fromAddress) && !string.IsNullOrEmpty(ConfigVariables.EmailDefaultFrom))
            {
                fromAddress = ConfigVariables.EmailDefaultFrom;
            }

            // Initialize CC
            if (ccAddresses == null)
            {
                ccAddresses = new List<string>();
            }

            // Initialize BCC
            if (bccAddresses == null)
            {
                bccAddresses = new List<string>();
            }
            if (!bccAddresses.Contains(ConfigVariables.EmailDefaultBcc))
            {
                bccAddresses.Add(ConfigVariables.EmailDefaultBcc);
            }
            try
            {
                var mailClient = UtilsFactory.Resolve<ISendMail>();
                return mailClient.SendEmailWithBcc(fromAddress, toAddresses, ccAddresses, bccAddresses, subject, body, String.Empty);
            }
            catch (Exception e)
            {
                Utils.LogManager.Error("Failed to send email: " + e.Message);
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// Service in charge of sending emails to the outside world.
    /// </summary>
    public interface IMailManager
    {
        /// <summary>
        /// Sends an email.
        /// 
        /// Adds to any email the 3GPP address in BCC.
        /// </summary>
        /// <param name="fromAddress">Address to send from</param>
        /// <param name="toAddresses">List of addresses to send to</param>
        /// <param name="ccAddresses">List of addresses to send mail as CC</param>
        /// <param name="bccAddresses">List of addresses to send mail as BCC</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="body">HTML formated body of the email.</param>
        /// <returns></returns>
        bool SendEmail(string fromAddress, List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, string subject, string body);

    }
}
