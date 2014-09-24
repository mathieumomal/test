using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{

    /// <summary>
    /// ChangeRequestCategory
    /// </summary>

    [DataContract]
    public class ChangeRequestCategory
    {
        /// <summary>
        /// Gets or sets the PK_ enum cr category.
        /// </summary>
        /// <value>
        /// The PK_ enum cr category.
        /// </value>        
        [DataMember]
        public int Pk_EnumCRCategory { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        public string Description { get; set; }

    }
}
