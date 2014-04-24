using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImport.ModuleImport
{
    public class Enum_SerieImport : IModuleImport
    {
        /// <summary>
        /// Old table(s) : 
        /// jmm_spec_series
        /// </summary>
        public const string CodePrefixe = "SER_";
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext{get;set;}
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext{get;set;}
        public Etsi.Ultimate.DomainClasses.ImportReport Report{get;set;}

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            CreateTable();
            NewContext.SaveChanges();
        }

        #endregion

        #region migration methods
        private void CreateTable()
        {
            foreach (var elt in LegacyContext.jmm_spec_series)
            {
                var newSerie = new Domain.Enum_Serie();

                //Code <=> SER_01
                newSerie.Code = new StringBuilder()
                                        .Append(CodePrefixe)
                                        .Append(elt.series_id.Split('.')[0])
                                        .ToString();
                //Code <=> 01. Description
                newSerie.Description = new StringBuilder()
                                            .Append(elt.series_id)
                                            .Append(" ")
                                            .Append(elt.series_description.Trim())
                                            .ToString();

                //True False
                newSerie.Series_2g = elt.series_2g;
                newSerie.Series_3g = elt.series_3g;

                NewContext.Enum_Serie.Add(newSerie);
            }
        }
        #endregion
    }
}
