using System.ServiceModel.Configuration;

namespace CustomUserNameBinding
{
    /// <summary>
    /// Provides the configuration sections that specify predefined bindings provided by Windows Communication Foundation (WCF).
    /// </summary>
    public class CustomUsernameCollectionElement : StandardBindingCollectionElement<CustomUsernameBinding, CustomUsernameBindingElement>
    {
    }
}
