﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PasswordKeeper.Apps.Wpf.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PasswordKeeper.Apps.Wpf.Properties.Errors", typeof(Errors).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account name cannot be empty.
        /// </summary>
        internal static string AccountNameEmpty {
            get {
                return ResourceManager.GetString("AccountNameEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database file of this user has invalid format.
        /// </summary>
        internal static string InvalidDb {
            get {
                return ResourceManager.GetString("InvalidDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password is wrong or database file is corrupted.
        /// </summary>
        internal static string InvalidPassword {
            get {
                return ResourceManager.GetString("InvalidPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Login cannot be empty.
        /// </summary>
        internal static string LoginEmpty {
            get {
                return ResourceManager.GetString("LoginEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Login can contain only alphanumeric characters.
        /// </summary>
        internal static string LoginFormatInvalid {
            get {
                return ResourceManager.GetString("LoginFormatInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password cannot be empty.
        /// </summary>
        internal static string PasswordEmpty {
            get {
                return ResourceManager.GetString("PasswordEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User with such login already exists.
        /// </summary>
        internal static string UserExists {
            get {
                return ResourceManager.GetString("UserExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User already logged.
        /// </summary>
        internal static string UserLogged {
            get {
                return ResourceManager.GetString("UserLogged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User was not found.
        /// </summary>
        internal static string UserNotFound {
            get {
                return ResourceManager.GetString("UserNotFound", resourceCulture);
            }
        }
    }
}
