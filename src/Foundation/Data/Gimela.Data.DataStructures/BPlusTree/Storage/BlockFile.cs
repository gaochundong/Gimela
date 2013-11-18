using System;
using System.IO;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 块文件
  /// </summary>
  internal class BlockFile
  {
    #region Properties

    /// <summary>
    /// 基础文件的流
    /// </summary>
    internal Stream Stream { get; private set; }
    /// <summary>
    /// 块索引起始点
    /// </summary>
    internal long SeekStart { get; private set; }
    /// <summary>
    /// 块的大小(Bytes)
    /// </summary>
    internal int BlockSize { get; private set; }
    /// <summary>
    /// 块存储头部前缀
    /// </summary>
    internal byte[] FileHeaderPrefix { get; private set; }
    /// <summary>
    /// 块存储头部大小(Bytes)
    /// </summary>
    internal int FileHeaderSize
    {
      get
      {
        // 魔数前缀长度 | 块的大小
        return FileHeaderPrefix.Length     // 魔数前缀长度
          + StoredConstants.IntegerLength; // 块的大小 BlockSize
      }
    }

    #endregion

    #region Ctors

    /// <summary>
    /// 块文件
    /// </summary>
    /// <param name="stream">基础文件的流</param>
    /// <param name="seekStart">流起始索引点</param>
    /// <param name="blockFileHeaderPrefix">块存储头部前缀</param>
    /// <param name="blockSize">块的大小</param>
    public BlockFile(Stream stream, long seekStart, byte[] blockFileHeaderPrefix, int blockSize)
    {
      this.Stream = stream;      
      this.SeekStart = seekStart;
      this.FileHeaderPrefix = blockFileHeaderPrefix;
      this.BlockSize = blockSize;

      if (this.Stream == null)
      {
        throw new ArgumentNullException("stream");
      }
      if (this.SeekStart < 0)
      {
        throw new BlockFileException("Cannot start at negative seek position " + this.SeekStart + ".");
      }
      if (this.FileHeaderPrefix == null || this.FileHeaderPrefix.Length < StoredConstants.MinFileHeaderPrefixLength)
      {
        throw new BlockFileException("Block file header prefix length is too small.");
      }
      if (this.BlockSize < StoredConstants.MinBlockSize)
      {
        throw new BlockFileException("Block size is too small " + this.BlockSize + ".");
      }
    }

    #endregion

    #region File Stream

    /// <summary>
    /// 从指定的流初始化块文件
    /// </summary>
    /// <param name="fromFile">指定的流</param>
    /// <param name="seekStart">流起始索引点</param>
    /// <param name="blockFileHeaderPrefix">块存储头部前缀</param>
    /// <param name="blockSize">块的大小</param>
    /// <returns>块文件</returns>
    public static BlockFile InitializeInStream(Stream fromFile, long seekStart, byte[] blockFileHeaderPrefix, int blockSize)
    {
      BlockFile file = new BlockFile(fromFile, seekStart, blockFileHeaderPrefix, blockSize);
      file.WriteFileHeader();
      return file;
    }

    /// <summary>
    /// 从指定的流初始化块文件
    /// </summary>
    /// <param name="fromFile">指定的流</param>
    /// <param name="seekStart">流起始索引点</param>
    /// <param name="blockFileHeaderPrefix">块存储头部前缀</param>
    /// <returns>块文件</returns>
    public static BlockFile SetupFromExistingStream(Stream fromFile, long seekStart, byte[] blockFileHeaderPrefix)
    {
      BlockFile file = new BlockFile(fromFile, seekStart, blockFileHeaderPrefix, 100); // dummy block size for now
      file.ReadFileHeader();
      return file;
    }

    #endregion

    #region File Header

    /// <summary>
    /// 写入块存储头部
    /// </summary>
    private void WriteFileHeader()
    {
      byte[] header = this.MakeFileHeader();
      this.Stream.Seek(this.SeekStart, SeekOrigin.Begin);
      this.Stream.Write(header, 0, header.Length);
    }

    /// <summary>
    /// 读取块存储头部
    /// </summary>
    private void ReadFileHeader()
    {
      // 魔数前缀长度 | 块的大小
      byte[] header = new byte[FileHeaderSize];
      this.Stream.Seek(this.SeekStart, SeekOrigin.Begin);
      this.Stream.Read(header, 0, FileHeaderSize);

      int index = 0;

      // 魔数前缀
      foreach (byte b in FileHeaderPrefix)
      {
        if (header[index] != b)
        {
          throw new BlockFileException("Invalid block file header prefix.");
        }
        index++;
      }

      // 块的大小 4 Bytes
      this.BlockSize = StoredHelper.RetrieveInt(header, index);
      index += StoredConstants.IntegerLength;

      if (this.BlockSize < StoredConstants.MinBlockSize)
      {
        throw new BlockFileException("Block size is too small " + this.BlockSize + ".");
      }
    }

    /// <summary>
    /// 构造存储头
    /// </summary>
    /// <returns></returns>
    public byte[] MakeFileHeader()
    {
      // 魔数前缀长度 | 块的大小
      byte[] header = new byte[FileHeaderSize];

      // 魔数前缀
      FileHeaderPrefix.CopyTo(header, 0);

      // 块的大小 4 Bytes
      StoredHelper.Store(this.BlockSize, header, FileHeaderPrefix.Length);

      return header;
    }

    #endregion

    #region Read & Write
    
    /// <summary>
    /// 读取块
    /// </summary>
    /// <param name="blockNumber">块序号</param>
    /// <param name="toArray">写入至此数组</param>
    /// <param name="startAt">写入起始点</param>
    /// <param name="length">写入长度</param>
    public void ReadBlock(long blockNumber, byte[] toArray, int startAt, int length)
    {
      if (blockNumber >= this.NextBlockNumber())
      {
        throw new BlockFileException("Last block is " + this.NextBlockNumber() + " not " + blockNumber + ".");
      }
      if (length > this.BlockSize)
      {
        throw new BlockFileException("Block size is too small for retrieval " + BlockSize + " need " + length + ".");
      }

      long seekPosition = this.SeekBlock(blockNumber);
      this.Stream.Seek(seekPosition, SeekOrigin.Begin);
      this.Stream.Read(toArray, startAt, length);
    }

    /// <summary>
    /// 写入块
    /// </summary>
    /// <param name="blockNumber">块序号</param>
    /// <param name="fromArray">读取至此数组</param>
    /// <param name="startAt">读取起始点</param>
    /// <param name="length">读取长度</param>
    public void WriteBlock(long blockNumber, byte[] fromArray, int startAt, int length)
    {
      if (length > this.BlockSize)
      {
        throw new BlockFileException("Block size is too small for assignment " + BlockSize + " need " + length + ".");
      }
      if (blockNumber > this.NextBlockNumber())
      {
        throw new BlockFileException("Cannot skip block numbers from " + this.NextBlockNumber() + " to " + blockNumber + ".");
      }

      long seekPosition = this.SeekBlock(blockNumber);
      this.Stream.Seek(seekPosition, SeekOrigin.Begin);
      this.Stream.Write(fromArray, startAt, length);
    }

    /// <summary>
    /// 获取指定块序号的索引
    /// </summary>
    /// <param name="blockNumber">块序号</param>
    /// <returns>指定块序号的索引</returns>
    public long SeekBlock(long blockNumber)
    {
      if (blockNumber < 0)
      {
        throw new BlockFileException("Block number cannot be negative.");
      }
      return this.SeekStart + FileHeaderSize + (this.BlockSize * blockNumber);
    }
    
    /// <summary>
    /// 生成下一个块序号
    /// </summary>
    /// <returns>新的块序号</returns>
    public long NextBlockNumber()
    {
      // round up the block number based on the current file length
      long fileLength = this.Stream.Length;
      long blockSpace = fileLength - this.SeekStart - FileHeaderSize;
      long numberOfBlocks = blockSpace / this.BlockSize;
      long remainder = blockSpace % this.BlockSize;
      if (remainder > 0)
      {
        return numberOfBlocks + 1;
      }
      return numberOfBlocks; // 块号从0开始，获取下一个块号
    }

    /// <summary>
    /// 刷新缓冲区
    /// </summary>
    public void Flush()
    {
      this.Stream.Flush();
    }

    #endregion
  }
}
