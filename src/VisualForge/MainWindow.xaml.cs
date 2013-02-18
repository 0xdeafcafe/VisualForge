using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.Windows;
using VisualForge.Helix.Plugins;
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
		private readonly Log ActionLog;

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
					if (placedObject.TagEntry.Tag.TagPath.Contains("spawning"))
					{

					}

					string gameAssetPath;
					try
					{
						gameAssetPath = VariousFunctions.GetGameAsset(Halo3.GameId, placedObject.TagEntry.Tag.TagPath);
					}
					catch (FileNotFoundException)
					{
						ActionLog.AddEntry("Missing Game Asset");
						continue;
					}

					var model = (Model3D)ModelImporter.Load(gameAssetPath);
					model.Transform = CreateTransformGroup(placedObject);
					container.Add(model);
				}

				ModelViewport.Children.Clear();
				ModelViewport.Children.Add(new GridLines());
				foreach (var model in container)
				{
					ModelViewport.Children.Add(new ModelVisual3D
						                           {
							                           Content = model
						                           });
				}
				var light = new LightVisual3D();
				var ambientLight = new AmbientLight(Colors.Yellow)
					                   {
						                   Transform = new MatrixTransform3D(new Matrix3D
							                                                     {
								                                                     OffsetX = 0,
								                                                     OffsetY = 0,
								                                                     OffsetZ = 100
							                                                     })
					                   };
				light.Content = ambientLight;
				ModelViewport.Children.Add(light);

				ModelViewport.ShowCameraInfo = true;
				ModelViewport.ZoomExtents();
				ModelViewport.IsHeadLightEnabled = true;
			}
		}

		public Transform3DGroup CreateTransformGroup(Halo3.ObjectChunk placedObject)
		{
			var transformGroup = new Transform3DGroup();
			float yaw, pitch, roll;
			Core.Helpers.VectorMath.Convert.ToYawPitchRoll(
				placedObject.SpawnPosition.Right,
				placedObject.SpawnPosition.Forward,
				placedObject.SpawnPosition.Up,
				out yaw,
				out pitch,
				out roll);

			var swag = Microsoft.Xna.Framework.Quaternion.CreateFromYawPitchRoll(roll, pitch, yaw);

			// Apply 3D Matrix
			var matrix = new Matrix3D();
			matrix.Rotate(new Quaternion(swag.X, swag.Y, swag.Z, swag.W));
			matrix.OffsetX = placedObject.SpawnCoordinates.X;
			matrix.OffsetY = placedObject.SpawnCoordinates.Y;
			matrix.OffsetZ = placedObject.SpawnCoordinates.Z;
			// TODO: FUCK THIS VALUE
			// TODO: AND FUCK BUNGIE
			//matrix.Prepend(new Matrix3D
			//					{
			//						OffsetX = 0,
			//						OffsetY = 0,
			//						OffsetZ = 0
			//					});
			transformGroup.Children.Add(new MatrixTransform3D(matrix));


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