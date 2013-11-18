using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gimela.Net.Sockets")]
[assembly: AssemblyDescription("Gimela.Net.Sockets")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Gimela Technology Co., Ltd.")]
[assembly: AssemblyProduct("Gimela® Net Library")]
[assembly: AssemblyCopyright("Copyright © 2011-2012 Gimela Technologies Co., Ltd. All rights reserved.")]
[assembly: AssemblyTrademark("Gimela®")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c17d311a-d8b9-4f77-a30d-a6bb5ad9a6cc")]

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
[assembly: NeutralResourcesLanguageAttribute("")]

[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela", Scope = "namespace", Target = "Gimela.Net.Sockets")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gimela", Scope = "namespace", Target = "Gimela.Net.Sockets.Broadcast")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Net.Sockets.UdpDatagramReceivedEventArgs.#Datagram")]


[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpClient.#HandleDatagramReceived(System.IAsyncResult)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpClient.#HandleTcpServerConnected(System.IAsyncResult)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpClient.#Addresses")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpServer.#.ctor(System.Net.IPEndPoint)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpClient.#.ctor(System.Net.IPEndPoint)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpClient.#.ctor(System.Net.IPEndPoint,System.Net.IPEndPoint)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpServer.#HandleDatagramReceived(System.IAsyncResult)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Net.Sockets.TcpServerConnectedEventArgs.#Addresses")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Net.Sockets.TcpServerDisconnectedEventArgs.#Addresses")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Net.Sockets.TcpServerExceptionOccurredEventArgs.#Addresses")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Common.Logging.Logger.Debug(System.String)", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpClient.#HandleTcpServerConnected(System.IAsyncResult)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Net.Sockets.UdpPacket.#Header")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Gimela.Net.Sockets.UdpPacket.#Payload")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Gimela.Net.Sockets.AsyncTcpServer.#HandleDatagramWritten(System.IAsyncResult)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Gimela.Net.Sockets.TcpClientState.#NetworkStream")]
