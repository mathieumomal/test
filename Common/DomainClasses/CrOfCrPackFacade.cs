namespace Etsi.Ultimate.DomainClasses
{
    /// <summary>
    /// CR key
    /// SpecId/SpecNumber, CrNumber, Revision, TsgTdocNumber
    /// </summary>
    public class CrOfCrPackFacade
    {
        /// <summary>
        /// Gets or sets the TSG tdoc number.
        /// </summary>
        public string TsgTdocNumber { get; set; }

        /// <summary>
        /// Gets or sets the spec identifier.
        /// </summary>
        public int SpecId { get; set; }

        /// <summary>
        /// Gets or sets the cr number.
        /// </summary>
        public string CrNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        public int RevisionNumber { get; set; }

        /// <summary>
        /// Gets or sets the Status (string).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Status (Pk Enum).
        /// </summary>
        public int? PkEnumStatus { get; set; }
    }
}
