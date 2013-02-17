using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace VisualForge.Core.Helpers
{
	public static class VariousFunctions
	{
		public static byte[] GetTaglistFile(string gameId, int mapId)
		{
			var taglistPath = GetApplicationLocation() + string.Format(@"\Content\{0}\Taglists\{1}.vftl", gameId, mapId);
			if (!File.Exists(taglistPath))
				throw new FileNotFoundException("Taglist for the specified map is missing");

			return File.ReadAllBytes(taglistPath);
		}
		public static string GetGameAsset(string gameId, string tagFileName)
		{
			var assetPath = GetApplicationLocation() + string.Format(@"\Content\{0}\Assets\{1}.obj", gameId, tagFileName);
			if (!File.Exists(assetPath))
				throw new FileNotFoundException("Game Asset for the specified tag is missing");

			return assetPath;
		}

		/// <summary>
		/// Gets the parent directory of the application's exe
		/// </summary>
		public static string GetApplicationLocation()
		{
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		/// <summary>
		/// Gets the location of the applications assembly (lulz, assembly.exe)
		/// </summary>
		public static string GetApplicationAssemblyLocation()
		{
			return Assembly.GetExecutingAssembly().Location;
		}


		public enum EncodingType
		{
			ASCII,
			Unicode,
			UTF7,
			UTF8
		}
		/// <summary> 
		/// Converts a byte array to a string using specified encoding. 
		/// </summary> 
		/// <param name="bytes">Array of bytes to be converted.</param> 
		/// <param name="encodingType">EncodingType enum.</param> 
		public static string ByteArrayToString(byte[] bytes, EncodingType encodingType)
		{
			Encoding encoding;
			switch (encodingType)
			{
				case EncodingType.ASCII:
					encoding = new ASCIIEncoding();
					break;
				case EncodingType.Unicode:
					encoding = new UnicodeEncoding();
					break;
				case EncodingType.UTF7:
					encoding = new UTF7Encoding();
					break;
				case EncodingType.UTF8:
					encoding = new UTF8Encoding();
					break;

				default:
					encoding = new ASCIIEncoding();
					break;
			}
			return encoding.GetString(bytes);
		} 

		public class GZip
		{
			public static byte[] Decompress(byte[] input)
			{
				using (var stream = new GZipStream(new MemoryStream(input), CompressionMode.Decompress))
				{
					const int size = 4096;
					var buffer = new byte[size];
					using (var memory = new MemoryStream())
					{
						int count;
						do
						{
							count = stream.Read(buffer, 0, size);
							if (count > 0)
							{
								memory.Write(buffer, 0, count);
							}
						}
						while (count > 0);
						return memory.ToArray();
					}
				}

			}

			public static byte[] Compress(byte[] input)
			{
				byte[] b;
				using (Stream f = new MemoryStream(input))
				{
					b = new byte[f.Length];
					f.Read(b, 0, (int)f.Length);
				}

				using (Stream f2 = new MemoryStream(input))
				using (var gz = new GZipStream(f2, CompressionMode.Compress, false))
				{
					gz.Write(b, 0, b.Length);
				}

				return b;
			}
		}
	}
}
