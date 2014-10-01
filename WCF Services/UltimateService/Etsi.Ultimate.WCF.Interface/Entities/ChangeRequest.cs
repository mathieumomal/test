﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// Change Request entity
    /// </summary>
    [DataContract]
    public class ChangeRequest
    {
        /// <summary>
        /// Gets or sets the PK_ change request.
        /// </summary>
        /// <value>
        /// The PK_ change request.
        /// </value>
        [DataMember]
        public int Pk_ChangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the cr number.
        /// </summary>
        /// <value>
        /// The cr number.
        /// </value>
        [DataMember]
        public string CRNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value>
        /// The revision.
        /// </value>
        [DataMember]
        public Nullable<int> Revision { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [DataMember]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the FK_ TSG status.
        /// </summary>
        /// <value>
        /// The FK_ TSG status.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_TSGStatus { get; set; }

        /// <summary>
        /// Gets or sets the FK_ wg status.
        /// </summary>
        /// <value>
        /// The FK_ wg status.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_WGStatus { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        [DataMember]
        public Nullable<DateTime> CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the TSG source organizations.
        /// </summary>
        /// <value>
        /// The TSG source organizations.
        /// </value>
        [DataMember]
        public string TSGSourceOrganizations { get; set; }

        /// <summary>
        /// Gets or sets the wg source organizations.
        /// </summary>
        /// <value>
        /// The wg source organizations.
        /// </value>
        [DataMember]
        public string WGSourceOrganizations { get; set; }

        /// <summary>
        /// Gets or sets the TSG meeting.
        /// </summary>
        /// <value>
        /// The TSG meeting.
        /// </value>
        [DataMember]
        public Nullable<int> TSGMeeting { get; set; }

        /// <summary>
        /// Gets or sets the TSG target.
        /// </summary>
        /// <value>
        /// The TSG target.
        /// </value>
        [DataMember]
        public Nullable<int> TSGTarget { get; set; }

        /// <summary>
        /// Gets or sets the wg source for TSG.
        /// </summary>
        /// <value>
        /// The wg source for TSG.
        /// </value>
        [DataMember]
        public Nullable<int> WGSourceForTSG { get; set; }

        /// <summary>
        /// Gets or sets the wg meeting.
        /// </summary>
        /// <value>
        /// The wg meeting.
        /// </value>
        [DataMember]
        public Nullable<int> WGMeeting { get; set; }

        /// <summary>
        /// Gets or sets the wg target.
        /// </summary>
        /// <value>
        /// The wg target.
        /// </value>
        [DataMember]
        public Nullable<int> WGTarget { get; set; }

        /// <summary>
        /// Gets or sets the FK_ enum_ cr category.
        /// </summary>
        /// <value>
        /// The FK_ enum_ cr category.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_Enum_CRCategory { get; set; }

        /// <summary>
        /// Gets or sets the FK_ specification.
        /// </summary>
        /// <value>
        /// The FK_ specification.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_Specification { get; set; }

        /// <summary>
        /// Gets or sets the FK_ release.
        /// </summary>
        /// <value>
        /// The FK_ release.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_Release { get; set; }

        /// <summary>
        /// Gets or sets the FK_ current version.
        /// </summary>
        /// <value>
        /// The FK_ current version.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets the FK_ new version.
        /// </summary>
        /// <value>
        /// The FK_ new version.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_NewVersion { get; set; }

        /// <summary>
        /// Gets or sets the FK_ impact.
        /// </summary>
        /// <value>
        /// The FK_ impact.
        /// </value>
        [DataMember]
        public Nullable<int> Fk_Impact { get; set; }

        /// <summary>
        /// Gets or sets the TSGT document.
        /// </summary>
        /// <value>
        /// The TSGT document.
        /// </value>
        [DataMember]
        public string TSGTDoc { get; set; }

        /// <summary>
        /// Gets or sets the WGT document.
        /// </summary>
        /// <value>
        /// The WGT document.
        /// </value>
        [DataMember]
        public string WGTDoc { get; set; }

        /// <summary>
        /// CR category
        /// </summary>
        [DataMember]        
        public ChangeRequestCategory Category { get; set; }
        
        /// <summary>
        /// Gets or sets the work item ids.
        /// </summary>
        /// <value>
        /// The work item ids.
        /// </value>
        [DataMember]
        public List<int> Fk_WorkItemIds { get; set; }
    }
}
