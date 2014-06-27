using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.ServiceModel.Channels;

namespace CustomUserNameBinding
{
    /// <summary>
    /// Convert the value to the respect MessageVersion (Version of SOAP and WS-Addressing associated with a message and its exchange.)
    /// </summary>
    public class MessageVersionConverter : TypeConverter
    {
        #region Override Methods (TypeConverter)

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A System.Type that represents the type you want to convert from.</param>
        /// <returns>True if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((typeof(string) == sourceType) || base.CanConvertFrom(context, sourceType));
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A System.Type that represents the type you want to convert to.</param>
        /// <returns>True if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((typeof(InstanceDescriptor) == destinationType) || base.CanConvertTo(context, destinationType));
        }

        /// <summary>
        /// Converts the given value to the type of this converter.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">The System.Globalization.CultureInfo to use as the current culture.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }
            string str = (string)value;
            switch (str)
            {
                case "Soap11WSAddressing10":
                    return MessageVersion.Soap11WSAddressing10;

                case "Soap12WSAddressing10":
                    return MessageVersion.Soap12WSAddressing10;

                case "Soap11WSAddressingAugust2004":
                    return MessageVersion.Soap11WSAddressingAugust2004;

                case "Soap12WSAddressingAugust2004":
                    return MessageVersion.Soap12WSAddressingAugust2004;

                case "Soap11":
                    return MessageVersion.Soap11;

                case "Soap12":
                    return MessageVersion.Soap12;

                case "None":
                    return MessageVersion.None;

                case "Default":
                    return MessageVersion.Default;
            }
            throw new ArgumentOutOfRangeException("messageVersion", str, "The argument must be of type System.ServiceModel.Channels.MessageVersion");
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((typeof(string) != destinationType) || !(value is MessageVersion))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            MessageVersion version = (MessageVersion)value;
            if (version == MessageVersion.Default)
            {
                return "Default";
            }
            if (version == MessageVersion.Soap11WSAddressing10)
            {
                return "Soap11WSAddressing10";
            }
            if (version == MessageVersion.Soap12WSAddressing10)
            {
                return "Soap12WSAddressing10";
            }
            if (version == MessageVersion.Soap11WSAddressingAugust2004)
            {
                return "Soap11WSAddressingAugust2004";
            }
            if (version == MessageVersion.Soap12WSAddressingAugust2004)
            {
                return "Soap12WSAddressingAugust2004";
            }
            if (version == MessageVersion.Soap11)
            {
                return "Soap11";
            }
            if (version == MessageVersion.Soap12)
            {
                return "Soap12";
            }
            if (version != MessageVersion.None)
            {
                throw new ArgumentOutOfRangeException("messageVersion", value, "The argument must be of type System.ServiceModel.Channels.MessageVersion");
            }
            return "None";
        } 

        #endregion
    }
}
