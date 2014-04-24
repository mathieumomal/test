using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service = Etsi.Ultimate.Services;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Business;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace DatabaseImport.ModuleImport
{
    /// <summary>
    /// Generate SpecificationResponsibleGroup table datas
    /// </summary>
    public class SpecificationResponsibleGroupImport : IModuleImport
    {
        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G (WG prim and other)
        /// </summary>
        /// 
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.ImportReport Report { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            NewContext.SetAutoDetectChanges(false);
            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
            NewContext.SaveChanges();
            //catch(DbUpdateException ex) {
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            var total = LegacyContext.Specs_GSM_3G.Count();
            var count = 0;
            foreach (var legacySpecifification in LegacyContext.Specs_GSM_3G)
            {
                ResponsibleGroupCase(legacySpecifification, true);

                ResponsibleGroupCase(legacySpecifification, false);

                count++;
                Console.WriteLine(String.Format("Responsible Group {0}/{1}", count, total));
            }

        }

        private void ResponsibleGroupCase(OldDomain.Specs_GSM_3G legacySpecifification, bool isPrime)
        {
            string groupsCode = "";
            if(isPrime)
                groupsCode = Utils.CheckString(legacySpecifification.WG_prime, 0, "PrimeResponGroup", "", Report);
            else
                groupsCode = Utils.CheckString(legacySpecifification.WG_other, 0, "PrimeResponGroup", "", Report);
            if (!String.IsNullOrEmpty(groupsCode) && !groupsCode.Equals("-"))
            {
                String[] respGroups = null;
                if (groupsCode.Contains(',') && groupsCode.Contains('/'))
                {
                    Report.LogWarning("[Responsible group] too many kind of separator.");
                }
                else if (groupsCode.Contains(','))
                {
                    respGroups = groupsCode.Split(',');
                }
                else
                {
                    respGroups = groupsCode.Split('/');
                }
                foreach (var respGroup in respGroups)
                {
                    var cleanRespGroup = Utils.CheckString(respGroup, 0, "ResponGroup", "", Report);
                    if (!String.IsNullOrEmpty(cleanRespGroup))
                    {
                        var communityGroup = NewContext.Communities.Where(x => x.ShortName == cleanRespGroup).FirstOrDefault();
                        if (communityGroup != null)
                        {
                            var spec = NewContext.Specifications.Where(x => x.Number == legacySpecifification.Number).FirstOrDefault();
                            if (spec == null)
                            {
                                Report.LogWarning("[Responsible group] Spec : " + legacySpecifification.Number + " not found.");
                            }
                            else
                            {
                                var responsibleGroup = new Domain.SpecificationResponsibleGroup()
                                {
                                    IsPrime = isPrime,
                                    Fk_commityId = communityGroup.TbId,
                                    Fk_SpecificationId = spec.Pk_SpecificationId,
                                    Specification = spec
                                };
                                NewContext.SpecificationResponsibleGroups.Add(responsibleGroup);
                            }
                        }
                        else
                        {
                            Report.LogWarning("[Responsible group] Group : " + cleanRespGroup + " not found in community table.");
                        }
                    }
                }
            }
        }




        
        #endregion
    }
}
