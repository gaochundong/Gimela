using System;
using System.Collections.ObjectModel;

namespace Gimela.Infrastructure.Patterns
{
  public interface ISmartQueueMapper<TMember, TItem> : IDisposable
  {
    string Name { get; }
    ReadOnlyCollection<TMember> Members { get; }
    int MemberCount { get; }
    int ActiveMemberCount { get; }
    int ActiveMemberQueueAverageLength { get; }
    int ActiveMemberQueueMaxLength { get; }
    int ActiveMemberQueueTotalLength { get; }
    int TotalWorkerCount { get; }

    void Enqueue(TMember member, TItem item);
    void Remove(TMember member);
    ISmartQueueMapper<TMember, TItem> Pause();
    ISmartQueueMapper<TMember, TItem> Resume();
  }
}
