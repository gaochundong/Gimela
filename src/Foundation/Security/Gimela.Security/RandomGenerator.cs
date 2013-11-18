using System;
using System.Security.Cryptography;

namespace Gimela.Security
{
  /// <summary>
  /// 随机数生成器
  /// </summary>
  public static class RandomGenerator
  {
    /// <summary>
    /// Generates a random salt value by giving a length.
    /// </summary>
    /// <param name="length">a value must be greater than 0</param>
    /// <returns>The byte array of value that represents salt. This value is non-zero byte value.</returns>
    /// <remarks>In cryptography, a salt consists of random bits, creating one of the inputs to a one-way function.</remarks>
    public static byte[] NextRandomSalt(int length)
    {
      if (length <= 0)
      {
        throw new ArgumentOutOfRangeException("length", "Salt length must be greater than 0");
      }

      byte[] salt = new byte[length];

      using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
      {
        rng.GetNonZeroBytes(salt);
      }

      return salt;
    }

    /// <summary>
    /// Generates a random salt value by giving a range of length.  
    /// Giving a range of length, the length of salt will be randomly generated basing 
    /// on the range.  
    /// </summary>
    /// <param name="minLength">The minimum length of the salt value (including min)</param>
    /// <param name="maxLength">The maximum length of the salt value (including max)</param>
    /// <returns>The byte array of value that represents salt. This value is non-zero byte value.</returns>
    /// <remarks>In cryptography, a salt consists of random bits, creating one of the inputs to a one-way function.</remarks>
    public static byte[] NextRandomSalt(int minLength, int maxLength)
    {
      return NextRandomSalt(NextRandomValue(minLength, maxLength));
    }

    /// <summary>
    /// Generates a random value with giving range of min and max values.
    /// </summary>
    /// <param name="minValue">The minimum value (including min). The value must be equal or greater than 0</param>
    /// <param name="maxValue">The maximum value (including max). The value must be equal or greater than 0 and equal or greater than min value.</param>
    /// <returns>The randomly generated value between a given min and max value</returns>
    public static int NextRandomValue(int minValue, int maxValue)
    {
      if (minValue < 0)
      {
        throw new ArgumentOutOfRangeException("minValue", "minValue must be 0 or greater than 0");
      }
      if (maxValue < 0)
      {
        throw new ArgumentOutOfRangeException("maxValue", "maxValue must be 0 or greater than 0");
      }
      if (maxValue < minValue)
      {
        throw new ArgumentOutOfRangeException("maxValue", "max value must be greater than or equal min value");
      }

      Random rnd = new Random();
      int randValue = rnd.Next(minValue, ++maxValue);

      return randValue;
    }
  }
}
