﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.Utils.WcfMailService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Mail", Namespace="http://schemas.datacontract.org/2004/07/Capgemini.Etsi.Ngpp.Business")]
    [System.SerializableAttribute()]
    public partial class Mail : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string bodyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ccAddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string fromAddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string smtpField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string subjectField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string toAddressField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string body {
            get {
                return this.bodyField;
            }
            set {
                if ((object.ReferenceEquals(this.bodyField, value) != true)) {
                    this.bodyField = value;
                    this.RaisePropertyChanged("body");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ccAddress {
            get {
                return this.ccAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.ccAddressField, value) != true)) {
                    this.ccAddressField = value;
                    this.RaisePropertyChanged("ccAddress");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string fromAddress {
            get {
                return this.fromAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.fromAddressField, value) != true)) {
                    this.fromAddressField = value;
                    this.RaisePropertyChanged("fromAddress");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string smtp {
            get {
                return this.smtpField;
            }
            set {
                if ((object.ReferenceEquals(this.smtpField, value) != true)) {
                    this.smtpField = value;
                    this.RaisePropertyChanged("smtp");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string subject {
            get {
                return this.subjectField;
            }
            set {
                if ((object.ReferenceEquals(this.subjectField, value) != true)) {
                    this.subjectField = value;
                    this.RaisePropertyChanged("subject");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string toAddress {
            get {
                return this.toAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.toAddressField, value) != true)) {
                    this.toAddressField = value;
                    this.RaisePropertyChanged("toAddress");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CodePageDataItem", Namespace="http://schemas.datacontract.org/2004/07/System.Globalization")]
    [System.SerializableAttribute()]
    public partial class CodePageDataItem : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string m_bodyNameField;
        
        private int m_dataIndexField;
        
        private uint m_flagsField;
        
        private string m_headerNameField;
        
        private int m_uiFamilyCodePageField;
        
        private string m_webNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string m_bodyName {
            get {
                return this.m_bodyNameField;
            }
            set {
                if ((object.ReferenceEquals(this.m_bodyNameField, value) != true)) {
                    this.m_bodyNameField = value;
                    this.RaisePropertyChanged("m_bodyName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int m_dataIndex {
            get {
                return this.m_dataIndexField;
            }
            set {
                if ((this.m_dataIndexField.Equals(value) != true)) {
                    this.m_dataIndexField = value;
                    this.RaisePropertyChanged("m_dataIndex");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public uint m_flags {
            get {
                return this.m_flagsField;
            }
            set {
                if ((this.m_flagsField.Equals(value) != true)) {
                    this.m_flagsField = value;
                    this.RaisePropertyChanged("m_flags");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string m_headerName {
            get {
                return this.m_headerNameField;
            }
            set {
                if ((object.ReferenceEquals(this.m_headerNameField, value) != true)) {
                    this.m_headerNameField = value;
                    this.RaisePropertyChanged("m_headerName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int m_uiFamilyCodePage {
            get {
                return this.m_uiFamilyCodePageField;
            }
            set {
                if ((this.m_uiFamilyCodePageField.Equals(value) != true)) {
                    this.m_uiFamilyCodePageField = value;
                    this.RaisePropertyChanged("m_uiFamilyCodePage");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string m_webName {
            get {
                return this.m_webNameField;
            }
            set {
                if ((object.ReferenceEquals(this.m_webNameField, value) != true)) {
                    this.m_webNameField = value;
                    this.RaisePropertyChanged("m_webName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WcfMailService.ISendMail")]
    public interface ISendMail {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendEmail", ReplyAction="http://tempuri.org/ISendMail/SendEmailResponse")]
        bool SendEmail(string fromAddress, string toAddress, string ccAddress, string subject, string body, string smtpHost);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendEmail", ReplyAction="http://tempuri.org/ISendMail/SendEmailResponse")]
        System.Threading.Tasks.Task<bool> SendEmailAsync(string fromAddress, string toAddress, string ccAddress, string subject, string body, string smtpHost);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendAnEmail", ReplyAction="http://tempuri.org/ISendMail/SendAnEmailResponse")]
        bool SendAnEmail(Etsi.Ultimate.Utils.WcfMailService.Mail m);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendAnEmail", ReplyAction="http://tempuri.org/ISendMail/SendAnEmailResponse")]
        System.Threading.Tasks.Task<bool> SendAnEmailAsync(Etsi.Ultimate.Utils.WcfMailService.Mail m);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendEmailWithBcc", ReplyAction="http://tempuri.org/ISendMail/SendEmailWithBccResponse")]
        bool SendEmailWithBcc(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, string subject, string body, string smtpHost);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendEmailWithBcc", ReplyAction="http://tempuri.org/ISendMail/SendEmailWithBccResponse")]
        System.Threading.Tasks.Task<bool> SendEmailWithBccAsync(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, string subject, string body, string smtpHost);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendEmailComplete", ReplyAction="http://tempuri.org/ISendMail/SendEmailCompleteResponse")]
        bool SendEmailComplete(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, System.Collections.Generic.List<System.Net.Mail.Attachment> attachments, string subject, string body, string smtpHost);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendMail/SendEmailComplete", ReplyAction="http://tempuri.org/ISendMail/SendEmailCompleteResponse")]
        System.Threading.Tasks.Task<bool> SendEmailCompleteAsync(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, System.Collections.Generic.List<System.Net.Mail.Attachment> attachments, string subject, string body, string smtpHost);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISendMailChannel : Etsi.Ultimate.Utils.WcfMailService.ISendMail, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SendMailClient : System.ServiceModel.ClientBase<Etsi.Ultimate.Utils.WcfMailService.ISendMail>, Etsi.Ultimate.Utils.WcfMailService.ISendMail {
        
        public SendMailClient() {
        }
        
        public SendMailClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SendMailClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SendMailClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SendMailClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool SendEmail(string fromAddress, string toAddress, string ccAddress, string subject, string body, string smtpHost) {
            return base.Channel.SendEmail(fromAddress, toAddress, ccAddress, subject, body, smtpHost);
        }
        
        public System.Threading.Tasks.Task<bool> SendEmailAsync(string fromAddress, string toAddress, string ccAddress, string subject, string body, string smtpHost) {
            return base.Channel.SendEmailAsync(fromAddress, toAddress, ccAddress, subject, body, smtpHost);
        }
        
        public bool SendAnEmail(Etsi.Ultimate.Utils.WcfMailService.Mail m) {
            return base.Channel.SendAnEmail(m);
        }
        
        public System.Threading.Tasks.Task<bool> SendAnEmailAsync(Etsi.Ultimate.Utils.WcfMailService.Mail m) {
            return base.Channel.SendAnEmailAsync(m);
        }
        
        public bool SendEmailWithBcc(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, string subject, string body, string smtpHost) {
            return base.Channel.SendEmailWithBcc(fromAddress, toAddresses, ccAddresses, bccAddresses, subject, body, smtpHost);
        }
        
        public System.Threading.Tasks.Task<bool> SendEmailWithBccAsync(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, string subject, string body, string smtpHost) {
            return base.Channel.SendEmailWithBccAsync(fromAddress, toAddresses, ccAddresses, bccAddresses, subject, body, smtpHost);
        }
        
        public bool SendEmailComplete(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, System.Collections.Generic.List<System.Net.Mail.Attachment> attachments, string subject, string body, string smtpHost) {
            return base.Channel.SendEmailComplete(fromAddress, toAddresses, ccAddresses, bccAddresses, attachments, subject, body, smtpHost);
        }
        
        public System.Threading.Tasks.Task<bool> SendEmailCompleteAsync(string fromAddress, System.Collections.Generic.List<string> toAddresses, System.Collections.Generic.List<string> ccAddresses, System.Collections.Generic.List<string> bccAddresses, System.Collections.Generic.List<System.Net.Mail.Attachment> attachments, string subject, string body, string smtpHost) {
            return base.Channel.SendEmailCompleteAsync(fromAddress, toAddresses, ccAddresses, bccAddresses, attachments, subject, body, smtpHost);
        }
    }
}