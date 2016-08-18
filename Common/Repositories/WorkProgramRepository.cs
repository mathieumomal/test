using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
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
            var outputparameter = new ObjectParameter("NEW_WKI_ID", typeof(int));

            UoW.Context.Transposition_CreateEtsiWorkItem(outputparameter, 
                entry.EtsiNumber, entry.StandardType, entry.EtsiDocNumber, entry.EtsiPartNumber,
                entry.Reference, entry.SerialNumber, entry.Version,
                entry.CommunityId, entry.TitlePart1, entry.TitlePart2,
                entry.TitlePart3, entry.RapporteurId, entry.SecretaryId, entry.WorkingTitle);

            return (int)outputparameter.Value;
        }

        public void InsertWIScheduleEntry(int WKI_ID, int MajVersion, int TechVersion, int EditVersion)
        {
            UoW.Context.Transposition_CreateWiScheduleEntries(WKI_ID, MajVersion, TechVersion, EditVersion);
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

        public void InsertWIMemo(int WKI_Id, string WKI_Scope)
        {
            UoW.Context.Transposition_CreateWiMemoEntry(WKI_Id, WKI_Scope);
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

        void InsertWIScheduleEntry(int WKI_ID, int MajVersion, int TechVersion, int EditVersion);

        void InsertWIKeyword(int WKI_ID, string kEYWORD_CODE);

        void InsertWIProject(int WKI_ID, int Project_ID);

        void InsertWIRemeark(int WKI_Id, int SeqNo, string remarkText);

        void InsertWIMemo(int WKI_Id, string WKI_Scope);
    }
}
