using System;
using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    [DataContract]
    public class Community
    {
        [DataMember]
        public int TbId { get; set; }

        [DataMember]
        public string TbName { get; set; }

        [DataMember]
        public string TbType { get; set; }

        [DataMember]
        public string TbTitle { get; set; }

        [DataMember]
        public Nullable<int> ParentTbId { get; set; }

        [DataMember]
        public string ActiveCode { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        [DataMember]
        public string DetailsURL { get; set; }
    }
}
