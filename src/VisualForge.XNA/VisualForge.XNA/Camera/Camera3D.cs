using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VisualForge.XNA.Camera
{
	/// <summary>
	/// A simple first person camera. Most code from JigLibX's camera class.
	/// </summary>
	public class FirstPersonCamera : GameComponent
	{
		private Matrix view;
		public Matrix View
		{
			get { return view; }
		}

		private Matrix projection;
		public Matrix Projection
		{
			get { return projection; }
		}

		public Matrix ViewProjection
		{
			get { return view * projection; }
		}

		private Vector3 position = new Vector3(0, 15, 10);
		public Vector3 Position
		{
			get { return position; }
			set { position = value; }
		}

		/// <summary>
		/// Describes the amount in radians to offset from the X and Y planes.
		/// </summary>
		private Vector2 angles = Vector2.Zero;
		public Vector2 Angles
		{
			get { return angles; }
		}

		/// <summary>
		/// Screen width / 2
		/// </summary>
		private int widthOver2;

		/// <summary>
		/// Screen height / 2
		/// </summary>
		private int heightOver2;

		private float fieldOfView = MathHelper.ToRadians(70.0f);
		private float aspectRatio;
		private float nearPlaneDist = 0.1f;
		private float farPlaneDist = 10000.0f;

		/// <summary>
		/// Gets and sets the camera's target.
		/// </summary>
		public Vector3 Target
		{
			get
			{
				var cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
				return position + Vector3.Transform(Vector3.Forward, cameraRotation);
			}
			set
			{
				var forward = Vector3.Normalize(position - value);
				var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
				var up = Vector3.Normalize(Vector3.Cross(right, forward));

				var test = Matrix.Identity;
				test.Forward = forward;
				test.Right = right;
				test.Up = up;

				angles.X = -(float)Math.Asin(test.M32);
				angles.Y = -(float)Math.Asin(test.M13);
			}
		}


		public FirstPersonCamera(Game game)
			: base(game)
		{
			widthOver2 = game.Window.ClientBounds.Width / 2;
			heightOver2 = game.Window.ClientBounds.Height / 2;
			aspectRatio = game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height;
			UpdateProjection();
		}

		public override void Update(GameTime gameTime)
		{
			ProcessInput();
			UpdateView();
			base.Update(gameTime);
		}

		/// <summary>
		/// Update the projection matrix.
		/// </summary>
		private void UpdateProjection()
		{
			projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDist, farPlaneDist);
		}

		/// <summary>
		/// Process input and realign the camera
		/// </summary>
		private void ProcessInput()
		{
			var movementVec = new Vector3();
			var gamePad = GamePad.GetState(PlayerIndex.One);
			var boostModifier = gamePad.Triggers.Left + 1;

			if (gamePad.ThumbSticks.Left.X != 0)
			{
				movementVec.X = -gamePad.ThumbSticks.Left.X / (15 / boostModifier);
			}
			if (gamePad.ThumbSticks.Left.Y != 0)
			{
				movementVec.Z -= -gamePad.ThumbSticks.Left.Y / (15 / boostModifier);
			}

			var camRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
			position += Vector3.Transform(movementVec, camRotation);

			// Turn the camera based on the distance between the center of the screen
			//and our current mouse position.
			if (gamePad.ThumbSticks.Right.X != 0)
			{
				angles.Y -= -gamePad.ThumbSticks.Right.X / 15;
			}
			if (gamePad.ThumbSticks.Right.Y != 0)
			{
				angles.X -= gamePad.ThumbSticks.Right.Y / 15;
			}

			//if (currentMouseState.X != widthOver2)
			//	angles.Y -= movementAmt / 800.0f * (currentMouseState.X - widthOver2);
			//if (currentMouseState.Y != heightOver2)
			//	angles.X -= movementAmt / 800.0f * (currentMouseState.Y - heightOver2);

			// Limit the angles of rotation to (1.4, 2PI)
			//if (angles.X > 1.4f) angles.X = 1.4f;
			//if (angles.X < -1.4f) angles.X = -1.4f;
			//if (angles.Y > Math.PI) angles.Y -= 2 * (float)Math.PI;
			//if (angles.Y < -Math.PI) angles.Y += 2 * (float)Math.PI;
		}

		/// <summary>
		/// Update the view matrix to reflect camera realignment.
		/// </summary>
		private void UpdateView()
		{
			var camRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
			var targetPos = position + Vector3.Transform(Vector3.Backward, camRotation);
			var up = Vector3.Transform(Vector3.Up, camRotation);
			view = Matrix.CreateLookAt(position, targetPos, up);
		}
	}
}

