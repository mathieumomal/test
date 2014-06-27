using System.Net.Security;
using System.ServiceModel.Channels;

namespace CustomUserNameBinding
{
    /// <summary>
    /// Defines the contract for specifying the security capabilities for bindings.
    /// </summary>
    public class AutoSecuredHttpSecurityCapabilities : ISecurityCapabilities
    {
        #region ISecurityCapabilities Members

        /// <summary>
        /// Gets the protection level requests supported by the binding.
        /// </summary>
        public ProtectionLevel SupportedRequestProtectionLevel
        {
            get { return ProtectionLevel.EncryptAndSign; }
        }

        /// <summary>
        /// Gets the protection level responses supported by the binding.
        /// </summary>
        public ProtectionLevel SupportedResponseProtectionLevel
        {
            get { return ProtectionLevel.EncryptAndSign; }
        }

        /// <summary>
        /// Gets a value that indicates whether the binding supports client authentication.
        /// </summary>
        public bool SupportsClientAuthentication
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates whether the binding supports client Windows identity.
        /// </summary>
        public bool SupportsClientWindowsIdentity
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates whether the binding supports server authentication.
        /// </summary>
        public bool SupportsServerAuthentication
        {
            get { return true; }
        } 

        #endregion
    }
}
