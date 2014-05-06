using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Utils
{
    /// <summary>
    /// Default implementation of the IMail Manager. This service uses the Mail Service developped during the NGPP project to send the mails
    /// outside.
    /// 
    /// This class is a Singleton.
    /// </summary>
    public class MailManager : IMailManager
    {
        private static IMailManager instance;

        /// <summary>
        /// Retrieves the Instance of the mail manager. If instance is not yet set, instantiate it.
        /// 
        /// Note: you might set the instance by yourself if needed (Unit test, specific call)
        /// </summary>
        public static IMailManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MailManager();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        private WcfMailService.ISendMail _mailClient;
        public WcfMailService.ISendMail MailClient
        {
            get
            {
                if (_mailClient == null)
                {
                    _mailClient = new WcfMailService.SendMailClient();
                }
                return _mailClient;
            }
            set
            {
                _mailClient = value;
            }
        }

        /// <summary>
        /// Private constructor. Call GetInstance() to retrieve the instance.
        /// </summary>
        private MailManager() { }


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

            // Initialize BCC
            if (bccAddresses == null)
            {
                bccAddresses = new List<string>();
            }
            if (!bccAddresses.Contains(ConfigVariables.EmailDefaultBcc))
            {
                bccAddresses.Add(ConfigVariables.EmailDefaultBcc);
            }

            return MailClient.SendEmailWithBcc(fromAddress, toAddresses, ccAddresses, bccAddresses, subject, body, String.Empty);
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

        /// <summary>
        /// Mail client used to send the email.
        /// </summary>
        WcfMailService.ISendMail MailClient { get; set; }
    }
}
