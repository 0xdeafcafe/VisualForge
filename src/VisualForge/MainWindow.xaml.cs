using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.Windows;
using VisualForge.Usermaps.Games;
using HelixToolkit.Wpf;
using VisualForge.Core.Helpers;

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
				lblSandboxPath.Text = ofd.FileName;

				var container = new Model3DCollection();
				foreach (var placedObject in h3.SandboxObjects.Where(placedObject => placedObject.TagIndex != -1))
				{
					// Try to load Model
					string gameAssetPath;
					try
					{
						gameAssetPath = Core.Helpers.VariousFunctions.GetGameAsset(Halo3.GameId, placedObject.TagEntry.Tag.TagPath);
					}
					catch (FileNotFoundException)
					{
						ActionLog.AddEntry("Missing Game Asset");
						continue;
					}

					var model = ModelImporter.Load(gameAssetPath);
					model.Transform = CreateTransformGroup(placedObject.SpawnCoordinates);
					container.Add(model);
				}

				ModelViewport.Children.Clear();
				foreach (var model in container)
				{
					ModelViewport.Children.Add(new ModelVisual3D
						                           {
							                           Content = model
						                           });
				}

				ModelViewport.ShowCameraInfo = true;
				ModelViewport.ZoomExtents();
			}
		}

		public Transform3DGroup CreateTransformGroup(Halo3.ObjectChunk.Coordinates objectCoordinates)
		{
			var transformGroup = new Transform3DGroup();

			// X, Y, Z Coordinates
			transformGroup.Children.Add(new TranslateTransform3D(
											objectCoordinates.X,
											objectCoordinates.Y,
											objectCoordinates.Z
										));

			// Roll
			transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(
											new Vector3D(1, 0, 0), 90)));

			// Pitch
			transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(
											new Vector3D(0, 1, 0), objectCoordinates.Pitch)));

			// Yaw
			transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(
											new Vector3D(0, 0, 1), objectCoordinates.Yaw)));


			return transformGroup;
		}

		public class Log
		{
			public Log(TextBox textbox)
			{
				_textbox = textbox;
				ClearLog();
				AddEntry("Welcome to Visual Forge!");
			}
			private readonly TextBox _textbox;

			public void AddEntry(string content)
			{
				var timestamp = string.Format("[{0}]", DateTime.Now.ToString("HH:mm:ss.ffff"));

				_textbox.Text += string.Format("{0} - {1}" + Environment.NewLine, timestamp, content);
			}
			public void ClearLog()
			{
				_textbox.Text = "";
			}
		}
	}
}