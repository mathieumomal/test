﻿using System;
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
        Specification_ForceUnforceTransposition,
    }
}
