using System.ServiceModel.Channels;
using System.Xml;

namespace CustomUserNameBinding
{
    /// <summary>
    /// Represents the binding element used to specify an HTTP transport for transmitting messages.
    /// </summary>
    public class AutoSecuredHttpTransportElement : HttpTransportBindingElement, ITransportTokenAssertionProvider
    {
        #region Override Methods (HttpTransportBindingElement)

        /// <summary>
        /// Gets a property from the specified System.ServiceModel.Channels.BindingContext.
        /// </summary>
        /// <typeparam name="T">The type of the property to get</typeparam>
        /// <param name="context">A System.ServiceModel.Channels.BindingContext</param>
        /// <returns>The property from the specified System.ServiceModel.Channels.BindingContext</returns>
        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
                return (T)(ISecurityCapabilities)new AutoSecuredHttpSecurityCapabilities();

            return base.GetProperty<T>(context);
        } 

        #endregion

        #region ITransportTokenAssertionProvider Members

        /// <summary>
        /// Gets a transport token assertion.
        /// </summary>
        /// <returns>An System.Xml.XmlElement that represents a transport token assertion.</returns>
        public XmlElement GetTransportTokenAssertion()
        {
            return null;
        } 

        #endregion
    }
}
