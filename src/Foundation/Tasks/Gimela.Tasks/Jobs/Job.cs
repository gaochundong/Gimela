using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Tasks.Jobs
{
  /// <summary>
  /// 作业，这是一个抽象类。
  /// </summary>
  [Serializable]
  [DataContract]
  public abstract class Job : IJob
  {
    /// <summary>
    /// 作业构造函数
    /// </summary>
    protected Job()
    {
    }

    /// <summary>
    /// 作业Id
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string Id { get; set; }

    /// <summary>
    /// 作业名称
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// 执行作业
    /// </summary>
    public abstract void Run();

    #region ToString

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"Id[{0}], Name[{1}]", Id, Name);
    }

    #endregion
  }
}
