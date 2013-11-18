using System;

namespace Gimela.Rukbat.DomainModels
{
  public interface IBaseObject : IComparable
  {
    string Id { get; set; }
    string Name { get; set; }
  }
}
