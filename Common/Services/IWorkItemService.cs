using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public interface IWorkItemService
    {
        /// <summary>
        /// Analyse imported workItems and return errors and warnings logs + cache token
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        KeyValuePair<int, ImportReport> AnalyseWorkItemForImport(String path);
        /// <summary>
        /// Import workitems uploaded on the server
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        String ImportWorkItem(int token);
    }
}
