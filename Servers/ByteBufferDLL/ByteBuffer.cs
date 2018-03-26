using System;
using System.Collections.Generic;
using System.Text;

namespace ByteBufferDLL
{
	public class ByteBuffer : IDisposable
	{
		List<byte> buffer; //store buffer as list to do not hit size limit
		byte[] readbuffer; //the list will later be compressed to this array
		int readpos;
		bool bufferUpdate = false; //"Notifies" that something have been written to the buffer

		public ByteBuffer()
		{
			buffer = new List<byte>();
			readpos = 0;

		}

		public int getPos()
		{
			return readpos;
		}

		public byte[] ToArray()
		{
			return buffer.ToArray();
		}

		public int Size()
		{
			return buffer.Count;
		}

		public int Length() //Returns the length of the data to be sent
		{
			return Size() - readpos;
		}

		public void Clear() //Clears the buffer and returns readposition to the begining
		{
			buffer.Clear();
			readpos = 0;
		}
		#region "Wrinting data"
		/*public void WriteObjects(Object o, int length)
		{
			WriteInt(length);
			buffer.AddRange(BitConverter.GetBytes(o));
			BitConverter.GetBytes()
		}*/

		public void WriteByte(byte input)
		{
			buffer.Add(input);
			bufferUpdate = true;
		}

		public void WriteBytes(byte[] input)
		{
			buffer.AddRange(input);
			bufferUpdate = true;
		}

		public void WriteShort(short input) // As the method for writing short and ints are the same, just call the one to write short
		{
			buffer.AddRange(BitConverter.GetBytes(input));
			bufferUpdate = true;
		}

		public void WriteInt(int input) 
		{
			buffer.AddRange(BitConverter.GetBytes(input));
			bufferUpdate = true;
		}

		public void WriteFloat(float input)
		{
			buffer.AddRange(BitConverter.GetBytes(input));
			bufferUpdate = true;
		}

		public void WriteString(string input)
		{
			buffer.AddRange(BitConverter.GetBytes(input.Length));
			buffer.AddRange(Encoding.ASCII.GetBytes(input));
			bufferUpdate = true;
		}
		#endregion

		#region "Reading Data"

		public int ReadInt(bool peek = true)
		{
			if (buffer.Count > readpos)
			{
				if (bufferUpdate)
				{
					readbuffer = buffer.ToArray();
					bufferUpdate = false;
				}

				int ret = BitConverter.ToInt32(readbuffer, readpos); // Reads 4 bytes from a given starting point and converts to int

				if (peek && buffer.Count > readpos)
					readpos += 4;
				return ret;
			}
			else
			{
				throw new Exception("Buffer is past its limit");
			}
		}

		public String ReadString(bool peek = true)
		{
			int length = ReadInt(); // the size of the string is written to the buffer first
									// So now it's getting how many bytes is needed to read to retrieve the string

			if (bufferUpdate)
			{
				readbuffer = buffer.ToArray();
				bufferUpdate = false;
			}

			string readString = Encoding.ASCII.GetString(readbuffer, readpos, length);
			if (peek && buffer.Count > readpos)
				readpos += length;

			return readString;
		}

		public byte ReadByte(bool peek = true)
		{
			if (buffer.Count > readpos)
			{
				if (bufferUpdate)
				{
					readbuffer = buffer.ToArray();
					bufferUpdate = false;
				}

				byte ret = readbuffer[readpos]; 

				if (peek && buffer.Count > readpos)
					readpos += 1;
				return ret;
			}
			else
			{
				throw new Exception("Buffer is past its limit");
			}
		}

		public byte[] ReadBytes(int Length, bool peek = true)
		{
			if (bufferUpdate)
			{
				readbuffer = buffer.ToArray();
				bufferUpdate = false;
			}

			byte[] readBytes = buffer.GetRange(readpos, Length).ToArray();

			if (peek)
				readpos += Length;

			return readBytes;
		}

		public float ReadFloat(bool peek = true)
		{
			if (buffer.Count > readpos)
			{
				if (bufferUpdate)
				{
					readbuffer = buffer.ToArray();
					bufferUpdate = false;
				}

				float ret = BitConverter.ToSingle(readbuffer, readpos); // Reads 4 bytes from a given starting point and converts to int

				if (peek && buffer.Count > readpos)
					readpos += 4;
				return ret;
			}
			else
			{
				throw new Exception("Buffer is past its limit");
			}
		}


		#endregion

		bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
				if (disposing)
				{
					buffer.Clear();
				}
			readpos = 0;
			this.disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
