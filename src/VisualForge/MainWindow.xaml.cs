using System.Linq;
using Microsoft.Win32;
using System.Windows;
using VisualForge.Usermaps.Games;

namespace VisualForge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void btnOpenSandbox_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog();
			if ((bool)ofd.ShowDialog())
			{
				var h3 = new Halo3(ofd.FileName);

				foreach (var tag in h3.SandboxTagEntries.Where(tag => tag.PlacedObjects != null && tag.PlacedObjects.Count > 0))
				{

				}
			}
		}
	}
}
