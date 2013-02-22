using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using VisualForge.Core.Helpers;
using VisualForge.XNA.Camera;

namespace VisualForge.XNA
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Halo3 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		private IList<GameModel> gameAssets;
		public class GameModel
		{
			public Usermaps.Games.Halo3.MapMetaData.Tag Tag { get; set; }
			public Model Model { get; set; }
		}
		//Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 15.0f);
		private float _aspectRatio;

		private readonly string _filePath;
		private Usermaps.Games.Halo3 _sandbox;

		// Camera
		private FirstPersonCamera camera;


		public Halo3(string filePath)
		{
			_filePath = filePath;
			graphics = new GraphicsDeviceManager(this)
				           {
					           PreferredBackBufferWidth = 1280, 
							   PreferredBackBufferHeight = 720
				           };
			IsMouseVisible = true;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_sandbox = new Usermaps.Games.Halo3(_filePath);
			camera = new FirstPersonCamera(this);
			Components.Add(camera);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			gameAssets = new List<GameModel>();
			foreach(var tag in _sandbox.SandboxMapMetaData.Tags)
			{
				var tagPath = string.Format("{0}\\Content\\{1}\\Assets\\{2}.xnb", VariousFunctions.GetApplicationLocation(),
				                            Usermaps.Games.Halo3.GameId, tag.TagPath);
				var pipelineTagPath = string.Format("{0}\\Assets\\{1}", Usermaps.Games.Halo3.GameId, tag.TagPath);

				if (File.Exists(tagPath))
					gameAssets.Add(new GameModel
						               {
							               Model = Content.Load<Model>(pipelineTagPath),
							               Tag = tag
						               });
			}
			_aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				Exit();
			if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// Draw Models
			foreach (var placedObject in _sandbox.SandboxObjects)
			{
				// if (placedObject.TagEntry == null || !placedObject.TagEntry.Tag.TagPath.Contains("sand_pyr_corner")) continue;

				var assetModel = gameAssets.FirstOrDefault(asset => placedObject.TagEntry != null && asset.Tag.TagIndex == placedObject.TagEntry.Tag.TagIndex);
				if (assetModel == null) continue;

				var transforms = new Matrix[assetModel.Model.Bones.Count];
				assetModel.Model.CopyAbsoluteBoneTransformsTo(transforms);

				foreach (var mesh in assetModel.Model.Meshes)
				{
					foreach (BasicEffect effect in mesh.Effects)
					{
						float yaw, pitch, roll;
						Core.Helpers.VectorMath.Convert.ToYawPitchRoll(
							placedObject.SpawnPosition.Right,
							placedObject.SpawnPosition.Forward,
							placedObject.SpawnPosition.Up,
							out yaw,
							out pitch,
							out roll);

						effect.View = camera.View;
						effect.Projection = camera.Projection;

						effect.EnableDefaultLighting();
						effect.World = Matrix.CreateFromYawPitchRoll(roll, pitch, yaw) * Matrix.CreateTranslation(
							new Vector3(
								placedObject.SpawnCoordinates.X,
								placedObject.SpawnCoordinates.Y,
								placedObject.SpawnCoordinates.Z));
					}

					mesh.Draw();
				}
			}

			base.Draw(gameTime);
		}
	}
}
