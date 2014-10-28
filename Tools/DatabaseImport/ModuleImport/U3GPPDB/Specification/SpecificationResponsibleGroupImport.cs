using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
{
    /// <summary>
    /// Generate SpecificationResponsibleGroup table datas
    /// </summary>
    public class SpecificationResponsibleGroupImport : IModuleImport
    {
        public const string RefImportForLog = "[Specification/ResponsibleGroup]";
        public List<Community> tmpCommunities = new List<Community>();
        public List<Etsi.Ultimate.DomainClasses.Specification> tmpSpecs = new List<Etsi.Ultimate.DomainClasses.Specification>();
        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G (WG prim and other)
        /// </summary>
        /// 
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            UltimateContext.SetAutoDetectChanges(false);
            CreateDatas();
            UltimateContext.SetAutoDetectChanges(true);
            UltimateContext.SaveChanges();
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
                tmpCommunities = UltimateContext.Communities.ToList();
                tmpSpecs = UltimateContext.Specifications.ToList();

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
        private void ResponsibleGroupCase(Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G legacySpecifification, bool isPrime)
        {
            string groupsCode = "";
            if(isPrime)//We get WG_prime value from legacy specs column because we search the prime Responsiblegroups
                groupsCode = Utils.CheckString(legacySpecifification.WG_prime, 0, RefImportForLog + "PrimeResponGroup", "");
            else//We get WG_other value from legacy specs column because we search the other Responsiblegroups
                groupsCode = Utils.CheckString(legacySpecifification.WG_other, 0, RefImportForLog + "NoPrimeResponGroup", "");
            //If we had found any groupCode :
            if (!String.IsNullOrEmpty(groupsCode) && groupsCode != "-")
            {
                String[] respGroups = null;
                //We split the geting field to have a GroupCode table
                if (groupsCode.Contains(',') && groupsCode.Contains('/'))
                    LogManager.LogWarning(RefImportForLog + " too many kind of separator.");
                else if (groupsCode.Contains(','))
                    respGroups = groupsCode.Split(',');
                else
                    respGroups = groupsCode.Split('/');
                //Foreach groups found
                foreach (var respGroup in respGroups)
                {
                    var cleanRespGroup = Utils.CheckString(respGroup, 0, RefImportForLog, "");
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
                                LogManager.LogWarning(RefImportForLog + " Spec : " + legacySpecifification.Number + " not found.");
                            }
                            else
                            {
                                var responsibleGroup = new SpecificationResponsibleGroup()
                                {
                                    IsPrime = isPrime,
                                    Fk_commityId = communityGroup.TbId,
                                    Fk_SpecificationId = spec.Pk_SpecificationId,
                                    Specification = spec
                                };
                                UltimateContext.SpecificationResponsibleGroups.Add(responsibleGroup);
                            }
                        }
                        else
                        {
                            LogManager.LogWarning(RefImportForLog + " Group : " + cleanRespGroup + " not found in community table.");
                        }
                    }
                }
            }
        }




        
        #endregion
    }
}
