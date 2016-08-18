using System;
using System.Linq;
using System.Text;

namespace Etsi.Ultimate.DomainClasses
{
    public class EtsiWorkItemImport
    {
        #region Properties
        public string EtsiNumber { get; set; }
        public string StandardType { get; set; }

        //Example: spec number = 36.512-3
        //- EtsiDocNumber = 36512
        //- EtsiPartNumber = 3
        public int EtsiDocNumber { get; set; }
        public int? EtsiPartNumber { get; set; }

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

            if (specification.Number.Contains("-"))//ex: 36.512-3
            {
                EtsiDocNumber = int.Parse(specification.Number.Split('-')[0].Replace(".", ""));
                EtsiPartNumber = int.Parse(specification.Number.Split('-')[1]);
            }
            else//ex: 36.512
            {
                EtsiDocNumber = Int32.Parse(specification.Number.Replace(".", ""));
                EtsiPartNumber = null;
            }

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
            var tempSerialNumber = new StringBuilder();
            tempSerialNumber.Append(EtsiDocNumber);
            tempSerialNumber.Append(EtsiPartNumber != null ? "-" + EtsiPartNumber : string.Empty);
            tempSerialNumber.Append("v" + version);

            SerialNumber = tempSerialNumber.ToString();
            //Reference completed
            Reference += SerialNumber;
        }

        #endregion
    }
}
