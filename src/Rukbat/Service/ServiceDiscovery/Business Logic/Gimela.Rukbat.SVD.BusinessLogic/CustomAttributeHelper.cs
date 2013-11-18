using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Gimela.Rukbat.SVD.BusinessLogic
{
  public static class CustomAttributeHelper
  {
    public static ReadOnlyCollection<CustomAttributeData> GetCustomAttributes(Assembly assembly, Type customAttributeType)
    {
      List<CustomAttributeData> data = new List<CustomAttributeData>();

      foreach (CustomAttributeData customAttributeData in CustomAttributeData.GetCustomAttributes(assembly))
      {
        if (customAttributeData.Constructor.DeclaringType.Name == customAttributeType.Name)
        {
          data.Add(customAttributeData);
        }
      }

      return data.AsReadOnly();
    }

    public static ReadOnlyCollection<CustomAttributeData> GetCustomAttributes(Type type, Type customAttributeType)
    {
      List<CustomAttributeData> data = new List<CustomAttributeData>();

      foreach (CustomAttributeData customAttributeData in CustomAttributeData.GetCustomAttributes(type))
      {
        if (customAttributeData.Constructor.DeclaringType.Name == customAttributeType.Name)
        {
          data.Add(customAttributeData);
        }
      }

      return data.AsReadOnly();
    }
  }
}
