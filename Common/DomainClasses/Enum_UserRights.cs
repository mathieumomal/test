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

        Remarks_ViewPrivate
    }
}
