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
        public const string RefImportForLog = "[Specification/ResponsibleGroup]";
        public List<Community> tmpCommunities = new List<Community>();
        public List<Specification> tmpSpecs = new List<Specification>();
        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G (WG prim and other)
        /// </summary>
        /// 
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

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
                //Get communities list once
                tmpCommunities = NewContext.Communities.ToList();
                tmpSpecs = NewContext.Specifications.ToList();

                ResponsibleGroupCase(legacySpecifification, true);

                ResponsibleGroupCase(legacySpecifification, false);

                count++;
                if(count%100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }

        }

        /// <summary>
        /// Responsible group creation in two case : 
        /// - Prime case (relative to WG_prime legacy specifications column)
        /// - Other case (relative to WG_other legacy specifications column)
        /// </summary>
        /// <param name="legacySpecifification"></param>
        /// <param name="isPrime"></param>
        private void ResponsibleGroupCase(OldDomain.Specs_GSM_3G legacySpecifification, bool isPrime)
        {
            string groupsCode = "";
            if(isPrime)//We get WG_prime value from legacy specs column because we search the prime Responsiblegroups
                groupsCode = Utils.CheckString(legacySpecifification.WG_prime, 0, RefImportForLog + "PrimeResponGroup", "", Report);
            else//We get WG_other value from legacy specs column because we search the other Responsiblegroups
                groupsCode = Utils.CheckString(legacySpecifification.WG_other, 0, RefImportForLog + "NoPrimeResponGroup", "", Report);
            //If we had found any groupCode :
            if (!String.IsNullOrEmpty(groupsCode) && groupsCode != "-")
            {
                String[] respGroups = null;
                //We split the geting field to have a GroupCode table
                if (groupsCode.Contains(',') && groupsCode.Contains('/'))
                    Report.LogWarning(RefImportForLog + " too many kind of separator.");
                else if (groupsCode.Contains(','))
                    respGroups = groupsCode.Split(',');
                else
                    respGroups = groupsCode.Split('/');
                //Foreach groups found
                foreach (var respGroup in respGroups)
                {
                    var cleanRespGroup = Utils.CheckString(respGroup, 0, RefImportForLog, "", Report);
                    if (!String.IsNullOrEmpty(cleanRespGroup))
                    {
                        //If we found a community corresponding to this GroupCode : 
                        var communityGroup = tmpCommunities.Where(x => x.ShortName == cleanRespGroup).FirstOrDefault();
                        if (communityGroup != null)
                        {
                            //If we found a new spec corresponding to the legacy spec
                            var spec = tmpSpecs.Where(x => x.Number == legacySpecifification.Number).FirstOrDefault();
                            if (spec == null)
                            {
                                Report.LogWarning(RefImportForLog + " Spec : " + legacySpecifification.Number + " not found.");
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
                            Report.LogWarning(RefImportForLog + " Group : " + cleanRespGroup + " not found in community table.");
                        }
                    }
                }
            }
        }




        
        #endregion
    }
}
