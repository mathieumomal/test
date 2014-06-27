using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace CustomUserNameBinding
{
    /// <summary>
    /// Provide the configuration element that specify predefined bindings provided by Windows Communication Foundation (WCF).
    /// </summary>
    public class CustomUsernameBindingElement : StandardBindingElement
    {
        #region Variables

        private ConfigurationPropertyCollection properties;

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the version of SOAP and WS-Addressing associated with a message and its exchange.
        /// </summary>
        public MessageVersion MessageVersion
        {
            get
            {
                return (MessageVersion)base["messageVersion"];
            }
            set
            {
                base["messageVersion"] = value;
            }
        } 

        #endregion

        #region Override Methods (StandardBindingElement)

        /// <summary>
        /// Called when the content of a specified binding element is applied to this binding configuration element.
        /// </summary>
        /// <param name="binding">A binding</param>
        protected override void OnApplyConfiguration(Binding binding)
        {
            CustomUsernameBinding b = binding as CustomUsernameBinding;
            b.SetMessageVersion(MessageVersion);
        }

        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("messageVersion", typeof(MessageVersion), MessageVersion.Soap11, new MessageVersionConverter(), null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        /// <summary>
        /// Gets the System.Type object that represents the custom binding element.
        /// </summary>
        protected override Type BindingElementType
        {
            get { return typeof(CustomUsernameBinding); }
        } 

        #endregion
    }
}
