namespace Etsi.Ultimate.Utils
{
    public class SpecVersionPathHelper : ISpecVersionPathHelper
    {
        public string GetFtpArchivePath
        {
            get { return "{0}\\Specs\\archive\\{1}_series\\{2}\\"; }
        }

        public string GetFtpLatestPath
        {
            get { return "{0}\\Specs\\latest\\{1}\\{2}_series\\"; }
        }

        public string GetFtpLatestDraftsPath
        {
            get { return "{0}\\Specs\\latest-drafts\\"; }
        }

        public string GetFtpCustomPath
        {
            get { return "{0}\\Specs\\{1}\\{2}\\{3}_series\\"; }
        }

        public string GetFtpLatestPathBase
        {
            get { return "{0}\\Specs\\latest"; }
        }
    }

    public interface ISpecVersionPathHelper
    {
        /// <summary>
        /// {0}\\Specs\\archive\\{1}_series\\{2}\\
        /// </summary>
        string GetFtpArchivePath { get; }

        /// <summary>
        /// {0}\\Specs\\latest\\{1}\\{2}_series\\
        /// </summary>
        string GetFtpLatestPath { get; }

        /// <summary>
        /// {0}\\Specs\\latest-drafts\\
        /// </summary>
        string GetFtpLatestDraftsPath { get; }

        /// <summary>
        /// {0}\\Specs\\{1}\\{2}\\{3}_series\\
        /// </summary>
        string GetFtpCustomPath { get; }

        /// <summary>
        /// {0}\\Specs\\latest
        /// </summary>
        string GetFtpLatestPathBase { get; }
    }
}
