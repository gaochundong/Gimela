using System.Collections.Generic;
using System.Management;
using System.Net;

namespace Gimela.Management
{
  /// <summary>
  /// 本机描述信息
  /// </summary>
  public static class LocalMachine
  {
    /// <summary>
    /// 本机主机名
    /// </summary>
    public static string HostName
    {
      get
      {
        return Dns.GetHostName();
      }
    }

    /// <summary>
    /// 本机IP主机信息
    /// </summary>
    public static IPHostEntry IPHost
    {
      get
      {
        return Dns.GetHostEntry(Dns.GetHostName());
      }
    }

    /// <summary>
    /// 本机IP地址列表
    /// </summary>
    public static IList<string> IPAddresses
    {
      get
      {
        IList<string> ipAddressList = new List<string>();

        using (ManagementObjectSearcher query =
            new ManagementObjectSearcher(
                "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'"))
        {
          ManagementObjectCollection nics = query.Get();
          foreach (ManagementObject nic in nics)
          {
            string caption = nic["Caption"] as string;
            string description = nic["Description"] as string;
            string[] ipaddress = nic["IPAddress"] as string[];

            if (caption.StartsWith(@"VM", true, null) || description.StartsWith(@"VM", true, null))
            {
              // 过滤掉VMWare
              continue;
            }
            else if (caption.StartsWith(@"MS", true, null) || description.StartsWith(@"MS", true, null))
            {
              // 过滤掉MS TCP Loop
              continue;
            }

            for (int i = 0; i < ipaddress.Length; i++)
            {
              ipAddressList.Add(ipaddress[i]);
            }
          }
        }

        return ipAddressList;
      }
    }

    /// <summary>
    /// 本机MAC地址列表
    /// </summary>
    public static IList<string> MacAddresses
    {
      get
      {
        IList<string> macAddresses = new List<string>();

        using (ManagementObjectSearcher query =
            new ManagementObjectSearcher(
                "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'"))
        {
          ManagementObjectCollection nics = query.Get();
          foreach (ManagementObject nic in nics)
          {
            string caption = nic["Caption"] as string;
            string description = nic["Description"] as string;
            string macAddress = nic["MACAddress"] as string;

            if (caption.StartsWith(@"VM", true, null) || description.StartsWith(@"VM", true, null))
            {
              // 过滤掉VMWare
              continue;
            }
            else if (caption.StartsWith(@"MS", true, null) || description.StartsWith(@"MS", true, null))
            {
              // 过滤掉MS TCP Loop
              continue;
            }

            macAddresses.Add(macAddress);
          }
        }

        return macAddresses;
      }
    }
  }
}
