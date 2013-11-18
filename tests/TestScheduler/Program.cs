using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gimela.Tasks.Expressions;

namespace TestScheduler
{
  class Program
  {
    static void Main(string[] args)
    {
      TestCronExpression();

      Console.ReadKey();
    }

    private static void TestCronExpression()
    {
      CronExpression cron = new CronExpression();

      //cron.CronExpressionString = @"* * * * * * *";
      //cron.CronExpressionString = @"23-37 * * * * * *";
      //cron.CronExpressionString = @"10-36/3 * * * * * *";
      //cron.CronExpressionString = @"10,23,27,33,55 * * * * * *";
      //cron.CronExpressionString = @"13 * * * * * *";
      //cron.CronExpressionString = @"* * * * * 1 *";
      //cron.CronExpressionString = @"1 2 3 5 12 1 *";
      //cron.CronExpressionString = @"1 2 3 * * 0 *";
      //cron.CronExpressionString = @"1 2 15 * * 0 *";
      //cron.CronExpressionString = @"* * * * * 0 *";
      //cron.CronExpressionString = @"20 * * * * 0 *";
      cron.CronExpressionString = @"0 42 23 1 10 * 2099";

      for (int i = 0; i < 100; i++)
      {
        Thread.Sleep(2000);
        Console.WriteLine("Now  : " + DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss"));
        Console.WriteLine("Cron : " + cron.NextTime.Value.ToString(@"yyyy-MM-dd HH:mm:ss"));
        Console.WriteLine((long)(Math.Round((cron.NextTime.Value - DateTime.Now).TotalMilliseconds)));
      }      
    }
  }
}
