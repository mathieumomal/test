﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.Module.Specifications.App_LocalResources {
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
    public class SpecificationDetails_aspx {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SpecificationDetails_aspx() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Etsi.Ultimate.Module.Specifications.App_LocalResources.SpecificationDetails.aspx", typeof(SpecificationDetails_aspx).Assembly);
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
        ///   Looks up a localized string similar to System failed to notify the Secretary and Workplan Manager. Please contact them directly to notify them of the reference assignment..
        /// </summary>
        public static string Error_NumberAssigned_NotifyMCC_NoEmail {
            get {
                return ResourceManager.GetString("Error_NumberAssigned_NotifyMCC_NoEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specification has been created, but system failed to notify the Specification Manager(s). Please contact them directly to request them to set the number..
        /// </summary>
        public static string Error_NumberNeeded_NotifySpecMgr_NoEmail {
            get {
                return ResourceManager.GetString("Error_NumberNeeded_NotifySpecMgr_NoEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specification has been successfully created. An email has been sent to the Specification Manager(s) requesting the allocation of a specification number..
        /// </summary>
        public static string Warning_NumberNeeded_NotifySpec_Mgr {
            get {
                return ResourceManager.GetString("Warning_NumberNeeded_NotifySpec_Mgr", resourceCulture);
            }
        }
    }
}