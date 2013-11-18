using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gimela.Data.Json")]
[assembly: AssemblyDescription("Gimela.Data.Json")]
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
[assembly: Guid("86ffe0c5-d34a-46b4-8309-b82b46b9a8f0")]

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

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Gimela.Data.Json.Bson")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "Gimela.Data.Json.JsonParser.#NextTokenCore()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Bson", Scope = "namespace", Target = "Gimela.Data.Json.Bson")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Bson", Scope = "type", Target = "Gimela.Data.Json.Bson.BsonConvert")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Bson", Scope = "type", Target = "Gimela.Data.Json.Bson.BsonSerializationException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela", Scope = "namespace", Target = "Gimela.Data.Json")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela", Scope = "namespace", Target = "Gimela.Data.Json.Bson")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Gimela.Data.Json.JsonDeserializer.#ParseProperties(System.Collections.Generic.Dictionary`2<System.String,System.Object>,System.Type)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Gimela.Data.Json.JsonSerializer.#Parse(System.Object)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Gimela.Data.Json.Bson.BsonSerializer.#WriteProperty(System.String,System.Object)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Gimela.Data.Json.JsonDeserializer.#CreateValueType(System.Object,System.Type)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Gimela.Data.Json.ObjectId.#ToString()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "Gimela.Data.Json.JsonDeserializer.#ParseProperties(System.Collections.Generic.Dictionary`2<System.String,System.Object>,System.Type)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "bson", Scope = "member", Target = "Gimela.Data.Json.Bson.BsonConvert.#DeserializeObject`1(System.Byte[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate", Scope = "member", Target = "Gimela.Data.Json.DateTimeHelper.#InitialJavaScriptDateTicks")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "t1", Scope = "member", Target = "Gimela.Data.Json.JsonDeserializer.#CreateStringDictionary(System.Collections.Generic.Dictionary`2<System.String,System.Object>,System.Type,System.Type[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Gimela.Data.Json.DateTimeHelper.#ConvertDateTimeToJavaScriptTicks(System.DateTime)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Gimela.Data.Json.DateTimeHelper.#ConvertDateTimeToJavaScriptTicks(System.DateTime,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Gimela.Data.Json.DateTimeHelper.#ToUniversalTicks(System.DateTime)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Gimela.Data.Json.JsonDeserializer.#CreateList(System.Collections.ArrayList,System.Type,System.Type)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyType", Scope = "member", Target = "Gimela.Data.Json.JsonDeserializer.#CreateArray(System.Collections.ArrayList,System.Type,System.Type)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Data.Json.ObjectId.#Value")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Gimela.Data.Json.Bson.BsonTypeMapping.#DeserializeTypeMapping")]
