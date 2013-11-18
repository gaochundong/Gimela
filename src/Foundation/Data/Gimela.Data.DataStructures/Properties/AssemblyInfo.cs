using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gimela.Data.DataStructures")]
[assembly: AssemblyDescription("Gimela.Data.DataStructures")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Gimela Technology Co., Ltd.")]
[assembly: AssemblyProduct("Gimela® Data Library")]
[assembly: AssemblyCopyright("Copyright © 2011-2012 Gimela Technologies Co., Ltd. All rights reserved.")]
[assembly: AssemblyTrademark("Gimela®")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e6918bc3-a3d7-446e-81bc-3aff3a0894b7")]

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
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela", Scope = "namespace", Target = "Gimela.Data.DataStructures")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Data.DataStructures.IByteTree.#Item[System.String]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Scope = "member", Target = "Gimela.Data.DataStructures.IIndexTree.#Get(System.String,System.Object)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set", Scope = "member", Target = "Gimela.Data.DataStructures.IIndexTree.#Set(System.String,System.Object)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTree.#Item[System.String]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#Insert(System.String,System.Int64,System.String&,Gimela.Data.DataStructures.BPlusTreeNode&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeLong.#Item[System.String]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeBytes.#Create(System.String,System.String,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeBytes.#Create(System.String,System.String,System.Int32,System.Int32,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeBytes.#Open(System.String,System.String,System.IO.FileAccess)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeBytes.#Item[System.String]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#MergeLeaves(Gimela.Data.DataStructures.BPlusTreeNode,Gimela.Data.DataStructures.BPlusTreeNode,System.Boolean&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#Merge(Gimela.Data.DataStructures.BPlusTreeNode,System.String,Gimela.Data.DataStructures.BPlusTreeNode,System.String&,System.Boolean&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#Merge(Gimela.Data.DataStructures.BPlusTreeNode,System.String,Gimela.Data.DataStructures.BPlusTreeNode,System.String&,System.Boolean&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#InsertLeaf(System.String,System.Int64,System.String&,Gimela.Data.DataStructures.BPlusTreeNode&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#InsertLeaf(System.String,System.Int64,System.String&,Gimela.Data.DataStructures.BPlusTreeNode&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#Insert(System.String,System.Int64,System.String&,Gimela.Data.DataStructures.BPlusTreeNode&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#Insert(System.String,System.Int64,System.String&,Gimela.Data.DataStructures.BPlusTreeNode&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#FindKey(System.String,System.Int64&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#DeleteLeaf(System.String,System.Boolean&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Scope = "member", Target = "Gimela.Data.DataStructures.BPlusTreeNode.#Delete(System.String,System.Boolean&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Scope = "member", Target = "Gimela.Data.DataStructures.IIndexTree.#Get(System.String)")]
