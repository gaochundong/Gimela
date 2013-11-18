using System;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库URL构造器
  /// </summary>
  [Serializable]
  public class MagpieUrlBuilder
  {
    private MagpieServerAddress _server;

    /// <summary>
    /// 数据库URL构造器
    /// </summary>
    public MagpieUrlBuilder()
    {
      _server = null;
    }

    /// <summary>
    /// 数据库URL构造器
    /// </summary>
    /// <param name="url">数据库URL</param>
    public MagpieUrlBuilder(string url)
      : this()
    {
      Parse(url);
    }

    /// <summary>
    /// 数据库服务器地址
    /// </summary>
    public MagpieServerAddress Server
    {
      get { return _server; }
      set { _server = value; }
    }

    private void Parse(string url)
    {
      _server = MagpieServerAddress.Create(url);
    }

    /// <summary>
    /// 生成数据库服务器配置
    /// </summary>
    /// <returns></returns>
    public MagpieServerSettings ToServerSettings()
    {
      return new MagpieServerSettings(_server);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return _server.ToString();
    }
  }
}
