using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestJson
{
  class Program
  {
    static void Main(string[] args)
    {
      TestJson();

      //TestBson();
    }

    private static void TestBson()
    {
      byte[] bson = Gimela.Data.Json.Bson.BsonConvert.SerializeObject(new ClassA());

      ClassA obj = Gimela.Data.Json.Bson.BsonConvert.DeserializeObject<ClassA>(bson);

      Console.ReadKey();
    }

    private static void TestJson()
    {
      string json = Gimela.Data.Json.JsonConvert.SerializeObject(new ClassA());
      Console.WriteLine(json);

      ClassA obj = Gimela.Data.Json.JsonConvert.DeserializeObject<ClassA>(json);

      Console.ReadKey();
    }
  }

  public class ClassA
  {
    public ClassA()
    {
      MyInt32 = 100;
      MyString = "Test string  \n\t 中文字符串";
      MyLong = 999;
      MyDateTime = new DateTime(2012, 1, 1, 1, 1, 1, DateTimeKind.Local);
      MyDouble = 2.3333333d;
      MyFloat = 3.555f;
      MyBoolean = true;
      MyByteArray = new byte[10];
      for (int i = 0; i < MyByteArray.Length; i++)
      {
        MyByteArray[i] = (byte)i;
      }
      MyClassB = new ClassB();
      MyStringDictionary = new Dictionary<string, string>();
      MyStringDictionary.Add("11", "11111111111111111111");
      MyStringDictionary.Add("22", "333333333333333");
      MyObjectDictionary = new Dictionary<string, ClassB>();
      MyObjectDictionary.Add("aa", new ClassB());
      MyObjectDictionary.Add("bb", new ClassB());
      MyNull = null;
      MyList = new List<string>();
      MyList.Add("qq");
      MyList.Add("ww");
      MyList.Add("tt");
    }
    public int MyInt32 { get; set; }
    public string MyString { get; set; }
    public long MyLong { get; set; }
    public DateTime MyDateTime { get; set; }
    public double MyDouble { get; set; }
    public float MyFloat { get; set; }
    public bool MyBoolean { get; set; }
    public byte[] MyByteArray { get; set; }
    public ClassB MyClassB { get; set; }
    public Dictionary<string, string> MyStringDictionary { get; set; }
    public Dictionary<string, ClassB> MyObjectDictionary { get; set; }
    public string MyNull { get; set; }
    public List<string> MyList { get; set; }
  }

  public class ClassB
  {
    public ClassB()
    {
      MyInt = 444;
      MyClassC = new ClassC();

      MyList = new List<string>();
      MyList.Add("rr");
      MyList.Add("tt");

      MyDictionary = new Dictionary<string, string>();
      MyDictionary.Add("33", "11111111111111111111");
      MyDictionary.Add("44", "333333333333333");
    }
    public int MyInt { get; set; }
    public ClassC MyClassC { get; set; }
    public List<string> MyList { get; set; }
    public Dictionary<string, string> MyDictionary { get; set; }
  }

  public class ClassC
  {
    public ClassC()
    {
      MyInt = 4;
    }
    public int MyInt { get; set; }
  }
}
