﻿using System;
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

        public List<int> FullSpecificationRapporteurs
        {
            get
            {
                List<int> result = new List<int>();
                if (SpecificationRapporteurs != null && SpecificationRapporteurs.Count > 0)
                    SpecificationRapporteurs.ToList().ForEach(r => result.Add(r.Fk_RapporteurId));
                return result;
            }
        }

        public List<int> PrimeSpecificationRapporteurs
        {
            get
            {
                List<int> result = new List<int>();
                if (SpecificationRapporteurs != null && SpecificationRapporteurs.Where(r => r.IsPrime).ToList().Count >0 )
                    SpecificationRapporteurs.Where(r => r.IsPrime).ToList().ForEach(r => result.Add(r.Fk_RapporteurId));
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
                    if ((IsUnderChangeControl != null) && (IsUnderChangeControl.Value))
                        specificationStatus = "under change control";
                }

                else
                {
                    if (IsUnderChangeControl == null || !IsUnderChangeControl.Value)
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
                if (IsTS == null)
                    return specificationType;
                if (IsTS.Value)
                {
                    specificationType = "TS";
                }
                else
                {
                    specificationType = "TR";
                }
                return specificationType;
            }
        }

        public string SpecificationTypeFullText
        {
            get
            {
                string specificationType = string.Empty;
                if (IsTS == null)
                    return specificationType;
                if (IsTS.Value)
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

        public string SpecificationInitialRelease { get; set; }

        public List<Enum_Technology> SpecificationTechnologiesList { get; set; }
        public List<WorkItem> SpecificationWIsList { get; set; }

        public string SpecificationTypeShortName
        {
            get
            {
                if (IsTS == null)
                    return String.Empty;
                else
                    return (IsTS.Value) ? "TS" : "TR";
            }
        }
    }
}