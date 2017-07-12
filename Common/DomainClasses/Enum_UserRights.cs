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
        Specification_UnWithdrawFromRelease,
        Specification_InhibitPromote,
        Specification_Promote,
        Specification_Demote,
        Specification_RemoveInhibitPromote,
        Specification_BulkPromote,
        Specification_Delete,

        SpecificationRelease_Remove,
        SpecificationRelease_Remove_Disabled,

        Versions_Allocate,
        Versions_Upload,
        Versions_AllocateUpload_For_ReleaseClosed_Allowed,
        Versions_AddRemark,
        Versions_Modify_MajorVersion,
        Versions_Edit,
        Version_Draft_Delete,
        Versions_Avoid_Quality_Checks,

        Cr_Add_Crs_To_CrPacks,

        Contribution_Change_Type,
        Contribution_Change_Type_Limited,
        Contribution_DraftTsTrEnabledReleaseField,
        Contribution_DraftTsTr_CreationAndEdition,

        Release_Test_Page_Access
    }
}
