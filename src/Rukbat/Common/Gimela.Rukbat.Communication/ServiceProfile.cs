using System;

namespace Gimela.Rukbat.Communication
{
  public class ServiceProfile
  {
    public string Name { get; set; }

    public string ContractName { get; set; }

    public string Binding { get; set; }

    public string Address { get; set; }

    public string HostName { get; set; }

    public Uri Uri { get; set; }

    public override string ToString()
    {
      return Uri.ToString();
    }
  }
}
