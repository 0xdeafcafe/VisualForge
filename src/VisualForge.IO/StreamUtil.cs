using System;

namespace VisualForge.IO
{
    public sealed class StreamUtil
    {
        public static void Copy(IReader input, IWriter output)
        {
            // http://stackoverflow.com/questions/230128/best-way-to-copy-between-two-stream-instances-c-sharp
            const int BufferSize = 0x1000;
            var buffer = new byte[BufferSize];
            int read;
            while ((read = input.ReadBlock(buffer, 0, BufferSize)) > 0)
                output.WriteBlock(buffer, 0, read);
        }

        public static void Copy(IReader input, IWriter output, int size)
        {
            const int BufferSize = 0x1000;
			var buffer = new byte[BufferSize];
            while (size > 0)
            {
				var read = input.ReadBlock(buffer, 0, Math.Max(BufferSize, size));
                output.WriteBlock(buffer, 0, read);
                size -= BufferSize;
            }
        }
    }
}      
