﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Settings.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MIMDGP65MCiVh1EoxdmEA05zeikshfGiTXvXewhN3rm2eFGF0NS/G72RO9Blixr0C7AA38fIlVs0KurqK" +
            "c1J80q8N6p47L53UG5a3+ybTts=")]
        public string ConnectionString {
            get {
                return ((string)(this["ConnectionString"]));
            }
            set {
                this["ConnectionString"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AD_turniket@pwr")]
        public string LDAPUserName {
            get {
                return ((string)(this["LDAPUserName"]));
            }
            set {
                this["LDAPUserName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("uhh0joeX/12AzuPmHVrz5A==")]
        public string LDAPEncryptedPassword {
            get {
                return ((string)(this["LDAPEncryptedPassword"]));
            }
            set {
                this["LDAPEncryptedPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int MaxDBConnectionCount {
            get {
                return ((int)(this["MaxDBConnectionCount"]));
            }
            set {
                this["MaxDBConnectionCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\mcs.br\\adm\\server\\ActivityManager\\ActivityManager.exe")]
        public string ActivityManagerPath {
            get {
                return ((string)(this["ActivityManagerPath"]));
            }
            set {
                this["ActivityManagerPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("cp866")]
        public string ActivityManagerOutputCodePage {
            get {
                return ((string)(this["ActivityManagerOutputCodePage"]));
            }
            set {
                this["ActivityManagerOutputCodePage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\mcs.br\\adm\\server\\ActivityManager\\templates\\lifs")]
        public string ActivityManagerConfigsPath {
            get {
                return ((string)(this["ActivityManagerConfigsPath"]));
            }
            set {
                this["ActivityManagerConfigsPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int DataModelsCallbackUpdateTimeout {
            get {
                return ((int)(this["DataModelsCallbackUpdateTimeout"]));
            }
            set {
                this["DataModelsCallbackUpdateTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int CalcDataModelsUpdateTimeout {
            get {
                return ((int)(this["CalcDataModelsUpdateTimeout"]));
            }
            set {
                this["CalcDataModelsUpdateTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseLDAP {
            get {
                return ((bool)(this["UseLDAP"]));
            }
            set {
                this["UseLDAP"] = value;
            }
        }
    }
}
