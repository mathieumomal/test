using System;
using System.Linq;

namespace Etsi.Ultimate.DomainClasses
{
    public class EtsiWorkItemImport
    {
        #region Properties
        public string EtsiNumber { get; set; }
        public string StandardType { get; set; }
        public int EtsiDocNumber { get; set; }
        public string Reference { get; set; }
        public string SerialNumber { get; set; }
        public string Version { get; set; }
        public int CommunityId { get; set; }
        public string TitlePart1 { get; set; }
        public string TitlePart2 { get; set; }
        public string TitlePart3 { get; set; }
        public int RapporteurId { get; set; }
        public int SecretaryId { get; set; }
        public string WorkingTitle { get; set; }
        #endregion

        #region constructors

        public EtsiWorkItemImport()
        {
        }

        public EtsiWorkItemImport(SpecVersion version, Specification specification, Community community, int wgNumber, int SecretaryId, string releaseName, bool isAlreadyTransposed)
        {            

            EtsiNumber = "1" + specification.Number.Replace(".", " ");
            StandardType = specification.SpecificationType;
            EtsiDocNumber = Int32.Parse(specification.Number.Replace(".", ""));
            //Reference not yet completed
            Reference = (isAlreadyTransposed ? "R" : "D") + StandardType + "/TSG" + community.ShortName.Trim().ElementAt(0) + "-" + (community.TbType.Equals("TSG") ? "00" : ("0" + wgNumber));
            Version = version.Version;
            CommunityId = community.TbId;
            TitlePart1 = "";
            specification.SpecificationTechnologies.ToList().ForEach(e => { TitlePart1 += e.Enum_Technology.Description + "; "; });
            TitlePart1 = TitlePart1.Trim();
            TitlePart2 = specification.Title;
            TitlePart3 = "(3GPP " + StandardType + " " + specification.Number + " version " + version.Version + " " + releaseName + ")";
            RapporteurId = specification.PrimeSpecificationRapporteurIds.FirstOrDefault();
            this.SecretaryId = SecretaryId;
            WorkingTitle = specification.Title;

        }

        public void SetSerialNumber(string version)
        {
            SerialNumber = EtsiDocNumber + "v" + version;
            //Reference completed
            Reference += SerialNumber;
        }

        #endregion
    }
}
