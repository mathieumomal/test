using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class Specification
    {

        public string PrimeResponsibleGroupShortName
        {
            get;
            set;
        }


        public SpecificationResponsibleGroup PrimeResponsibleGroup
        {
            get
            {
                return SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().FirstOrDefault();
            }
        }

        public string SecondaryResponsibleGroupsShortNames
        {
            get;
            set;

        }

        public List<int> PrimeSpecificationRapporteurs
        {
            get
            {
                List<int> result = new List<int>();
                SpecificationRapporteurs.Where(r => r.IsPrime).ToList().ForEach(r => result.Add(r.Fk_RapporteurId));
                return result;
            }
        }

        public List<int> FullSpecificationRapporteurs
        {
            get
            {
                List<int> result = new List<int>();
                SpecificationRapporteurs.ToList().ForEach(r => result.Add(r.Fk_RapporteurId));
                return result;
            }
        }

        public string Status
        {
            get
            {
                string specificationStatus = string.Empty;
                if (IsActive){
                    specificationStatus = "Draft";
                    if(IsUnderChangeControl.Value)
                        specificationStatus = "under change control";
                }

                else
                {
                    if (!IsUnderChangeControl.Value)
                        specificationStatus = "Withdrawn before change control";
                    else
                        specificationStatus = "Withdrawn after change control";
                }

                return specificationStatus;
            }
        }

        public string SpecificationType
        {
            get{
                string specificationType = string.Empty;
                if (Type == null)
                    return specificationType;
                if (Type.Value)
                {
                    specificationType = "Technical specification (TS)";
                }
                else
                {
                    specificationType = "Technical report (TR)";
                }
                return specificationType;
            }
        }
    }
}
