using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Infrastructure.Patterns;

namespace TestFlyweight
{
  class Program
  {
    static void Main(string[] args)
    {
      var pool = new FlyweightObjectPool<byte[]>(() => new byte[65535]);
      pool.Allocate(1000);

      var buffer = pool.Dequeue();

      // .. do something here ..

      pool.Enqueue(buffer);
    }
  }
}
