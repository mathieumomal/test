﻿using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses.Facades;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class Specification
    {
        #region General data
        public string SpecNumberAndTitle
        {
            get
            {
                return Number + " " + Title;
            }
        }

        /// <summary>
        /// Name of the release initially targeted by the specification.
        /// </summary>
        public string SpecificationInitialRelease { get; set; }

        public List<Enum_Technology> SpecificationTechnologiesList { get; set; }
        public List<WorkItem> SpecificationWIsList { get; set; }
        public List<Release> SpecificationReleases { get; set; }

        /// <summary>
        /// Specify whether new vesion creation is enbaled or not in massive promotion case
        /// </summary>
        public bool IsNewVersionCreationEnabled { get; set; }
        #endregion

        #region Prime responsible group
        public string PrimeResponsibleGroupShortName { get; set; }

        public string PrimeResponsibleGroupFullName { get; set; }

        public string PrimeSpecificationRapporteurName { get; set; }

        public SpecificationResponsibleGroup PrimeResponsibleGroup
        {
            get
            {
                return (SpecificationResponsibleGroups != null && SpecificationResponsibleGroups.Count > 0) ? SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().FirstOrDefault() : null;
            }
        }

        public List<int> PrimeResponsibleGroupId
        {
            get
            {
                return (SpecificationResponsibleGroups != null && SpecificationResponsibleGroups.Count > 0) ? SpecificationResponsibleGroups.Where(g => g.IsPrime).Select(x => x.Fk_commityId).ToList() : new List<int>();
            }
        } 
        #endregion

        #region Secondary responsible groups
        public string SecondaryResponsibleGroupsShortNames { get; set; }

        public string SecondaryResponsibleGroupsFullNames { get; set; }

        public List<int> SecondaryResponsibleGroupsIds
        {
            get
            {
                return (SpecificationResponsibleGroups != null && SpecificationResponsibleGroups.Count > 0) ? SpecificationResponsibleGroups.Where(g => !g.IsPrime).Select(x => x.Fk_commityId).ToList() : new List<int>();
            }
        } 
        #endregion

        #region Rapporteurs
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


        /// <summary>
        /// Returns the list of IDs of all the rapporteurs specified as prime.
        /// </summary>
        public List<int> PrimeSpecificationRapporteurIds
        {
            get
            {
                List<int> result = new List<int>();
                if (SpecificationRapporteurs != null && SpecificationRapporteurs.Where(r => r.IsPrime).ToList().Count > 0)
                    result.AddRange(SpecificationRapporteurs.Where(r => r.IsPrime).Select(r => r.Fk_RapporteurId).ToList());
                return result;
            }
        }
        #endregion

        #region Status
        public string Status
        {
            get
            {
                string specificationStatus = string.Empty;
                if (IsActive)
                {
                    specificationStatus = "Draft";
                    if ((IsUnderChangeControl != null) && (IsUnderChangeControl.Value))
                        specificationStatus = "Under change control";
                }

                else
                {
                    if (IsUnderChangeControl == null || !IsUnderChangeControl.Value)
                        specificationStatus = "Withdrawn before change control";
                    else
                        specificationStatus = "Withdrawn under change control";
                }

                return specificationStatus;
            }
        }

        public string ShortStatus
        {
            get
            {
                string specificationStatus = string.Empty;
                if (IsActive)
                {
                    specificationStatus = "Draft";
                    if ((IsUnderChangeControl != null) && (IsUnderChangeControl.Value))
                        specificationStatus = "UCC";
                }

                else
                {
                    if (IsUnderChangeControl == null || !IsUnderChangeControl.Value)
                        specificationStatus = "Wdrn";
                    else
                        specificationStatus = "Wdrn(CC)";
                }

                return specificationStatus;
            }
        }
        #endregion

        #region Type
        public string SpecificationType
        {
            get
            {
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
        #endregion

        public override string ToString()
        {
            return string.Format("ID: {0}, Number: {1}", Pk_SpecificationId, Number);
        }
    }
}
