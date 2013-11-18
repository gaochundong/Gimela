
namespace Gimela.Data.Json.Bson
{
  /// <summary>
  /// BSON, short for Bin­ary JSON, is a bin­ary-en­coded seri­al­iz­a­tion of JSON-like doc­u­ments. 
  /// Like JSON, BSON sup­ports the em­bed­ding of doc­u­ments and ar­rays with­in oth­er doc­u­ments and ar­rays.
  /// </summary>
  public static class BsonConvert
  {
    /// <summary>
    /// 将对象序列化至BSON二进制数组
    /// </summary>
    /// <param name="target">被序列化的对象</param>
    /// <returns>BSON二进制数组</returns>
    public static byte[] SerializeObject(object target)
    {
      return SerializeObject(target, false);
    }

    /// <summary>
    /// 将对象序列化至BSON二进制数组
    /// </summary>
    /// <param name="target">被序列化的对象</param>
    /// <param name="isSerializeNullValue">是否序列化NULL值属性</param>
    /// <returns>BSON二进制数组</returns>
    public static byte[] SerializeObject(object target, bool isSerializeNullValue)
    {
      using (BsonSerializer serializer = new BsonSerializer())
      {
        serializer.IsSerializeNullValue = isSerializeNullValue;
        return serializer.Serialize(target);
      }
    }

    /// <summary>
    /// 从BSON二进制数组反序列化至指定类型的对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="bson">BSON二进制数组</param>
    /// <returns>指定类型的对象</returns>
    public static T DeserializeObject<T>(byte[] bson) where T : class
    {
      using (BsonDeserializer deserializer = new BsonDeserializer())
      {
        return deserializer.Deserialize<T>(bson);
      }
    }
  }
}
