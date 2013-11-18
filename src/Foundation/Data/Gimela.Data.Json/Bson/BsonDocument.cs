
namespace Gimela.Data.Json.Bson
{
  internal class BsonDocument
  {
    /// <summary>
    /// 父文档
    /// </summary>
    public BsonDocument Parent;
    /// <summary>
    /// 文档长度
    /// </summary>
    public int Length;
    /// <summary>
    /// 文档摘要Byte的数量
    /// </summary>
    public int Digested;
  }
}
