using System.IO;
namespace VisualForge.Usermaps
{
	public interface IUsermap
	{
		void Initalize(Stream fileStream);

		// Loading Code
		void LoadHeader();
		void LoadTags();
		void LoadItemPlacement();
		void LoadTagEntrys();

		// Updateing Code
		void Update();
		void UpdateHeader();
		void UpdateItemPlacement();
		void UpdateTagEntries();

		void Close();
	}
}