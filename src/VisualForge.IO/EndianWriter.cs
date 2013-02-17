using System;
using System.Text;
using System.IO;

namespace VisualForge.IO
{
    /// <summary>
    /// Writes binary data to a stream.
    /// The endianness can be dynamically changed.
    /// </summary>
    public class EndianWriter : IWriter
    {
	    /// <summary>
	    /// Constructs a new EndianWriter.
	    /// </summary>
	    /// <param name="stream">The stream to write to.</param>
		/// <param name="endianness">The endianness to use when performing I/O operations.</param>
	    public EndianWriter(Stream stream, Endian endianness)
        {
            _stream = stream;
            _bigEndian = (endianness == Endian.BigEndian);
        }

        /// <summary>
        /// The endianness to use when writing to the stream.
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
        /// Closes the underlying stream.
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        /// <summary>
        /// Writes a byte to the underlying stream.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        public void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a signed byte to the underlying stream.
        /// </summary>
        /// <param name="value">The signed byte to write.</param>
        public void WriteSByte(sbyte value)
        {
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes an unsigned 16-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt16(ushort value)
        {
            if (_bigEndian)
            {
                _stream.WriteByte((byte)(value >> 8));
                _stream.WriteByte((byte)(value & 0xFF));
            }
            else
            {
                _stream.WriteByte((byte)(value & 0xFF));
                _stream.WriteByte((byte)(value >> 8));
            }
        }

        /// <summary>
        /// Writes a signed 16-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt16(short value)
        {
            WriteUInt16((ushort)value);
        }

        /// <summary>
        /// Writes an signed 24-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value"></param>
        public void WriteInt24(int value)
        {
            if (_bigEndian)
            {
                _stream.WriteByte((byte)(value >> 16));
                _stream.WriteByte((byte)(value >> 8));
                _stream.WriteByte((byte)(value));
            }
            else
            {
                _stream.WriteByte((byte)(value >> 16));
                _stream.WriteByte((byte)(value >> 8));
                _stream.WriteByte((byte)(value));
            }
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt32(uint value)
        {
            if (_bigEndian)
            {
                _stream.WriteByte((byte)(value >> 24));
                _stream.WriteByte((byte)((value >> 16) & 0xFF));
                _stream.WriteByte((byte)((value >> 8) & 0xFF));
                _stream.WriteByte((byte)(value & 0xFF));
            }
            else
            {
                _stream.WriteByte((byte)(value & 0xFF));
                _stream.WriteByte((byte)((value >> 8) & 0xFF));
                _stream.WriteByte((byte)((value >> 16) & 0xFF));
                _stream.WriteByte((byte)(value >> 24));
            }
        }

        /// <summary>
        /// Writes a signed 32-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt32(int value)
        {
            WriteUInt32((uint)value);
        }

        /// <summary>
        /// Writes an unsigned 64-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt64(ulong value)
        {
            if (_bigEndian)
            {
                _stream.WriteByte((byte)(value >> 56));
                _stream.WriteByte((byte)((value >> 48) & 0xFF));
                _stream.WriteByte((byte)((value >> 40) & 0xFF));
                _stream.WriteByte((byte)((value >> 32) & 0xFF));
                _stream.WriteByte((byte)((value >> 24) & 0xFF));
                _stream.WriteByte((byte)((value >> 16) & 0xFF));
                _stream.WriteByte((byte)((value >> 8) & 0xFF));
                _stream.WriteByte((byte)(value & 0xFF));
            }
            else
            {
                _stream.WriteByte((byte)(value & 0xFF));
                _stream.WriteByte((byte)((value >> 8) & 0xFF));
                _stream.WriteByte((byte)((value >> 16) & 0xFF));
                _stream.WriteByte((byte)((value >> 24) & 0xFF));
                _stream.WriteByte((byte)((value >> 32) & 0xFF));
                _stream.WriteByte((byte)((value >> 40) & 0xFF));
                _stream.WriteByte((byte)((value >> 48) & 0xFF));
                _stream.WriteByte((byte)(value >> 56));
            }
        }

        /// <summary>
        /// Writes an signed 64-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt64(long value)
        {
            WriteUInt64((ulong)value);
        }

        /// <summary>
        /// Writes a 32-bit float value to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian == _bigEndian)
            {
                // Is there a faster way to do this?
                byte temp = bytes[0];
                bytes[0] = bytes[3];
                bytes[3] = temp;
                temp = bytes[1];
                bytes[1] = bytes[2];
                bytes[2] = temp;
            }
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a null-terminated ASCII string to the underlying stream.
        /// </summary>
        /// <param name="str">The string to write.</param>
        public void WriteAscii(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            WriteBlock(bytes);
            WriteByte(0);
        }

        /// <summary>
        /// Writes a null-terminated UTF-16 encoded string to the underlying stream.
        /// </summary>
        /// <param name="str">The string to write.</param>
        public void WriteUTF16(string str)
        {
            foreach (char ch in str)
                WriteInt16((short)ch);
            WriteInt16(0x0000);
        }

        /// <summary>
        /// Writes an array of bytes to the underlying stream.
        /// </summary>
        /// <param name="data">The array of bytes to write.</param>
        public void WriteBlock(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes part of an array of bytes to the underlying stream.
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        /// <param name="offset">The offset in the array to start reading from.</param>
        /// <param name="size">The maximum number of bytes to write.</param>
        public void WriteBlock(byte[] data, int offset, int size)
        {
            _stream.Write(data, offset, size);
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
        /// The stream that this EndianWriter is based off of.
        /// </summary>
        public Stream BaseStream
        {
            get { return _stream; }
        }

        private readonly Stream _stream;
        private bool _bigEndian;
    }
}      
