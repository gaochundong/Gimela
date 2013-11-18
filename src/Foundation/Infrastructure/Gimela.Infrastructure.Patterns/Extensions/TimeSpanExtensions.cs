using System;
using System.Diagnostics;

namespace Gimela.Infrastructure.Patterns
{
  public static class TimeSpanExtensions
  {
    public const int TimeSpanTicksPerSecond = 10000000;

    public static long PerformanceCounterTicks(this TimeSpan timeSpan)
    {
      return timeSpan.Ticks * Stopwatch.Frequency / TimeSpanTicksPerSecond;
    }

    public static double ToWellFormatSeconds(this TimeSpan timeSpan)
    {
      return Math.Round(timeSpan.TotalMilliseconds / 1000, 7, MidpointRounding.ToEven);
    }
  }
}
