﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaSys.Core.Features {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class jsFunction {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal jsFunction() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BaSys.Core.Features.jsFunction", typeof(jsFunction).Assembly);
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
        ///   Looks up a localized string similar to function dateDifference(startDate, endDate, kind) {
        ///    const diffInMilliseconds = endDate.getTime() - startDate.getTime();
        ///    switch (kind) {
        ///        case &apos;year&apos;:
        ///            return endDate.getFullYear() - startDate.getFullYear();
        ///        case &apos;month&apos;:
        ///            return (endDate.getFullYear() - startDate.getFullYear())
        ///                * 12 + (endDate.getMonth() - startDate.getMonth());
        ///        case &apos;quarter&apos;:
        ///            // eslint-disable-next-line no-case-declarations
        ///            const startQu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string dateDifference {
            get {
                return ResourceManager.GetString("dateDifference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Date.prototype.beginDay = function beginDay() {
        ///    return new Date(Date.UTC(this.getFullYear(), this.getMonth(), this.getDate()));
        ///};
        ///Date.prototype.endDay = function endDay() {
        ///    return new Date(Date.UTC(this.getFullYear(), this.getMonth(), this.getDate(), 23, 59, 59, 999));
        ///};
        ///Date.prototype.addDays = function addDays(days) {
        ///    const result = new Date(this);
        ///    result.setDate(result.getDate() + days);
        ///    return result;
        ///};
        ///Date.prototype.beginMonth = function beginMonth() {
        ///    return ne [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string dateExtensions {
            get {
                return ResourceManager.GetString("dateExtensions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function dateTimeNow() {
        ///    return new Date();
        ///}.
        /// </summary>
        internal static string dateTimeNow {
            get {
                return ResourceManager.GetString("dateTimeNow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function ifs(...args) {
        ///    for (let i = 0; i &lt; args.length; i += 2) {
        ///        const expressionResult = args[i];
        ///        if (expressionResult) {
        ///            if (i + 1 &lt; args.length) {
        ///                return args[i + 1];
        ///            }
        ///            return null;
        ///        }
        ///    }
        ///    return null;
        ///}.
        /// </summary>
        internal static string ifs {
            get {
                return ResourceManager.GetString("ifs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function iif(condition, valueTrue, valueFalse) {
        ///    if (condition) {
        ///        return valueTrue;
        ///    }
        ///    return valueFalse;
        ///}.
        /// </summary>
        internal static string iif {
            get {
                return ResourceManager.GetString("iif", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function isEmpty(value) {
        ///    if (value) {
        ///        return false;
        ///    }
        ///    return true;
        ///}.
        /// </summary>
        internal static string isEmpty {
            get {
                return ResourceManager.GetString("isEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function isNotEmpty(value) {
        ///    return !isEmpty(value);
        ///}.
        /// </summary>
        internal static string isNotEmpty {
            get {
                return ResourceManager.GetString("isNotEmpty", resourceCulture);
            }
        }
    }
}