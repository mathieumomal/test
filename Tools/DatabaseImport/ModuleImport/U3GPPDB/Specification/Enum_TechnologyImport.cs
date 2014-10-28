namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
{
    public class Enum_TechnologyImport : IModuleImport
    {
        /// <summary>
        /// Old table(s) : 
        /// 2002-04-25_WPM-title-lines & Specs_GSM+3G
        /// </summary>
        /// 
        public const string _2gCode = "2G";
        public const string _3gCode = "3G";
        public const string _lteCode = "LTE";
        public const int _2gWpmProjectId = 744;
        public const int _3gWpmProjectId = 704;
        public const int _lteWpmProjectId = 576;

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
            CreateDatas();
            UltimateContext.SaveChanges();
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            var _2g = new Etsi.Ultimate.DomainClasses.Enum_Technology()
            {
                Code = _2gCode,
                Description = _2gCode,
                WpmProjectId = _2gWpmProjectId
            };
            
            var _3g = new Etsi.Ultimate.DomainClasses.Enum_Technology(){
                Code = _3gCode,
                Description = _3gCode,
                WpmProjectId = _3gWpmProjectId
            };
            
            var _lte = new Etsi.Ultimate.DomainClasses.Enum_Technology()
            {
                Code = _lteCode,
                Description = _lteCode,
                WpmProjectId = _lteWpmProjectId
            };
            UltimateContext.Enum_Technology.Add(_2g);
            UltimateContext.Enum_Technology.Add(_3g);
            UltimateContext.Enum_Technology.Add(_lte);
        }
        #endregion
    }
}
