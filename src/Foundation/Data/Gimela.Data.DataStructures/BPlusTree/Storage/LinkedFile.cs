using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 链式块文件
  /// </summary>
  internal class LinkedFile
  {
    #region Consts

    /// <summary>
    /// 块开销 = 块标记(1 Byte) + 链接的下一个块序号(8 Bytes)
    /// </summary>
    public const int LinkedBlockOverhead = 1 + StoredConstants.LongLength;

    #endregion

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
    /// 链式块文件的块
    /// </summary>
    internal BlockFile BlockFile { get; private set; }
    /// <summary>
    /// 块的大小
    /// </summary>
    internal int BlockSize { get; private set; }
    /// <summary>
    /// 块存储头部前缀
    /// </summary>
    internal byte[] FileHeaderPrefix { get; private set; }
    /// <summary>
    /// 块存储头部大小
    /// </summary>
    internal int FileHeaderSize
    {
      get
      {
        // 魔数前缀长度 | 块的大小 | 空闲列表头序号
        return FileHeaderPrefix.Length    // 魔数前缀长度
          + StoredConstants.IntegerLength // 块的大小       4 Bytes
          + StoredConstants.LongLength;   // 空闲列表头序号 8 Bytes
      }
    }
    /// <summary>
    /// 块头部是否已被更改
    /// </summary>
    internal bool IsHeaderDirty { get; private set; }
    /// <summary>
    /// 空闲块头序号
    /// </summary>
    internal long FreeBlockHead { get; private set; }
    /// <summary>
    /// 最近新分配的块序号
    /// </summary>
    internal long RecentNewBlockNumber { get; private set; }

    #endregion

    #region Ctors

    /// <summary>
    /// 链式块文件
    /// </summary>
    /// <param name="stream">指定的流</param>
    /// <param name="seekStart">流起始索引点</param>
    /// <param name="linkedFileHeaderPrefix">块存储头部前缀</param>
    /// <param name="blockSize">块的大小</param>
    public LinkedFile(Stream stream, long seekStart, byte[] linkedFileHeaderPrefix, int blockSize)
    {
      this.Stream = stream;
      this.SeekStart = seekStart;
      this.FileHeaderPrefix = linkedFileHeaderPrefix;
      this.BlockSize = blockSize;      

      this.IsHeaderDirty = true;
      this.FreeBlockHead = StoredConstants.NullBlockNumber;
      this.RecentNewBlockNumber = StoredConstants.NullBlockNumber;

      if (this.Stream == null)
      {
        throw new ArgumentNullException("stream");
      }
      if (this.SeekStart < 0)
      {
        throw new LinkedFileException("cannot seek negative " + this.SeekStart);
      }
      if (this.FileHeaderPrefix == null || this.FileHeaderPrefix.Length < StoredConstants.MinFileHeaderPrefixLength)
      {
        throw new LinkedFileException("Linked file header prefix length is too small.");
      }
      if (this.BlockSize < StoredConstants.MinBlockSize)
      {
        throw new LinkedFileException("buffer size too small " + this.BlockSize);
      }
    }

    #endregion

    #region File Stream

    /// <summary>
    /// 从指定的流初始化链式块文件
    /// </summary>
    /// <param name="fromFile">指定的流</param>
    /// <param name="seekStart">流起始索引点</param>
    /// <param name="blockSize">块的大小</param>
    /// <returns>链式块文件</returns>
    public static LinkedFile InitializeInStream(Stream fromFile, long seekStart, int blockSize)
    {
      LinkedFile file = new LinkedFile(fromFile, seekStart, StoredConstants.LinkedFileHeaderPrefix, blockSize);
      file.WriteFileHeader();
      file.BlockFile = BlockFile.InitializeInStream(fromFile, seekStart + file.FileHeaderSize, StoredConstants.BlockFileHeaderPrefix, LinkedBlockOverhead + blockSize);
      return file;
    }

    /// <summary>
    /// 从指定的流初始化链式块文件
    /// </summary>
    /// <param name="fromFile">指定的流</param>
    /// <param name="seekStart">流起始索引点</param>
    /// <returns>链式块文件</returns>
    public static LinkedFile SetupFromExistingStream(Stream fromFile, long seekStart)
    {
      LinkedFile file = new LinkedFile(fromFile, seekStart, StoredConstants.LinkedFileHeaderPrefix, 100); // dummy block size for now
      file.ReadFileHeader();
      file.BlockFile = BlockFile.SetupFromExistingStream(fromFile, seekStart + file.FileHeaderSize, StoredConstants.BlockFileHeaderPrefix);
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

      this.IsHeaderDirty = false;
    }

    /// <summary>
    /// 读取块存储头部
    /// </summary>
    private void ReadFileHeader()
    {
      byte[] header = new byte[this.FileHeaderSize];

      this.Stream.Seek(this.SeekStart, SeekOrigin.Begin);
      this.Stream.Read(header, 0, this.FileHeaderSize);

      int index = 0;

      // 魔数前缀
      foreach (byte b in FileHeaderPrefix)
      {
        if (header[index] != b)
        {
          throw new LinkedFileException("invalid header prefix");
        }
        index++;
      }

      // 块的大小
      this.BlockSize = StoredHelper.RetrieveInt(header, index);
      index += StoredConstants.IntegerLength;

      // 空闲列表头序号
      this.FreeBlockHead = StoredHelper.RetrieveLong(header, index);
      index += StoredConstants.LongLength;

      this.IsHeaderDirty = false;

      if (this.BlockSize < StoredConstants.MinBlockSize)
      {
        throw new LinkedFileException("linked file block size too small " + this.BlockSize);
      }
    }

    /// <summary>
    /// 构造存储头
    /// </summary>
    /// <returns></returns>
    public byte[] MakeFileHeader()
    {
      // 魔数前缀长度 | 块的大小 | 空闲列表头序号
      byte[] header = new byte[this.FileHeaderSize];

      // 魔数前缀
      FileHeaderPrefix.CopyTo(header, 0);
      int index = FileHeaderPrefix.Length;

      // 块的大小
      StoredHelper.Store(this.BlockSize, header, index);
      index += StoredConstants.IntegerLength;

      // 空闲列表头序号
      StoredHelper.Store(this.FreeBlockHead, header, index);
      index += StoredConstants.LongLength;

      return header;
    }

    #endregion

    #region Chunk

    /// <summary>
    /// 存储新的大块数据，返回头序号。
    /// </summary>
    /// <param name="chunk">读取至此数组</param>
    /// <param name="startAt">读取起始点</param>
    /// <param name="length">读取长度</param>
    /// <returns>存储数据的头序号</returns>
    public long StoreChunk(byte[] chunk, int startAt, int length)
    {
      if (length < 0 || startAt < 0)
      {
        throw new LinkedFileException("cannot store negative length chunk (" + startAt + "," + length + ")");
      }

      // 分配用于存储的块 并标记为头块
      long currentBlockNumber = this.AllocateBlock();
      LinkedFileBlockFlag currentBlockFlag = LinkedFileBlockFlag.Head;

      // 存储数据的头序号
      long headBlockNumber = currentBlockNumber;

      int endAt = startAt + length;
      // special case: zero length chunk
      if (endAt > chunk.Length)
      {
        throw new LinkedFileException("array doesn't have this much data: " + endAt);
      }

      // store header with length information
      byte[] block = new byte[this.BlockSize];

      // 存储块长度
      StoredHelper.Store(length, block, 0);

      int fromIndex = startAt;
      int firstBlockLength = this.BlockSize - StoredConstants.IntegerLength;
      int storedLength = 0;
      if (firstBlockLength > length)
      {
        firstBlockLength = length;
      }

      // 存储数据
      Array.Copy(chunk, fromIndex, block, StoredConstants.IntegerLength, firstBlockLength);

      storedLength += firstBlockLength;
      fromIndex += firstBlockLength;

      // 存储剩余数据
      while (storedLength < length)
      {
        // 获取下一个块序号
        long nextBlockNumber = this.AllocateBlock();

        // 存储当前数据
        this.WriteBlock(currentBlockNumber, currentBlockFlag, block, 0, block.Length, nextBlockNumber);

        currentBlockNumber = nextBlockNumber;
        currentBlockFlag = LinkedFileBlockFlag.Body; // 下一个块则为Body

        int nextLength = this.BlockSize;
        if (storedLength + nextLength > length)
        {
          nextLength = length - storedLength;
        }

        Array.Copy(chunk, fromIndex, block, 0, nextLength);

        storedLength += nextLength;
        fromIndex += nextLength;
      }

      // 存储最终块
      this.WriteBlock(currentBlockNumber, currentBlockFlag, block, 0, block.Length, StoredConstants.NullBlockNumber);

      return headBlockNumber;
    }

    /// <summary>
    /// 获取指定头序号的大块数据
    /// </summary>
    /// <param name="headBlockNumber">指定头序号</param>
    /// <returns>大块数据</returns>
    public byte[] GetChunk(long headBlockNumber)
    {
      // get the head, interpret the length
      LinkedFileBlockFlag blockFlag;
      long nextBlockNumber;
      byte[] block = this.ReadBlock(headBlockNumber, out blockFlag, out nextBlockNumber);

      // 读取块长度
      int length = StoredHelper.RetrieveInt(block, 0);
      if (length < 0)
      {
        throw new LinkedFileException("negative length block? must be garbage: " + length);
      }

      if (blockFlag != LinkedFileBlockFlag.Head)
      {
        throw new LinkedFileException("first block not marked HEAD");
      }

      byte[] buffer = new byte[length];

      // 在第一个块中读取数据
      int firstLength = this.BlockSize - StoredConstants.IntegerLength;
      if (firstLength > length)
      {
        firstLength = length;
      }
      Array.Copy(block, StoredConstants.IntegerLength, buffer, 0, firstLength);

      // 在链接的其他块中读取数据
      int stored = firstLength;
      while (stored < length)
      {
        // get the next buffer
        long currentBlockNumber = nextBlockNumber;
        block = this.ReadBlock(currentBlockNumber, out blockFlag, out nextBlockNumber);

        int nextLength = this.BlockSize;
        if (length - stored < nextLength)
        {
          nextLength = length - stored;
        }

        Array.Copy(block, 0, buffer, stored, nextLength);
        stored += nextLength;
      }

      return buffer;
    }

    #endregion

    #region Allocate Block

    /// <summary>
    /// 分配块，如果有空闲块则分配空闲块，否则分配新块。
    /// </summary>
    /// <returns>块序号</returns>
    private long AllocateBlock()
    {
      if (this.FreeBlockHead != StoredConstants.NullBlockNumber)
      {
        // 使用空闲块
        long blockNumber = this.FreeBlockHead;

        LinkedFileBlockFlag blockFlag;
        long nextFreeBlockNumber;
        this.ReadBlock(blockNumber, out blockFlag, out nextFreeBlockNumber);

        if (blockFlag != LinkedFileBlockFlag.Free)
        {
          throw new LinkedFileException("free head block not marked free");
        }

        this.FreeBlockHead = nextFreeBlockNumber;
        this.IsHeaderDirty = true;
        this.RecentNewBlockNumber = StoredConstants.NullBlockNumber;

        return blockNumber;
      }
      else
      {
        // 分配新块
        long nextBlockNumber = this.BlockFile.NextBlockNumber();

        if (this.RecentNewBlockNumber == nextBlockNumber)
        {
          // the previous block has been allocated but not yet written. It must be written before the following one...
          nextBlockNumber++;
        }
        this.RecentNewBlockNumber = nextBlockNumber;

        return nextBlockNumber;
      }
    }

    /// <summary>
    /// 回收再利用指定序号的块
    /// </summary>
    /// <param name="blockNumber">指定序号</param>
    private void ReclaimBlock(long blockNumber)
    {
      // 将指定块置成空闲
      this.WriteBlock(blockNumber, LinkedFileBlockFlag.Free, null, 0, 0, this.FreeBlockHead);

      this.FreeBlockHead = blockNumber;
      this.IsHeaderDirty = true;
    }

    #endregion

    #region Read & Write Block

    /// <summary>
    /// 写入块
    /// </summary>
    /// <param name="blockNumber">块序号</param>
    /// <param name="blockFlag">块标记</param>
    /// <param name="fromArray">读取至此数组</param>
    /// <param name="startAt">读取起始点</param>
    /// <param name="length">读取长度</param>
    /// <param name="nextBlockNumber">链接的下一个块</param>
    private void WriteBlock(long blockNumber, LinkedFileBlockFlag blockFlag, byte[] fromArray, int startAt, int length, long nextBlockNumber)
    {
      if (this.BlockSize < length)
      {
        throw new LinkedFileException("block size too small " + this.BlockSize + "<" + length);
      }

      byte[] buffer = new byte[LinkedBlockOverhead + length];

      // 写入块标记
      buffer[0] = (byte)blockFlag; // 1 Byte

      // 写入链接的下一个块的序号
      StoredHelper.Store(nextBlockNumber, buffer, 1); // 8 Bytes

      // 写入数据
      if (fromArray != null)
      {
        Array.Copy(fromArray, startAt, buffer, LinkedBlockOverhead, length);
      }

      this.BlockFile.WriteBlock(blockNumber, buffer, 0, buffer.Length);
    }

    /// <summary>
    /// 读取块
    /// </summary>
    /// <param name="blockNumber">块序号</param>
    /// <param name="blockFlag">块标记</param>
    /// <param name="nextBlockNumber">链接的下一个块</param>
    /// <returns>块数据</returns>
    private byte[] ReadBlock(long blockNumber, out LinkedFileBlockFlag blockFlag, out long nextBlockNumber)
    {
      byte[] fullBuffer = new byte[LinkedBlockOverhead + this.BlockSize];
      this.BlockFile.ReadBlock(blockNumber, fullBuffer, 0, fullBuffer.Length);

      // 读取块标记
      blockFlag = (LinkedFileBlockFlag)fullBuffer[0];

      // 读取链接的下一个块的序号
      nextBlockNumber = StoredHelper.RetrieveLong(fullBuffer, 1);

      // 读取数据
      byte[] buffer = new byte[this.BlockSize];
      Array.Copy(fullBuffer, LinkedBlockOverhead, buffer, 0, this.BlockSize);

      return buffer;
    }

    #endregion

    #region Control

    /// <summary>
    /// 根据外部记录还原文件
    /// </summary>
    /// <param name="chunksInUse">若指定使用中的块列表，则逐个匹配确认是否存在对应的块</param>
    /// <param name="throwExceptionWhenNotMatch">当发现未匹配的使用块时，是否抛出异常</param>
    public void Recover(IDictionary<long, string> chunksInUse, bool throwExceptionWhenNotMatch)
    {
      // find missing space and recover it
      this.CheckStructure(chunksInUse, throwExceptionWhenNotMatch);
    }

    /// <summary>
    /// 检查文件结构
    /// </summary>
    /// <param name="chunksInUse">若指定使用中的块列表，则逐个匹配确认是否存在对应的块</param>
    /// <param name="throwExceptionWhenNotMatch">当发现未匹配的使用块时，是否抛出异常</param>
    private void CheckStructure(IDictionary<long, string> chunksInUse, bool throwExceptionWhenNotMatch)
    {
      Hashtable blockNumberToFlag = new Hashtable();
      Hashtable blockNumberToNext = new Hashtable();
      Hashtable visited = new Hashtable();

      long lastBlockNumber = this.BlockFile.NextBlockNumber();
      for (long number = 0; number < lastBlockNumber; number++)
      {
        LinkedFileBlockFlag blockFlag;
        long nextBufferNumber;
        this.ReadBlock(number, out blockFlag, out nextBufferNumber);

        blockNumberToFlag[number] = blockFlag;
        blockNumberToNext[number] = nextBufferNumber;
      }

      // traverse the freelist
      long currentFreeBlockNumber = this.FreeBlockHead;
      while (currentFreeBlockNumber != StoredConstants.NullBlockNumber)
      {
        if (visited.ContainsKey(currentFreeBlockNumber))
        {
          throw new LinkedFileException("cycle in free list " + currentFreeBlockNumber);
        }

        visited[currentFreeBlockNumber] = currentFreeBlockNumber;

        LinkedFileBlockFlag blockFlag = (LinkedFileBlockFlag)blockNumberToFlag[currentFreeBlockNumber];
        long nextBlockNumber = (long)blockNumberToNext[currentFreeBlockNumber];

        if (blockFlag != LinkedFileBlockFlag.Free)
        {
          throw new LinkedFileException("free list element not marked free " + currentFreeBlockNumber);
        }

        currentFreeBlockNumber = nextBlockNumber;
      }

      // traverse all nodes marked head
      Hashtable allChunks = new Hashtable();
      for (long number = 0; number < lastBlockNumber; number++)
      {
        LinkedFileBlockFlag blockFlag = (LinkedFileBlockFlag)blockNumberToFlag[number];
        if (blockFlag == LinkedFileBlockFlag.Head)
        {
          if (visited.ContainsKey(number))
          {
            throw new LinkedFileException("head buffer already visited " + number);
          }

          allChunks[number] = number;
          visited[number] = number;

          long bodyBlockNumber = (long)blockNumberToNext[number];
          while (bodyBlockNumber != StoredConstants.NullBlockNumber)
          {
            LinkedFileBlockFlag bodyBlockFlag = (LinkedFileBlockFlag)blockNumberToFlag[bodyBlockNumber];
            long nextBlockNumber = (long)blockNumberToNext[bodyBlockNumber];

            if (visited.ContainsKey(bodyBlockNumber))
            {
              throw new LinkedFileException("body block visited twice " + bodyBlockNumber);
            }

            visited[bodyBlockNumber] = bodyBlockFlag;

            if (bodyBlockFlag != LinkedFileBlockFlag.Body)
            {
              throw new LinkedFileException("body block not marked body " + blockFlag);
            }

            bodyBlockNumber = nextBlockNumber;
          }

          // check retrieval
          this.GetChunk(number);
        }
      }

      // make sure all were visited
      for (long number = 0; number < lastBlockNumber; number++)
      {
        if (!visited.ContainsKey(number))
        {
          throw new LinkedFileException("block not found either as data or free " + number);
        }
      }

      // check against in use list
      if (chunksInUse != null)
      {
        ArrayList notInUse = new ArrayList();

        foreach (var d in chunksInUse)
        {
          long blockNumber = (long)d.Key;
          if (!allChunks.ContainsKey(blockNumber))
          {
            throw new LinkedFileException("block in used list not found in linked file " + blockNumber + " " + d.Value);
          }
        }
        foreach (DictionaryEntry d in allChunks)
        {
          long blockNumber = (long)d.Key;
          if (!chunksInUse.ContainsKey(blockNumber))
          {
            if (!throwExceptionWhenNotMatch)
            {
              throw new LinkedFileException("block in linked file not in used list " + blockNumber);
            }
            notInUse.Add(blockNumber);
          }
        }

        notInUse.Sort();
        notInUse.Reverse();

        foreach (object item in notInUse)
        {
          long blockNumber = (long)item;
          this.ReleaseBlocks(blockNumber);
        }
      }
    }

    /// <summary>
    /// 释放指定头序号链接的所有块
    /// </summary>
    /// <param name="headBlockNumber">指定头序号</param>
    public void ReleaseBlocks(long headBlockNumber)
    {
      long currentBlockNumber = headBlockNumber;

      long nextBlockNumber;
      LinkedFileBlockFlag blockFlag;
      this.ReadBlock(headBlockNumber, out blockFlag, out nextBlockNumber);

      if (blockFlag != LinkedFileBlockFlag.Head)
      {
        throw new LinkedFileException("head block not marked HEAD");
      }

      this.ReclaimBlock(headBlockNumber);

      while (nextBlockNumber != StoredConstants.NullBlockNumber)
      {
        currentBlockNumber = nextBlockNumber;
        this.ReadBlock(currentBlockNumber, out blockFlag, out nextBlockNumber);

        if (blockFlag != LinkedFileBlockFlag.Body)
        {
          throw new LinkedFileException("body block not marked BODY");
        }

        this.ReclaimBlock(currentBlockNumber);
      }
    }

    /// <summary>
    /// 刷新缓冲区
    /// </summary>
    public void Flush()
    {
      if (this.IsHeaderDirty)
      {
        this.WriteFileHeader();
      }

      this.BlockFile.Flush();
    }

    /// <summary>
    /// 关闭文件操作
    /// </summary>
    public void Close()
    {
      this.Stream.Flush();
      this.Stream.Close();
    }

    #endregion
  }
}
