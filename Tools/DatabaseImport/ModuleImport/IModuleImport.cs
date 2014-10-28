using Etsi.Ngppdb.DataAccess;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImport.ModuleImport
{
    public interface IModuleImport
    {
        IUltimateContext UltimateContext { get; set; }
        INGPPDBContext NgppdbContext { get; set; }

        MeetingHelper MtgHelper { get; set; }

        ITmpDb LegacyContext { get; set; }

        void CleanDatabase();

        void FillDatabase();
    }
}
