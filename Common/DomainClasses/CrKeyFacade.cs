namespace Etsi.Ultimate.DomainClasses
{
    /// <summary>
    /// CR key
    /// SpecId/SpecNumber, CrNumber, Revision, TsgTdocNumber
    /// </summary>
    public class CrKeyFacade
    {
        /// <summary>
        /// Gets or sets the spec identifier.
        /// </summary>
        public int SpecId { get; set; }

        /// <summary>
        /// Gets or sets the spec number.
        /// </summary>
        public string SpecNumber { get; set; }

        /// <summary>
        /// Gets or sets the cr number.
        /// </summary>
        public string CrNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the TSG tdoc number.
        /// </summary>
        public string TsgTdocNumber { get; set; }

        /// <summary>
        /// Gets or sets the TSG source organization.
        /// </summary>
        public string TsgSourceOrganization { get; set; }

        /// <summary>
        /// Gets or sets the TSG meeting identifier.
        /// </summary>
        public int TsgMeetingId { get; set; }

        /// <summary>
        /// Identifier for log messages
        /// </summary>
        /// <returns></returns>
        public string GetIdentifierForLog()
        {
            return CrNumber + " for revision " + Revision;
        }
    }
}
