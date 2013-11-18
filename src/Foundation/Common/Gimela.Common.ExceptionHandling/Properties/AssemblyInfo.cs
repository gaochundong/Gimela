using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gimela.Common.ExceptionHandling")]
[assembly: AssemblyDescription("Gimela.Common.ExceptionHandling")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Gimela Technology Co., Ltd.")]
[assembly: AssemblyProduct("Gimela® Common Library")]
[assembly: AssemblyCopyright("Copyright © 2011-2012 Gimela Technologies Co., Ltd. All rights reserved.")]
[assembly: AssemblyTrademark("Gimela®")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("b61f8f41-3f1c-4344-b05e-36f98bea98e0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela", Scope = "namespace", Target = "Gimela.Common.ExceptionHandling")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Gimela.Common.ExceptionHandling")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Gimela.Common.ExceptionHandling.ErrorHandler.#ProvideFault(System.Exception,System.ServiceModel.Channels.MessageVersion,System.ServiceModel.Channels.Message&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member", Target = "Gimela.Common.ExceptionHandling.ErrorHandlingBehaviorAttribute.#ApplyDispatchBehavior(System.ServiceModel.Description.ServiceDescription,System.ServiceModel.ServiceHostBase)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Scope = "type", Target = "Gimela.Common.ExceptionHandling.ExceptionToFaultMappingAttribute")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Scope = "member", Target = "Gimela.Common.ExceptionHandling.IExceptionToFaultConverter.#ConvertExceptionToFaultDetail(System.Exception)")]
