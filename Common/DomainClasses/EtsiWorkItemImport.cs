using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public class EtsiWorkItemImport
    {
        private const string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        public string EtsiNumber { get; set;}
        public string StandardType { get; set; }
        public int EtsiDocNumber { get; set; }
        public string Reference { get; set; }
        public string SerialNumber { get; set; }
        public string Version { get; set; }
        public int CommunityId { get; set; }
        public string titllePart_1 { get; set; }
        public string titllePart_2 { get; set; }
        public string titllePart_3 { get; set; }
        public int RapporteurId { get; set; }
        public int SecretaryId { get; set; }
        public string WorkingTitle { get; set; }

        #region constructors

        public EtsiWorkItemImport()
        {
        }

        public EtsiWorkItemImport(SpecVersion version, Specification specification, Community community, int wgNumber , int SecretaryId, string releaseName, bool isAlreadyTransposed)
        {
            EtsiNumber = "1" + specification.Number.Replace(".", " ");
            StandardType = specification.SpecificationType; 
            EtsiDocNumber = Int32.Parse(specification.Number.Replace(".", ""));
            SerialNumber = EtsiDocNumber + "v" + Encode(version.MajorVersion.GetValueOrDefault()) + Encode(version.TechnicalVersion.GetValueOrDefault()) + Encode(version.EditorialVersion.GetValueOrDefault());
            //TOCOMPLETE
            Reference = (isAlreadyTransposed ? "R" : "D") + StandardType + "/TSG" + community.ShortName.Trim().ElementAt(0) + "-" + (community.TbType.Equals("TSG") ? "00" : ("0" + wgNumber)) + SerialNumber;
            Version = version.Version;
            CommunityId = community.TbId;
            titllePart_1 = "";
            specification.SpecificationTechnologies.ToList().ForEach(e => { titllePart_1 += e.Enum_Technology.Description + "; "; });
            titllePart_1 = titllePart_1.Trim();
            titllePart_2 = specification.Title;
            titllePart_3 = "3GPP " + StandardType + " " + specification.Number + " version " + version.Version + " " + releaseName;
            RapporteurId = specification.PrimeSpecificationRapporteurIds.FirstOrDefault();
            this.SecretaryId = SecretaryId;
            WorkingTitle = specification.Title;

        }

        #endregion

        private string Encode(int input)
        {
            char[] baseElements = base36.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(baseElements[input % 36]);
                input /= 36;
            }
            return new string(result.ToArray());
        }
    }
}
