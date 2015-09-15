using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    /// <summary>
    /// List all the rights that can be granted to a user.
    /// </summary>
    public enum Enum_UserRights
    {
        // Release related rights.
        Release_ViewCompleteList,
        Release_ViewPartialList,
        Release_ViewCompleteDetails,
        Release_ViewLimitedDetails,
        Release_ViewDetails,
        Release_Create,
        Release_Edit,
        Release_Freeze,
        Release_Close,

        WorkItem_ImportWorkplan,
        General_ViewPersonalData,

        Remarks_ViewPrivate,
        Remarks_AddPrivateByDefault,

        Specification_ViewDetails,
        Specification_Create,
        Specification_EditLimitted,
        Specification_EditFull,
        Specification_Withdraw,
        Specification_View_UnAllocated_Number,
        Specification_ManageITURecommendations,
        Specification_SyncHardLinksOnLatestFolder,
        Specification_ForceTransposition,
        Specification_UnforceTransposition,
        Specification_WithdrawFromRelease,
        Specification_InhibitPromote,
        Specification_Promote,
        Specification_RemoveInhibitPromote,
        Specification_BulkPromote,

        Versions_Allocate,
        Versions_Upload,
        Versions_AddRemark,
        Versions_Modify_MajorVersion,
        Versions_Edit,

        Cr_Add_Crs_To_CrPack
    }
}
