﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;


namespace DatabaseImport.ModuleImport
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
            CreateDatas();
            NewContext.SaveChanges();
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            var _3g = new Domain.Enum_Technology(){
                Code = _3gCode,
                Description = _3gCode
            };
            var _2g = new Domain.Enum_Technology()
            {
                Code = _2gCode,
                Description = _2gCode
            };
            var _lte = new Domain.Enum_Technology()
            {
                Code = _lteCode,
                Description = _lteCode
            };
            NewContext.Enum_Technology.Add(_3g);
            NewContext.Enum_Technology.Add(_2g);
            NewContext.Enum_Technology.Add(_lte);
        }
        #endregion
    }
}