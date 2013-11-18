using System;
using System.Collections.ObjectModel;

namespace Gimela.Tasks.Expressions
{
  /// <summary>
  /// CronExpression集合
  /// </summary>
  [Serializable]
  public class CronExpressionCollection : Collection<int>
  {
    /// <summary>
    /// 对集合进行排序
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/zh-cn/library/bb383977.aspx
    /// </remarks>
    public void Sort()
    {
      int i, j;
      bool done = false;

      j = 1;
      while ((j < this.Count) && (!done))
      {
        done = true;

        for (i = 0; i < this.Count - j; i++)
        {
          if (this[i].CompareTo(this[i + 1]) > 0)
          {
            done = false;
            int t = this[i];
            this[i] = this[i + 1];
            this[i + 1] = t;
          }
        }

        j++;
      }
    }

    /// <summary>
    /// 获取指定元素的索引
    /// </summary>
    /// <param name="item">指定元素</param>
    /// <returns></returns>
    public int GetIndex(int item)
    {
      int index = 0;

      for (int i = 0; i < this.Count; i++)
      {
        if (this[i] == item)
        {
          index = i;
          break;
        }
      }

      return index;
    }
  }
}
