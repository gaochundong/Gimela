using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.DataStructures;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace TestBPlusTree
{
  class Program
  {
    static Hashtable AllInsertHashtable = new Hashtable();
    static Hashtable LastCommittedInsertHashtable = new Hashtable();

    static string tempDirectory = @"C:\";
    static int keylength = 5;
    static int nodesize = 2;
    static int buffersize = 100;

    static void Main(string[] args)
    {
      TestLongTree();

      Console.ReadKey();
    }

    public static void TestLongTree()
    {
      Stream mstream = new FileStream(string.Format(@"{0}\{1}.txt", tempDirectory, DateTime.Now.ToString("HHmmss")), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
      BPlusTreeLong tree = BPlusTreeLong.InitializeInStream(mstream, 0L, keylength, nodesize);

      Insert(tree, "aaa", 11111);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "bbb", 22222);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "ccc", 33333);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "ddd", 44444);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "eee", 55555);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "fff", 66666);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "ggg", 77777);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "hhh", 88888);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "iii", 99999);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "ggh", 77777);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "ggi", 77777);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "ggj", 77777);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Insert(tree, "ggk", 77777);
      Debug.WriteLine(tree.ToText());
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Debug.WriteLine("aaa" + " " + tree.Get("aaa").ToString());
      Debug.WriteLine("bbb" + " " + tree.Get("bbb").ToString());
      Debug.WriteLine("ccc" + " " + tree.Get("ccc").ToString());
      Debug.WriteLine("ddd" + " " + tree.Get("ddd").ToString());

      Delete(tree, "aaa");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "bbb");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "ccc");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "ddd");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "eee");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "fff");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "ggg");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "hhh");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "iii");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "ggh");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "ggi");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "ggj");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

      Delete(tree, "ggk");
      Commit(tree);
      Debug.WriteLine(tree.ToText());

    }
    public static void Insert(BPlusTreeLong bpt, string key, long map)
    {
      bpt.Set(key, map);
      AllInsertHashtable[key] = map;
      Check(bpt);
    }
    public static void Delete(BPlusTreeLong bpt, string key)
    {
      bpt.RemoveKey(key);
      AllInsertHashtable.Remove(key);
      Check(bpt);
    }
    public static void Commit(BPlusTreeLong bpt)
    {
      bpt.Commit();
      LastCommittedInsertHashtable = (Hashtable)AllInsertHashtable.Clone();
      Check(bpt);
    }
    public static void Abort(BPlusTreeLong bpt)
    {
      bpt.Abort();
      AllInsertHashtable = (Hashtable)LastCommittedInsertHashtable.Clone();
      Check(bpt);
    }
    public static void Check(BPlusTreeLong bpt)
    {
      Console.WriteLine(bpt.ToText());
      bpt.CheckFreeBlock();
      ArrayList allkeys = new ArrayList();
      foreach (DictionaryEntry d in AllInsertHashtable)
      {
        allkeys.Add(d.Key);
      }
      allkeys.Sort();
      allkeys.Reverse();
      foreach (object thing in allkeys)
      {
        string thekey = (string)thing;
        long thevalue = (long)AllInsertHashtable[thing];
        if (thevalue != bpt[thekey])
        {
          throw new ApplicationException("no match on retrieval " + thekey + " --> " + bpt[thekey] + " not " + thevalue);
        }
      }
      allkeys.Reverse();
      string currentkey = bpt.FirstKey();
      foreach (object thing in allkeys)
      {
        string testkey = (string)thing;
        if (currentkey == null)
        {
          throw new ApplicationException("end of keys found when expecting " + testkey);
        }
        if (!testkey.Equals(currentkey))
        {
          throw new ApplicationException("when walking found " + currentkey + " when expecting " + testkey);
        }
        currentkey = bpt.NextKey(testkey);
      }
    }

    public static void TestStringTree()
    {
      Stream treefile = null, blockfile = null;
      BPlusTree tree = GetStringTree(null, ref treefile, ref blockfile, false);
      Debug.WriteLine(tree.ToText());

      Hashtable allmaps = new Hashtable();
      for (int i = 0; i < 2; i++)
      {
        for (int j = 0; j < 2; j++)
        {
          Hashtable record = new Hashtable();
          for (int k = 0; k < 2; k++)
          {
            string thiskey = MakeKey(i, j, k);
            string thisvalue = MakeValue(j, k, i);

            Debug.WriteLine("Set Pair: [" + thiskey + " : " + thisvalue + "]");

            record[thiskey] = thisvalue;
            tree[thiskey] = thisvalue;
            tree.Commit();

            Debug.WriteLine("Get Pair: [" + thiskey + " : " + tree.Get(thiskey).ToString() + "]");

            Debug.WriteLine(tree.ToText());
          }
        }
      }
    }
    public static BPlusTree GetStringTree(BPlusTree bpt, ref Stream treefile, ref Stream blockfile, bool isReadOnly)
    {
      if (tempDirectory != null)
      {
        // allocate in filesystem
        string treename = Path.Combine(tempDirectory, "BPlusTreeString.dat");
        string blockname = Path.Combine(tempDirectory, "BPlusTreeStringBlock.dat");
        treefile = null;
        blockfile = null;
        if (bpt == null)
        {
          // allocate new
          if (File.Exists(treename))
          {
            File.Delete(treename);
          }
          if (File.Exists(blockname))
          {
            File.Delete(blockname);
          }
          bpt = BPlusTree.Create(treename, blockname, keylength, nodesize, buffersize);
        }
        else
        {
          // reopen old
          bpt.Shutdown();
          if (isReadOnly)
          {
            bpt = BPlusTree.OpenReadOnly(treename, blockname);
          }
          else
          {
            bpt = BPlusTree.Open(treename, blockname);
          }
        }
      }
      else
      {
        // allocate in memory
        if (bpt == null)
        {
          // allocate new
          treefile = new MemoryStream();
          blockfile = new MemoryStream(); ;
          bpt = BPlusTree.Create(treefile, blockfile, keylength, nodesize, buffersize);
        }
        else
        {
          // reopen
          bpt = BPlusTree.Open(treefile, blockfile);
        }
      }
      return bpt;
    }
    public static string MakeKey(int i, int j, int k)
    {
      int selector = (i + j + k) % 3;
      string result = "" + i + "." + j + "." + k;
      if (selector == 0)
      {
        result = "" + k + "." + (j % 5) + "." + i;
      }
      else if (selector == 1)
      {
        result = "" + k + "." + j + "." + i;
      }
      return result;
    }
    public static string MakeValue(int i, int j, int k)
    {
      if (((i + j + k) % 5) == 3)
      {
        return "";
      }
      StringBuilder sb = new StringBuilder();
      sb.Append("value#");
      for (int x = 0; x < i + k * 500; x++)
      {
        sb.Append(j);
        sb.Append(k);
      }
      return sb.ToString();
    }
  }
}
