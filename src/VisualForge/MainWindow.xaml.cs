using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.Windows;
using VisualForge.Usermaps.Games;
using HelixToolkit.Wpf;

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

			ActionLog = new Log(txtLog);
		}
		private Log ActionLog;

		private void btnOpenSandbox_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog();
			if ((bool)ofd.ShowDialog())
			{
				var h3 = new Halo3(ofd.FileName);

				var container = new Model3DCollection();
				foreach (var placedObject in h3.SandboxObjects.Where(placedObject => placedObject.TagIndex != -1))
				{
					// Try to load Model
					string gameAssetPath;
					try
					{
						gameAssetPath = Core.Helpers.VariousFunctions.GetGameAsset(Halo3.GameId, placedObject.TagEntry.Tag.TagPath);
					}
					catch (FileNotFoundException ex)
					{
						ActionLog.AddEntry("Missing Game Asset");
						continue;
					}

					var model = ModelImporter.Load(gameAssetPath);
					model.Transform = new TranslateTransform3D(
						placedObject.SpawnCoordinates.X,
						placedObject.SpawnCoordinates.Y,
						placedObject.SpawnCoordinates.Z);

					container.Add(model);
				}

				foreach(var model in container)
					ModelViewport.Children.Add(new ModelVisual3D()
						                           {
							                           Content = model
						                           });

				ModelViewport.ShowCameraInfo = true;
				ModelViewport.ZoomExtents();
			}
		}

		public class Log
		{
			public Log(TextBox textbox)
			{
				_textbox = textbox;
				AddEntry("Welcome to Visual Forge!");
			}
			private readonly TextBox _textbox;

			public void AddEntry(string content)
			{
				var timestamp = string.Format("[{0}]", DateTime.Now.ToString("HH:mm:ss.ffff"));

				_textbox.Text += string.Format("{0} - {1}\r\n", timestamp, content);
			}
		}
	}
}
