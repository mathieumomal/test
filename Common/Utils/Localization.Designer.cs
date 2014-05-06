﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.Utils {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Localization {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Localization() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Etsi.Ultimate.Utils.Localization", typeof(Localization).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Release closed. {0}.
        /// </summary>
        public static string History_Release_Close {
            get {
                return ResourceManager.GetString("History_Release_Close", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Release created..
        /// </summary>
        public static string History_Release_Created {
            get {
                return ResourceManager.GetString("History_Release_Created", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Release frozen. {0}.
        /// </summary>
        public static string History_Release_Freeze {
            get {
                return ResourceManager.GetString("History_Release_Freeze", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Release updated. Changes:&lt;br /&gt;{0}.
        /// </summary>
        public static string History_Release_Updated {
            get {
                return ResourceManager.GetString("History_Release_Updated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specification has been created for release {0}.
        /// </summary>
        public static string History_Specification_Created {
            get {
                return ResourceManager.GetString("History_Specification_Created", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid format for specification number. Should be “ab.xyz”. .
        /// </summary>
        public static string Specification_ERR002_Number_Invalid_Format {
            get {
                return ResourceManager.GetString("Specification_ERR002_Number_Invalid_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specification number already in use by “{0}”. .
        /// </summary>
        public static string Specification_ERR003_Number_Already_Use {
            get {
                return ResourceManager.GetString("Specification_ERR003_Number_Already_Use", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Acronym {2} is more than 50 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_Acronym_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_Acronym_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Workplan is not correctly formatted: {0}.
        /// </summary>
        public static string WorkItem_Import_Bad_Format {
            get {
                return ResourceManager.GetString("WorkItem_Import_Bad_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect zip file. Zip should contain only one csv file..
        /// </summary>
        public static string WorkItem_Import_Bad_Zip_File {
            get {
                return ResourceManager.GetString("WorkItem_Import_Bad_Zip_File", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Acronym &quot;{2}&quot; is duplicated on other work items with same level ({3}). Cannot proceed..
        /// </summary>
        public static string WorkItem_Import_DuplicateAcronymSameLevel {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateAcronymSameLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Acronym &quot;{2}&quot; is defined in the parent WI. Discarding this acronym..
        /// </summary>
        public static string WorkItem_Import_DuplicateAcronymSubLevel {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateAcronymSubLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UID {0} is defined for both WpId {1} and WpId {2}..
        /// </summary>
        public static string WorkItem_Import_DuplicateUID {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateUID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Workplan id {0} already exist. There might be conflicts when displaying..
        /// </summary>
        public static string WorkItem_Import_DuplicateWorkplanId {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateWorkplanId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error occured while analysing workplan. Please check that your login on the site has not expired, and that the settings of the modules are defined. If problem persists, please contact helpdesk..
        /// </summary>
        public static string WorkItem_Import_Error_Analysis {
            get {
                return ResourceManager.GetString("WorkItem_Import_Error_Analysis", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Imported file was not found at {0}. Please check the module configuration, and contact the helpdesk if necessary..
        /// </summary>
        public static string WorkItem_Import_FileNotFound {
            get {
                return ResourceManager.GetString("WorkItem_Import_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Completion is incorrect: {2}.
        /// </summary>
        public static string WorkItem_Import_Incorrect_Completion {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_Completion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Creation date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_CreationDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_CreationDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: End date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_EndDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_EndDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Last modified date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_LastModifiedDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_LastModifiedDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Start date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_StartDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_StartDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File format is invalid: file does not have CSV extension..
        /// </summary>
        public static string WorkItem_Import_Invalid_File_Format {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_File_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Workitem has level {2}, whereas parent has level {3}. Cannot process..
        /// </summary>
        public static string WorkItem_Import_Invalid_Hierarchy {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Hierarchy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Level has not been recognized: {2}. Defaulted to 0..
        /// </summary>
        public static string WorkItem_Import_Invalid_Level {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Level", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Could not retrieve PCG approval meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_PcgApprovedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_PcgApprovedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Could not retrieve PCG stopped meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_PcgStoppedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_PcgStoppedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Rapporteur email &quot;{2}&quot; could not match an existing user in database..
        /// </summary>
        public static string WorkItem_Import_Invalid_Rapporteur {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Rapporteur", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Cannot identify release: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_Release {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Release", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Resource format has not been recognized: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_Resource {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Resource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Status report format has not been recognized: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_StatusReport {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_StatusReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Could not retrieve TSG approval meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_TsgApprovedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_TsgApprovedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: Could not retrieve TSG stopped meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_TsgStoppedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_TsgStoppedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: WID format has not been recognized: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_WiD {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_WiD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;Name&quot; field is more than 255 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_Name_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_Name_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;PcgApprovalMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_PcgApprovalMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_PcgApprovalMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;PcgStoppedMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_PcgStoppedMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_PcgStoppedMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;RapporteurCompany&quot; field is more than 100 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_RapporteurCompany_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_RapporteurCompany_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;RapporteurStr&quot; field is more than 100 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_RapporteurStr_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_RapporteurStr_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;StatusReport&quot; field is more than 50 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_StatusReport_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_StatusReport_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;TsgApprovalMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_TsgApprovalMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_TsgApprovalMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;TsgStoppedMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_TsgStoppedMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_TsgStoppedMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;TSs and TRs&quot; field is more than 50 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_TsTr_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_TsTr_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unknown exception has occured. It has been logged by our error system. Please contact helpdesk for investigation. Uid of last successfully treated WI: {0}.
        /// </summary>
        public static string WorkItem_Import_Unknown_Exception {
            get {
                return ResourceManager.GetString("WorkItem_Import_Unknown_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Uid {1}: &quot;Wid&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_Wid_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_Wid_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WorkplanId {0} comes after WorkplanId {1}. Possible messing in the Work items..
        /// </summary>
        public static string WorkItem_Import_Wrong_Order {
            get {
                return ResourceManager.GetString("WorkItem_Import_Wrong_Order", resourceCulture);
            }
        }
    }
}
