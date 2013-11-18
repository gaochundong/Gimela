using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gimela.Data.Json.Bson
{
  internal enum BsonType : sbyte
  {
    Number = 1,
    String = 2,
    Object = 3,
    Array = 4,
    Binary = 5,
    Undefined = 6,
    ObjectId = 7,
    Boolean = 8,
    DateTime = 9,
    Null = 10,
    Regex = 11,
    Reference = 12,
    Code = 13,
    Symbol = 14,
    ScopedCode = 15,
    Integer = 16,
    Timestamp = 17,
    Long = 18,
    MinKey = -1,
    MaxKey = 127,
  }

  internal static class BsonTypeMapping
  {
    internal static readonly IDictionary<Type, BsonType> SerializeTypeMapping =
      new Dictionary<Type, BsonType>
        {
          {typeof (int), BsonType.Integer},
          {typeof (long), BsonType.Long},
          {typeof (bool), BsonType.Boolean},
          {typeof (string), BsonType.String},
          {typeof (double), BsonType.Number},
          {typeof (Guid), BsonType.Binary},
          {typeof (Regex), BsonType.Regex},
          {typeof (DateTime), BsonType.DateTime},
          {typeof (float), BsonType.Number},
          {typeof (byte[]), BsonType.Binary},
          {typeof (ObjectId), BsonType.ObjectId},
        };

    internal readonly static IDictionary<BsonType, Type> DeserializeTypeMapping = 
      new Dictionary<BsonType, Type>
        {
          {BsonType.Integer, typeof(int)}, 
          {BsonType.Long, typeof (long)}, 
          {BsonType.Boolean, typeof (bool)}, 
          {BsonType.String, typeof (string)},
          {BsonType.Number, typeof(double)}, 
          {BsonType.Binary, typeof (byte[])}, 
          {BsonType.Regex, typeof (Regex)}, 
          {BsonType.DateTime, typeof (DateTime)},
          {BsonType.ObjectId, typeof(ObjectId)}
        };
  }
}
