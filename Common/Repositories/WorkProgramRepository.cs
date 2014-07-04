using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Repositories
{
    public class WorkProgramRepository : IWorkProgramRepository
    {
        #region Properties

        /// <summary>
        /// Unit of Work
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region IWorkProgramRepository Members       

        public int InsertEtsiWorkITem(EtsiWorkItemImport entry)
        {
            return 0;// UoW.Context.Transposition_CreateEtsiWorkItem("", null); 
        }

        public void InsertWIScheduleEntry(int WKI_ID, int SCHE_ID)
        {
            return; // UoW.Context.Transposition_CreateWiScheduleEntry(WKI_ID, SCHE_ID);
        }

        public void InsertWIKeyword(int WKI_ID, string kEYWORD_CODE)
        {
            UoW.Context.Transposition_CreateWiKeywordEntry(WKI_ID, kEYWORD_CODE);
        }

        public void InsertWIProject(int WKI_ID, int Project_ID)
        {
            UoW.Context.Transposition_CreateWiProjectEntry(WKI_ID, Project_ID);
        }

        public void InsertWIRemeark(int WKI_Id, int SeqNo, string remarkText)
        {
            UoW.Context.Transposition_CreateWiRemarkEntry(WKI_Id, SeqNo, remarkText);
        }

        #endregion
    }

    public interface IWorkProgramRepository
    {
        /// <summary>
        /// Unit of Work
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        int InsertEtsiWorkITem(EtsiWorkItemImport entry);

        void InsertWIScheduleEntry(int WKI_ID, int SCHE_ID);

        void InsertWIKeyword(int WKI_ID, string kEYWORD_CODE);

        void InsertWIProject(int WKI_ID, int Project_ID);

        void InsertWIRemeark(int WKI_Id, int SeqNo, string remarkText);
    }
}
