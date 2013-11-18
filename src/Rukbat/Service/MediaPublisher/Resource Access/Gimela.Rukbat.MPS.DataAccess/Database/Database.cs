using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Magpie;

namespace Gimela.Rukbat.MPS.DataAccess
{
  public class Database : IDatabase
  {
    private MagpieServer _server;
    private MagpieDatabase _database;

    public Database(string path, string databaseName)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException("path");
      if (string.IsNullOrEmpty(databaseName))
        throw new ArgumentNullException("databaseName");

      _server = MagpieServer.Create(path);
      _database = _server.GetDatabase(databaseName);
    }

    #region IDatabase Members

    public MagpieDatabase Instance
    {
      get
      {
        return _database;
      }
    }

    public void Shutdown()
    {
      _server.Shutdown();
    }

    #endregion
  }
}
