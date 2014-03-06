﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class WorkItemServiceMock : IWorkItemService
    {
        #region IWorkItemService Membres

        public KeyValuePair<int, ImportReport> AnalyseWorkItemForImport(String path)
        {
            ImportReport myImportReport = new ImportReport();
            myImportReport.ErrorList.Add("Error 1");
            myImportReport.ErrorList.Add("Error 2");
            myImportReport.ErrorList.Add("Error 3");
            myImportReport.WarningList.Add("Warning 1");
            myImportReport.WarningList.Add("Warning 2");
            myImportReport.WarningList.Add("Warning 3");
            KeyValuePair<int, ImportReport> analyseWorkItemForImportResult = new KeyValuePair<int, ImportReport>(15, myImportReport);
            return analyseWorkItemForImportResult;
        }

        public string ImportWorkItem(int token)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}