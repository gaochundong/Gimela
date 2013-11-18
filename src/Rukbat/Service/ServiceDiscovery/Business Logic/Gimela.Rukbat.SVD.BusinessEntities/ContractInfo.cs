using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.SVD.BusinessEntities
{
  public class ContractInfo : IComparable
  {
    public Type ContractType { get; set; }

    #region IComparable Members

    public int CompareTo(object obj)
    {
      if (obj is ContractInfo)
      {
        ContractInfo other = (ContractInfo)obj;

        // 根据名称进行比较
        return this.ContractType.FullName.CompareTo(other.ContractType.FullName);
      }
      else
      {
        throw new ArgumentException("The target object is not a <ContractInfo>.");
      }
    }

    public override bool Equals(object obj)
    {
      return this.CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    #endregion

    public override string ToString()
    {
      return ContractType.FullName;
    }
  }
}
