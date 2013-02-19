using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using VisualForge.Usermaps.Games;

namespace VisualForge.XNA
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			if (args != null && args.Length > 0 && File.Exists(args[0]))
				MessageBox.Show("feature not yet implimented.");


	        var ofd = new OpenFileDialog
		                  {
							  Title = "Visual Forge - Select a Usermap",
							  Filter = "Xbox 360 Container File|*|Sandbox BLF file (sandbox.map)|*.map|All Files|*",
#if DEBUG
							  InitialDirectory = @"C:\Users\Alex Reed\Desktop\r-search\"
#endif
		                  };
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				var target = GetTarget(ofd.FileName);

				switch(target)
				{
					case TargetGame.Halo3:
						using (var game = new Halo3(ofd.FileName))
						{
							game.Run();
						}
						break;

					default: 
						throw new InvalidFilterCriteriaException("The selected file is not from a supported game. Current Visual Forge only supports Halo 3.");
				}
			}
        }

		public enum TargetGame
		{
			Halo3
		}

		public static TargetGame GetTarget(string filePath)
		{
			return TargetGame.Halo3;
		}
    }
#endif
}

