using System;

namespace Gimela.Presentation.Controls
{
  public class DateTextItem
  {
    public DateTextItem()
    {

    }

    public DateTime Date { get; set; }
    public string Text { get; set; }

    public DateTextItem(int date, string text, bool isFestival)
    {
      DateOfMonth = date;
      Text = text;
      IsFestival = isFestival;
    }

    public int DateOfMonth { get; set; }

    public bool IsFestival { get; set; }
  }
}
