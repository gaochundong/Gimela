using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Gimela.Rukbat.SVD.BusinessEntities;

namespace Gimela.Rukbat.SVD.BusinessLogic
{
  public interface IContractFinder
  {
    ReadOnlyCollection<ContractInfo> Find();
  }
}
