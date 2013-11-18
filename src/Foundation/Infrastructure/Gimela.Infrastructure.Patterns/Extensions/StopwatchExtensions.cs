using System;
using System.Diagnostics;

namespace Gimela.Infrastructure.Patterns
{
  public static class StopwatchExtensions
  {
    public static Stopwatch StopWatch(this Stopwatch watch)
    {
      if (watch.IsRunning)
      {
        watch.Stop();
      }
      return watch;
    }

    public static double ElapsedSeconds(this Stopwatch watch)
    {
      return Math.Round((double)watch.ElapsedMilliseconds / 1000, 7, MidpointRounding.ToEven);
    }
  }
}
