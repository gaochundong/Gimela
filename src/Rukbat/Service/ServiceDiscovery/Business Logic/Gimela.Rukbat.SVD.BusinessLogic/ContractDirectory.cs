using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gimela.ServiceModel.ManagedService.Attributes;
using Gimela.Rukbat.SVD.BusinessEntities;

namespace Gimela.Rukbat.SVD.BusinessLogic
{
  public static class ContractDirectory
  {
    private static readonly string assemblyFilePattern = "*.Contracts.dll";

    public static ReadOnlyCollection<ContractInfo> Scan(string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException("path");
      if (!Directory.Exists(path))
        throw new DirectoryNotFoundException(path);

      return ScanDirectory(new DirectoryInfo(path));
    }

    private static ReadOnlyCollection<ContractInfo> ScanDirectory(DirectoryInfo directory)
    {
      List<ContractInfo> contracts = new List<ContractInfo>();

      List<FileInfo> fileList = new List<FileInfo>(directory.GetFiles(assemblyFilePattern));
      foreach (var file in fileList)
      {
        foreach (var info in ScanFile(file))
        {
          if (!contracts.Contains(info))
          {
            contracts.Add(info);
          }
        }
      }

      DirectoryInfo[] subDirectories = directory.GetDirectories();
      foreach (var dir in subDirectories)
      {
        foreach (var info in ScanDirectory(dir))
        {
          if (!contracts.Contains(info))
          {
            contracts.Add(info);
          }
        }
      }

      return contracts.AsReadOnly();
    }

    private static ReadOnlyCollection<ContractInfo> ScanFile(FileInfo file)
    {
      List<ContractInfo> contracts = new List<ContractInfo>();

      Assembly assembly = Assembly.LoadFrom(file.FullName);
      if (assembly != null)
      {
        foreach (var info in ScanAssembly(assembly))
        {
          if (!contracts.Contains(info))
          {
            contracts.Add(info);
          }
        }
      }

      return contracts.AsReadOnly();         
    }

    private static ReadOnlyCollection<ContractInfo> ScanAssembly(Assembly assembly)
    {
      List<ContractInfo> contracts = new List<ContractInfo>();

      ReadOnlyCollection<CustomAttributeData> attributes = CustomAttributeHelper.GetCustomAttributes(assembly, typeof(ManagedServiceContractAssemblyAttribute));
      if (attributes.Count > 0)
      {
        foreach (Type type in assembly.GetTypes())
        {
          bool isContractType = ScanType(type);
          if (isContractType)
          {
            ContractInfo info = new ContractInfo() { ContractType = type };
            if (!contracts.Contains(info))
            {
              contracts.Add(info);
            }
          }
        }
      }

      return contracts.AsReadOnly();
    }

    private static bool ScanType(Type type)
    {
      return CustomAttributeHelper.GetCustomAttributes(type, typeof(ManagedServiceContractAttribute)).Count > 0;
    }
  }
}
