using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    [DataContract]
    public class ChangeRequestPackFacade
    {
        [DataMember]
        public string Uid { get; set; }

        [DataMember]
        public int MeetingId { get; set; }

        [DataMember]
        public string Source { get; set; }

        public override string ToString()
        {
            return string.Format("Uid: {0}, MeetingId: {1}, Source: {2}", Uid, MeetingId, Source);
        }  
    }
}
