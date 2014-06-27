using System.ServiceModel.Channels;

namespace CustomUserNameBinding
{
    /// <summary>
    /// Defines a binding from CustomBinding.
    /// </summary>
    public class CustomUsernameBinding : CustomBinding
    {
        #region Variables

        private MessageVersion _messageVersion = MessageVersion.None; 

        #endregion

        #region Override Methods (CustomBinding)

        /// <summary>
        /// Returns a generic collection of the binding elements from the custom binding.
        /// </summary>
        /// <returns>An System.Collections.Generic.ICollection<T> object of type System.ServiceModel.Channels.BindingElement that contains the binding elements from the custom binding</returns>
        public override BindingElementCollection CreateBindingElements()
        {
            var bindingElementCollection = new BindingElementCollection();
            bindingElementCollection.Add(new TextMessageEncodingBindingElement() { MessageVersion = this._messageVersion });
            bindingElementCollection.Add(SecurityBindingElement.CreateUserNameOverTransportBindingElement());
            bindingElementCollection.Add(new AutoSecuredHttpTransportElement());
            return bindingElementCollection;
        }

        /// <summary>
        /// Gets the URI scheme for transport used by the custom binding.
        /// </summary>
        public override string Scheme
        {
            get
            {
                return "http";
            }
        } 

        #endregion

        #region Public Methods

        /// <summary>
        /// Set Message Version
        /// </summary>
        /// <param name="messageVersion">Specifies the version of SOAP and WS-Addressing associated with a message and its exchange.</param>
        public void SetMessageVersion(MessageVersion messageVersion)
        {
            this._messageVersion = messageVersion;
        }

        #endregion
    }
}
