using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using Gimela.Rukbat.SVD.BusinessEntities;

namespace Gimela.Rukbat.SVD.BusinessLogic
{
  public class ContractFinder : IContractFinder
  {
    #region IContractFinder Members

    public ReadOnlyCollection<ContractInfo> Find()
    {
      string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      return ContractDirectory.Scan(currentAssemblyDirectoryName);
    }

    #endregion
  }
}
