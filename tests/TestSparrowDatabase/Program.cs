using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Json;
using Gimela.Data.Sparrow;

namespace TestSparrowDatabase
{
  class Program
  {
    static void Main(string[] args)
    {
      TestJsonDatabase();
      TestXmlDatabase();

      Console.ReadKey();
    }

    private static void TestJsonDatabase()
    {
      JsonDatabase db = new JsonDatabase(@"C:\tmp");
      db.OutputIndent = true;

      Cat origin = new Cat() { Name = "Garfield", Legs = 4 };
      db.Save<Cat>(origin);

      db.Save<Cat>(origin.Id, origin);
      db.Delete<Cat>(origin.Id);
    }

    private static void TestXmlDatabase()
    {
      XmlDatabase db = new XmlDatabase(@"C:\tmp");
      db.OutputIndent = true;

      Cat origin = new Cat() { Name = "Garfield", Legs = 4 };
      db.Save<Cat>(origin);

      db.Save<Cat>(origin.Id, origin);
      db.Delete<Cat>(origin.Id);
    }
  }

  [Serializable]
  public class Cat
  {
    public Cat()
    {
      Id = ObjectId.NewObjectId().ToString();
    }

    public Cat(string id)
    {
      Id = id;
    }

    public string Name { get; set; }
    public int Legs { get; set; }

    public string Id { get; set; }

    public override string ToString()
    {
      return string.Format("DocumentId={0}, Name={1}, Legs={2}", Id, Name, Legs);
    }
  }
}
