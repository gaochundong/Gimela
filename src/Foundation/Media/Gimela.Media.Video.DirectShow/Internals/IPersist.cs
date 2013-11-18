using System;
using System.Runtime.InteropServices;

namespace Gimela.Media.Video.DirectShow.Internals
{
  /// <summary>
  /// Provides the CLSID of an object that can be stored persistently in the system. Allows the object to specify which object 
  /// handler to use in the client process, as it is used in the default implementation of marshaling.
  /// </summary>
  [ComImport,
  Guid("0000010c-0000-0000-C000-000000000046"),
  InterfaceType(ComInterfaceType.InterfaceIsDual)]
  internal interface IPersist
  {
    /// <summary>
    /// Retrieves the class identifier (CLSID) of the object.
    /// </summary>
    /// <param name="pClassID"></param>
    /// <returns></returns>
    [PreserveSig]
    int GetClassID([Out] out Guid pClassID);
  }
}
