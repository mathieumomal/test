using System;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
{
    public class Enum_SerieImport : IModuleImport
    {
        /// <summary>
        /// Old table(s) : 
        /// jmm_spec_series
        /// </summary>
        public const string CodePrefixe = "SER_";
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext{get;set;}
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext{get;set;}
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            CreateTable();
            try
            {
                UltimateContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var test = ex;
                Console.WriteLine(ex.InnerException);
                Console.ReadLine();
            }
        }

        #endregion

        #region migration methods
        private void CreateTable()
        {
            foreach (var elt in LegacyContext.jmm_spec_series)
            {
                var newSerie = new Etsi.Ultimate.DomainClasses.Enum_Serie();

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

                UltimateContext.Enum_Serie.Add(newSerie);
            }
        }
        #endregion
    }
}
