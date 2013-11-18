using System;
using System.Text;

namespace Gimela.Data.Json
{
  internal static class DateTimeHelper
  {
    internal static readonly long InitialJavaScriptDateTicks = 621355968000000000L;

    internal static string WriteDateTimeString(DateTime value)
    {
      return WriteDateTimeString(value, GetUtcOffset(value), value.Kind);
    }

    internal static string WriteDateTimeString(DateTime value, TimeSpan offset, DateTimeKind kind)
    {
      StringBuilder sb = new StringBuilder();

      sb.Append("\"\\/Date(");
      sb.Append(ConvertDateTimeToJavaScriptTicks(value, offset));

      switch (kind)
      {
        case DateTimeKind.Unspecified:
        case DateTimeKind.Local:
          {
            sb.Append((offset.Ticks >= 0L) ? "+" : "-");

            int hours = Math.Abs(offset.Hours);
            if (hours < 10)
            {
              sb.Append(0);
            }
            sb.Append(hours);

            int minutes = Math.Abs(offset.Minutes);
            if (minutes < 10)
            {
              sb.Append(0);
            }
            sb.Append(minutes);

            break;
          }
      }

      sb.Append(")\\/\"");

      return sb.ToString();
    }

    internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
    {
      long universialTicks = ToUniversalTicks(dateTime, offset);
      return UniversialTicksToJavaScriptTicks(universialTicks);
    }

    internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
    {
      return ConvertDateTimeToJavaScriptTicks(dateTime, true);
    }

    internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
    {
      long universialTicks = convertToUtc ? ToUniversalTicks(dateTime) : dateTime.Ticks;
      return UniversialTicksToJavaScriptTicks(universialTicks);
    }

    internal static long UniversialTicksToJavaScriptTicks(long universialTicks)
    {
      return (universialTicks - InitialJavaScriptDateTicks) / 10000L;
    }

    internal static long ToUniversalTicks(DateTime dateTime)
    {
      if (dateTime.Kind == DateTimeKind.Utc)
      {
        return dateTime.Ticks;
      }
      return ToUniversalTicks(dateTime, GetUtcOffset(dateTime));
    }

    internal static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
    {
      if (dateTime.Kind == DateTimeKind.Utc)
      {
        return dateTime.Ticks;
      }
      long num = dateTime.Ticks - offset.Ticks;
      if (num > 3155378975999999999L)
      {
        return 3155378975999999999L;
      }
      if (num < 0L)
      {
        return 0L;
      }
      return num;
    }

    internal static TimeSpan GetUtcOffset(DateTime dateTime)
    {
      return TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
    }

    internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
    {
      DateTime result = new DateTime(javaScriptTicks * 10000L + InitialJavaScriptDateTicks, DateTimeKind.Utc);
      return result;
    }
  }
}
