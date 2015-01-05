﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.18063
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.Utils {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
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
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
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
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
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
        ///   Recherche une chaîne localisée semblable à Release and specification must be provided.
        /// </summary>
        public static string Allocate_Error_Missing_Release_Or_Specification {
            get {
                return ResourceManager.GetString("Allocate_Error_Missing_Release_Or_Specification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification release does not exist..
        /// </summary>
        public static string Allocate_Error_SpecRelease_Does_Not_Exist {
            get {
                return ResourceManager.GetString("Allocate_Error_SpecRelease_Does_Not_Exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Version should be greater that latest uploaded version for the release..
        /// </summary>
        public static string Allocate_Error_Version_Not_Allowed {
            get {
                return ResourceManager.GetString("Allocate_Error_Version_Not_Allowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à ChangeRequest {0}, revision {1} already exists for indicated specification..
        /// </summary>
        public static string ChangeRequest_Create_AlreadyExists {
            get {
                return ResourceManager.GetString("ChangeRequest_Create_AlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Release does not exist.
        /// </summary>
        public static string Error_Release_Does_Not_Exist {
            get {
                return ResourceManager.GetString("Error_Release_Does_Not_Exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification does not exist.
        /// </summary>
        public static string Error_Spec_Does_Not_Exist {
            get {
                return ResourceManager.GetString("Error_Spec_Does_Not_Exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Could not allocate version for Specification {0}: specification is not under change control.
        /// </summary>
        public static string FinalizeCrs_Warn_DraftSpec {
            get {
                return ResourceManager.GetString("FinalizeCrs_Warn_DraftSpec", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Could not allocate version for Specification {0}: Release {1} is not defined for this specification..
        /// </summary>
        public static string FinalizeCrs_Warn_SpecReleaseNotExisting {
            get {
                return ResourceManager.GetString("FinalizeCrs_Warn_SpecReleaseNotExisting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Could not allocate version for Specification {0}: Release {1} is withdrawn for this specification..
        /// </summary>
        public static string FinalizeCrs_Warn_SpecReleaseWithdrawn {
            get {
                return ResourceManager.GetString("FinalizeCrs_Warn_SpecReleaseWithdrawn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Could not allocate version for Specification {0}: specification is withdrawn.
        /// </summary>
        public static string FinalizeCrs_Warn_WithDrawnSpec {
            get {
                return ResourceManager.GetString("FinalizeCrs_Warn_WithDrawnSpec", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An error has occured. Please try again. If problem persists, please contact IT support..
        /// </summary>
        public static string GenericError {
            get {
                return ResourceManager.GetString("GenericError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Release closed. {0}.
        /// </summary>
        public static string History_Release_Close {
            get {
                return ResourceManager.GetString("History_Release_Close", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Release created..
        /// </summary>
        public static string History_Release_Created {
            get {
                return ResourceManager.GetString("History_Release_Created", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Release frozen. {0}.
        /// </summary>
        public static string History_Release_Freeze {
            get {
                return ResourceManager.GetString("History_Release_Freeze", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Release updated. Changes:&lt;br /&gt;{0}.
        /// </summary>
        public static string History_Release_Updated {
            get {
                return ResourceManager.GetString("History_Release_Updated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Prime responsible group has been changed to {0}. Previous: {1} .
        /// </summary>
        public static string History_Specification_Changed_Prime_Group {
            get {
                return ResourceManager.GetString("History_Specification_Changed_Prime_Group", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Prime Rapporteur has been changed to {0}. Previous: {1}.
        /// </summary>
        public static string History_Specification_Changed_Prime_Rapporteur {
            get {
                return ResourceManager.GetString("History_Specification_Changed_Prime_Rapporteur", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification has been created for release {0}.
        /// </summary>
        public static string History_Specification_Created {
            get {
                return ResourceManager.GetString("History_Specification_Created", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification has been made Under Change Control.
        /// </summary>
        public static string History_Specification_Status_Changed_UnderChangeControl {
            get {
                return ResourceManager.GetString("History_Specification_Status_Changed_UnderChangeControl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à End release does not exist..
        /// </summary>
        public static string ItuConversion_Error_InvalidEndRelease {
            get {
                return ResourceManager.GetString("ItuConversion_Error_InvalidEndRelease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à SA Plenary Meeting id is invalid..
        /// </summary>
        public static string ItuConversion_Error_InvalidMeetingId {
            get {
                return ResourceManager.GetString("ItuConversion_Error_InvalidMeetingId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Start release is greater than End release..
        /// </summary>
        public static string ItuConversion_Error_InvalidReleaseOrder {
            get {
                return ResourceManager.GetString("ItuConversion_Error_InvalidReleaseOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Start release does not exist..
        /// </summary>
        public static string ItuConversion_Error_InvalidStartRelease {
            get {
                return ResourceManager.GetString("ItuConversion_Error_InvalidStartRelease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à You do not have right to perform this action.
        /// </summary>
        public static string RightError {
            get {
                return ResourceManager.GetString("RightError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification number allocation required: {0}.
        /// </summary>
        public static string Specification_AwaitingReferenceNumberMail_Subject {
            get {
                return ResourceManager.GetString("Specification_AwaitingReferenceNumberMail_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification has been created, but system failed to notify the Specification Manager(s). Please contact them directly to request them to set the number..
        /// </summary>
        public static string Specification_ERR001_FailedToSendEmailToSpecManagers {
            get {
                return ResourceManager.GetString("Specification_ERR001_FailedToSendEmailToSpecManagers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid format for specification number. .
        /// </summary>
        public static string Specification_ERR002_Number_Invalid_Format {
            get {
                return ResourceManager.GetString("Specification_ERR002_Number_Invalid_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification number already in use by “{0}”. .
        /// </summary>
        public static string Specification_ERR003_Number_Already_Use {
            get {
                return ResourceManager.GetString("Specification_ERR003_Number_Already_Use", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à System failed to notify the Secretary and Workplan Manager. Please contact them directly to notify them of the reference assignment..
        /// </summary>
        public static string Specification_ERR101_FailedToSendEmailToSecretaryAndWorkplanManager {
            get {
                return ResourceManager.GetString("Specification_ERR101_FailedToSendEmailToSecretaryAndWorkplanManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification has been successfully created. An email has been sent to the Specification Manager(s) requesting the allocation of a specification number..
        /// </summary>
        public static string Specification_MSG002_SpecCreatedMailSendToSpecManager {
            get {
                return ResourceManager.GetString("Specification_MSG002_SpecCreatedMailSendToSpecManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Specification number allocated: {0}.
        /// </summary>
        public static string Specification_ReferenceNumberAssigned_Subject {
            get {
                return ResourceManager.GetString("Specification_ReferenceNumberAssigned_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Draft versions can not have Major number higher than 2..
        /// </summary>
        public static string Upload_Version_Error_Draft_Major_Too_High {
            get {
                return ResourceManager.GetString("Upload_Version_Error_Draft_Major_Too_High", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à You are not allowed to upload a lower version than an already uploaded one, except if it has been allocated..
        /// </summary>
        public static string Upload_Version_Error_Previous_Version {
            get {
                return ResourceManager.GetString("Upload_Version_Error_Previous_Version", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Version {0} already exists..
        /// </summary>
        public static string Upload_Version_Error_Version_Already_Exists {
            get {
                return ResourceManager.GetString("Upload_Version_Error_Version_Already_Exists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Version not found..
        /// </summary>
        public static string Version_Not_Found {
            get {
                return ResourceManager.GetString("Version_Not_Found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Acronym {2} is more than 50 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_Acronym_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_Acronym_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Workplan is not correctly formatted: {0}.
        /// </summary>
        public static string WorkItem_Import_Bad_Format {
            get {
                return ResourceManager.GetString("WorkItem_Import_Bad_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Incorrect zip file. Zip should contain only one csv file..
        /// </summary>
        public static string WorkItem_Import_Bad_Zip_File {
            get {
                return ResourceManager.GetString("WorkItem_Import_Bad_Zip_File", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Acronym &quot;{2}&quot; is duplicated on other work items with same level ({3}). Cannot proceed..
        /// </summary>
        public static string WorkItem_Import_DuplicateAcronymSameLevel {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateAcronymSameLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Acronym &quot;{2}&quot; is defined in the parent WI. Discarding this acronym..
        /// </summary>
        public static string WorkItem_Import_DuplicateAcronymSubLevel {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateAcronymSubLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à UID {0} is defined for both WpId {1} and WpId {2}..
        /// </summary>
        public static string WorkItem_Import_DuplicateUID {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateUID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Workplan id {0} already exist. There might be conflicts when displaying..
        /// </summary>
        public static string WorkItem_Import_DuplicateWorkplanId {
            get {
                return ResourceManager.GetString("WorkItem_Import_DuplicateWorkplanId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Error occured while analysing workplan. Please check that your login on the site has not expired, and that the settings of the modules are defined. If problem persists, please contact helpdesk..
        /// </summary>
        public static string WorkItem_Import_Error_Analysis {
            get {
                return ResourceManager.GetString("WorkItem_Import_Error_Analysis", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Imported file was not found at {0}. Please check the module configuration, and contact the helpdesk if necessary..
        /// </summary>
        public static string WorkItem_Import_FileNotFound {
            get {
                return ResourceManager.GetString("WorkItem_Import_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Completion is incorrect: {2}.
        /// </summary>
        public static string WorkItem_Import_Incorrect_Completion {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_Completion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Creation date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_CreationDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_CreationDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: End date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_EndDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_EndDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Last modified date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_LastModifiedDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_LastModifiedDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Start date is incorrect: {2}. Awaited format: DD/MM/YYYY.
        /// </summary>
        public static string WorkItem_Import_Incorrect_StartDate {
            get {
                return ResourceManager.GetString("WorkItem_Import_Incorrect_StartDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à File format is invalid: file does not have CSV extension..
        /// </summary>
        public static string WorkItem_Import_Invalid_File_Format {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_File_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Workitem has level {2}, whereas parent has level {3}. Cannot process..
        /// </summary>
        public static string WorkItem_Import_Invalid_Hierarchy {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Hierarchy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Level has not been recognized: {2}. Defaulted to 0..
        /// </summary>
        public static string WorkItem_Import_Invalid_Level {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Level", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Could not retrieve PCG approval meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_PcgApprovedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_PcgApprovedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Could not retrieve PCG stopped meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_PcgStoppedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_PcgStoppedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Rapporteur email &quot;{2}&quot; could not match an existing user in database..
        /// </summary>
        public static string WorkItem_Import_Invalid_Rapporteur {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Rapporteur", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Cannot identify release: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_Release {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Release", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Has different release from his parent. .
        /// </summary>
        public static string WorkItem_Import_Invalid_Release_From_Parent {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Release_From_Parent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Resource format has not been recognized: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_Resource {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_Resource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Status report format has not been recognized: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_StatusReport {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_StatusReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Could not retrieve TSG approval meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_TsgApprovedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_TsgApprovedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: Could not retrieve TSG stopped meeting {2} in database.
        /// </summary>
        public static string WorkItem_Import_Invalid_TsgStoppedMtg {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_TsgStoppedMtg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: WID format has not been recognized: {2}.
        /// </summary>
        public static string WorkItem_Import_Invalid_WiD {
            get {
                return ResourceManager.GetString("WorkItem_Import_Invalid_WiD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;Name&quot; field is more than 255 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_Name_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_Name_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: WI has different Release ({2}) from its parent ({3})..
        /// </summary>
        public static string WorkItem_Import_Parent_Release_Different_With_Child {
            get {
                return ResourceManager.GetString("WorkItem_Import_Parent_Release_Different_With_Child", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;PcgApprovalMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_PcgApprovalMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_PcgApprovalMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;PcgStoppedMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_PcgStoppedMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_PcgStoppedMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;RapporteurCompany&quot; field is more than 100 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_RapporteurCompany_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_RapporteurCompany_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;RapporteurStr&quot; field is more than 100 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_RapporteurStr_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_RapporteurStr_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;StatusReport&quot; field is more than 50 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_StatusReport_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_StatusReport_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;TsgApprovalMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_TsgApprovalMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_TsgApprovalMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;TsgStoppedMtgRef&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_TsgStoppedMtgRef_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_TsgStoppedMtgRef_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;TSs and TRs&quot; field is more than 50 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_TsTr_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_TsTr_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à An unknown exception has occured. It has been logged by our error system. Please contact helpdesk for investigation. Uid of last successfully treated WI: {0}.
        /// </summary>
        public static string WorkItem_Import_Unknown_Exception {
            get {
                return ResourceManager.GetString("WorkItem_Import_Unknown_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0}, Uid {1}: &quot;Wid&quot; field is more than 20 characters. It has been truncated..
        /// </summary>
        public static string WorkItem_Import_Wid_Too_Long {
            get {
                return ResourceManager.GetString("WorkItem_Import_Wid_Too_Long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à WorkplanId {0} comes after WorkplanId {1}. Possible messing in the Work items..
        /// </summary>
        public static string WorkItem_Import_Wrong_Order {
            get {
                return ResourceManager.GetString("WorkItem_Import_Wrong_Order", resourceCulture);
            }
        }
    }
}
