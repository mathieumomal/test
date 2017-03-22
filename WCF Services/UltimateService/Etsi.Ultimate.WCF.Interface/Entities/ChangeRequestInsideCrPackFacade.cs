using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    [DataContract]
    public class ChangeRequestInsideCrPackFacade
    {
        [DataMember]
        public int Id { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}", Id);
        }  
    }
}
