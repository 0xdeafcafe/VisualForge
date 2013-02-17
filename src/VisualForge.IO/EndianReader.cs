using System;
using System.IO;

namespace VisualForge.IO
{
    /// <summary>
    /// Reads big-endian data from a stream.
    /// Designed to support the various types that Bungie uses.
    /// </summary>
    public class EndianReader : IDisposable, IReader
    {
        /// <summary>
        /// Constructs a new EndianReader object given a base stream and an initial endianness.
        /// </summary>
		/// <param name="stream">The stream that data should be read from.</param>
        /// <param name="endianness">The endianness that should be used.</param>
        public EndianReader(Stream stream, Endian endianness)
        {
            _stream = stream;
            _bigEndian = (endianness == Endian.BigEndian);
        }

        /// <summary>
        /// The endianness to use when reading from the stream.
        /// </summary>
        public Endian Endianness
        {
            get
            {
                return _bigEndian ? Endian.BigEndian : Endian.LittleEndian;
            }
            set
            {
                _bigEndian = (value == Endian.BigEndian);
            }
        }

        /// <summary>
        /// Closes this EndianReader and the underlying stream.
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        /// <summary>
        /// Disposes of this stream.
        /// <seealso cref="Close"/>
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Reads a byte from the underlying stream and advances its position by 1.
        /// </summary>
        public byte ReadByte()
        {
            return (byte)_stream.ReadByte();
        }

        /// <summary>
        /// Reads a signed byte from the underlying stream and advances its position by 1.
        /// </summary>
        public sbyte ReadSByte()
        {
            return (sbyte)_stream.ReadByte();
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from the underlying stream and advances its position by 2.
        /// </summary>
        public ushort ReadUInt16()
        {
            _stream.Read(_buffer, 0, 2);
            if (_bigEndian)
                return (ushort)((_buffer[0] << 8) | _buffer[1]);
	        return (ushort)((_buffer[1] << 8) | _buffer[0]);
        }

        /// <summary>
        /// Reads a 16-bit signed integer from the underlying stream and advances its position by 2.
        /// </summary>
        public short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        /// <summary>
        /// Reads a 24-bit signed integer from the underlying stream and advances it position by 3.
        /// </summary>
        public int ReadInt24()
        {
            _stream.Read(_buffer, 0, 3);
            if (_bigEndian)
                return (_buffer[0] << 16) + (_buffer[1] << 8) + (_buffer[2]);
            return (_buffer[3] << 16) + (_buffer[2] << 8) + (_buffer[1]);
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the underlying stream and advances its position by 4.
        /// </summary>
        public uint ReadUInt32()
        {
            _stream.Read(_buffer, 0, 4);
            if (_bigEndian)
                return (uint)((_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3]);
            return (uint)((_buffer[3] << 24) | (_buffer[2] << 16) | (_buffer[1] << 8) | _buffer[0]);
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the underlying stream and advances its position by 4.
        /// </summary>
        public int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer from the underlying stream and advances its position by 8.
        /// </summary>
        /// <returns>The value that was read</returns>
        public ulong ReadUInt64()
        {
            /*_stream.Read(_buffer, 0, 8);
            return (ulong)((_buffer[0] << 56) | (_buffer[1] << 48) | (_buffer[2] << 40) | (_buffer[3] << 32) |
                           (_buffer[4] << 24) | (_buffer[5] << 16) | (_buffer[6] << 8) | _buffer[7]);*/
            var one = (ulong)ReadUInt32();
			var two = (ulong)ReadUInt32();
            if (_bigEndian)
                return (one << 32) | two;
            return (two << 32) | one;
        }

        /// <summary>
        /// Reads a 64-bit signed integer from the underlying stream and advances its position by 8.
        /// </summary>
        /// <returns>The value that was read</returns>
        public long ReadInt64()
        {
            /*_stream.Read(_buffer, 0, 8);
            return (long)((_buffer[0] << 56) | (_buffer[1] << 48) | (_buffer[2] << 40) | (_buffer[3] << 32) |
                          (_buffer[4] << 24) | (_buffer[5] << 16) | (_buffer[6] << 8) | _buffer[7]);*/
            return (long)ReadUInt64();
        }

        /// <summary>
        /// Reads a 32-bit float from the underlying stream and advances its position by 4.
        /// </summary>
        /// <returns>The float value that was read.</returns>
        public float ReadFloat()
        {
            _stream.Read(_buffer, 0, 4);
            if (BitConverter.IsLittleEndian == _bigEndian)
            {
                // Flip the bytes
                // Is there a faster way to do this?
                byte temp = _buffer[0];
                _buffer[0] = _buffer[3];
                _buffer[3] = temp;
                temp = _buffer[1];
                _buffer[1] = _buffer[2];
                _buffer[2] = temp;
            }
            return BitConverter.ToSingle(_buffer, 0);
        }

        /// <summary>
        /// Changes the position of the underlying stream.
        /// </summary>
        /// <param name="offset">The new offset.</param>       
        public void SeekTo(long offset)
        {
            _stream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Skips over a number of bytes in the underlying stream.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        public void Skip(long count)
        {
            _stream.Seek(count, SeekOrigin.Current);
        }

        /// <summary>
        /// Reads a null-terminated ASCII string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public string ReadAscii()
        {
			var result = "";
	        while (true)
            {
				var ch = _stream.ReadByte();
                if (ch == 0 || ch == -1)
                    break;
                result += (char)ch;
            }
            return result;
        }

        /// <summary>
        /// Reads an ASCII string of a specific size. Null terminators will be taken into account.
        /// The position of the underlying stream will be advanced by the string size.
        /// </summary>
        /// <param name="size">The size of the string to be read.</param>
        /// <returns>The string that was read.</returns>
        public unsafe string ReadAscii(int size)
        {
            var chars = new sbyte[size];
            string result;
            fixed (sbyte* str = chars)
            {
// ReSharper disable PossibleInvalidCastException
                _stream.Read((byte[])(Array)chars, 0, size);
// ReSharper restore PossibleInvalidCastException
                result = new string(str);
            }
            return result;
        }

        /// <summary>
        /// Reads a null-terminated UTF16-encoded string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public string ReadUTF16()
        {
			var result = "";
	        while (true)
            {
                int ch = ReadInt16();
                if (ch == 0)
                    break;
                result += (char)ch;
            }
            return result;
        }

        public string ReadUTF16(int length)
        {
			var result = "";
	        int i;
            for (i = 0; i < length; i++)
            {
                int ch = ReadInt16();
                if (ch == 0)
                    break;
                result += (char)ch;
            }
            if (i < length)
                Skip((length - i - 1) * 2);
            return result;
        }

        /// <summary>
        /// Reads a block of data from the stream and returns it.
        /// </summary>
        /// <param name="size">The number of bytes to read</param>
        /// <returns>The bytes that were read</returns>
        public byte[] ReadBlock(int size)
        {
			var result = new byte[size];
            _stream.Read(result, 0, size);
            return result;
        }

        /// <summary>
        /// Reads a block of data from the stream into an existing array.
        /// </summary>
        /// <param name="output">The array to read data into</param>
        /// <param name="offset">The array index to start reading to</param>
        /// <param name="size">The number of bytes to read</param>
        /// <returns>The number of bytes actually read</returns>
        public int ReadBlock(byte[] output, int offset, int size)
        {
            return _stream.Read(output, offset, size);
        }

        /// <summary>
        /// Reads a block of data from the stream into an existing array.
        /// </summary>
        /// <param name="output">The array to read data into</param>
        /// <param name="size">The number of bytes to read</param>
        /// <returns>The number of bytes actually read</returns>
        public int ReadBlock(byte[] output, int size)
        {
            return _stream.Read(output, 0, size);
        }

        /// <summary>
        /// Returns whether or not we are at the end of the stream.
        /// </summary>
        public bool EOF
        {
            get
            {
                return (Position >= Length);
            }
        }

        /// <summary>
        /// Returns the current position of the reader.
        /// </summary>
        public long Position
        {
            get
            {
                return _stream.Position;
            }
        }

        /// <summary>
        /// Returns the total length of the stream.
        /// </summary>
        public long Length
        {
            get
            {
                return _stream.Length;
            }
        }

        /// <summary>
        /// The stream that this EndianReader is based off of.
        /// </summary>
        public Stream BaseStream
        {
            get { return _stream; }
        }

        private readonly Stream _stream;
        private readonly byte[] _buffer = new byte[8];
        private bool _bigEndian;
    }
}      
