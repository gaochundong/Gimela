using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Infrastructure.Patterns;

namespace TestSingleton
{
  class Program
  {
    interface IInterfaceA
    {
      string GetData();
    }
    class ClassA : IInterfaceA
    {
      public string GetData()
      {
        return string.Format("This is from ClassA with hash [{0}].", this.GetHashCode());
      }
    }
    static void Main(string[] args)
    {
      string data1 = Singleton<ClassA>.GetInstance().GetData();
      Console.WriteLine(data1);
      string data2 = Singleton<ClassA>.GetInstance().GetData();
      Console.WriteLine(data2);

      Console.ReadKey();
    }
  }
}
