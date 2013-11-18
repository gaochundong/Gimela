using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Json;
using Gimela.Data.DataStructures;
using Gimela.Data.Magpie;

namespace TestMagpieDatabase
{
  class Program
  {
    static void Main(string[] args)
    {
      TestMagpieDatabase();

      Console.ReadKey();
    }

    private static void TestMagpieDatabase()
    {
      MagpieServer server = MagpieServer.Create(@"C:\tmp");
      MagpieDatabase database = server.GetDatabase("test");
      MagpieCollection<Cat> catCollection = database.GetCollection<Cat>("cat");
      MagpieCollection<Dog> dogCollection = database.GetCollection<Dog>("dog");

      Console.WriteLine("Save One Cat");
      Cat origin = new Cat() { Name = "Garfield", Legs = 4 };
      catCollection.Save<Cat>(origin);

      Console.WriteLine("Retrieve One Cat");
      Cat retrieve = catCollection.FindOneById(origin.Id);
      Console.WriteLine(retrieve.ToString());

      Console.WriteLine("Retrieve All Cat");
      foreach (var item in catCollection.FindAll())
      {
        Console.WriteLine(item.ToString());
      }

      Console.WriteLine("Remove One Cat");
      Console.WriteLine(origin.ToString());
      catCollection.Remove(origin.Id);

      Console.WriteLine("Retrieve All Cat");
      foreach (var item in catCollection.FindAll())
      {
        Console.WriteLine(item.ToString());
      }

      Console.WriteLine("Remove All Cat");
      catCollection.RemoveAll();

      Console.WriteLine("Retrieve All Cat");
      foreach (var item in catCollection.FindAll())
      {
        Console.WriteLine(item.ToString());
      }

      Console.WriteLine("Save One Dog");
      Dog originDog = new Dog() { Name = "Spotted Dog", Legs = 4 };
      dogCollection.Save<Dog>(originDog);

      Console.WriteLine("Retrieve One Dog");
      Dog retrieveDog = dogCollection.FindOneById(originDog.Id);
      Console.WriteLine(retrieveDog.ToString());
    }
  }

  [Serializable]
  public class Cat : IMagpieDocumentId
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

    #region IMagpieDocumentId Members

    public string Id { get; set; }

    #endregion

    public override string ToString()
    {
      return string.Format("DocumentId={0}, Name={1}, Legs={2}", Id, Name, Legs);
    }
  }

  [Serializable]
  public class Dog : IMagpieDocumentId
  {
    public Dog()
    {
      Id = ObjectId.NewObjectId().ToString();
    }

    public Dog(string id)
    {
      Id = id;
    }

    public string Name { get; set; }
    public int Legs { get; set; }

    #region IMagpieDocumentId Members

    public string Id { get; set; }

    #endregion

    public override string ToString()
    {
      return string.Format("DocumentId={0}, Name={1}, Legs={2}", Id, Name, Legs);
    }
  }
}
