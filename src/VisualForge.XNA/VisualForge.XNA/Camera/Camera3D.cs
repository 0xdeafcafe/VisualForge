using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VisualForge.XNA.Camera
{
	public class VisualCamera
	{
		public Vector3 CameraPosition { get; private set; }
		public float CameraYaw { get; private set; }
		public float CameraPitch { get; private set; }
		public Vector3 CameraLookAt
		{
			get
			{
				var lookAt = new Vector3(0f, 0f, -1f);

				// Rotate the Vector
				lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationX(MathHelper.ToRadians(CameraPitch)));
				lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationY(MathHelper.ToRadians(CameraYaw)));

				return lookAt;
			}
		}

		public Matrix ProjectionMatrix
		{
			get
			{
				return Matrix.CreatePerspectiveFieldOfView(
					MathHelper.ToRadians(60.0f),
					ParentGame.AspectRatio,
					0.5f,
					1000f);
			}
		}
		public Matrix ViewMatrix
		{
			get
			{
				return  Matrix.CreateLookAt(
					CameraPosition,
					CameraPosition + CameraLookAt,
					Vector3.Up);
			}
		}

		public Halo3 ParentGame { get; private set; }

		public VisualCamera(Halo3 game)
		{
			ParentGame = game;

			CameraYaw = 
				CameraPitch = 0.0f;
		}

		public void UpdateCamera()
		{
			var gamepadState = GamePad.GetState(PlayerIndex.One);

			// Sideways Movement
			if (gamepadState.ThumbSticks.Left.X != 0.0f)
				if (gamepadState.ThumbSticks.Left.X < 0)
					MoveCamera(new Vector3(-0.1f, 0, 0));
				else if (gamepadState.ThumbSticks.Left.X > 0)
					MoveCamera(new Vector3(0.1f, 0, 0));

			// Forward Movement
			if (gamepadState.ThumbSticks.Left.Y != 0.0f)
				if (gamepadState.ThumbSticks.Left.Y < 0)
					MoveCamera(new Vector3(0, 0, 0.1f));
				else if (gamepadState.ThumbSticks.Left.Y > 0)
					MoveCamera(new Vector3(0, 0, -0.1f));

			// Horizontal
			if (gamepadState.ThumbSticks.Right.X != 0.0f)
			{
				CameraYaw += gamepadState.ThumbSticks.Right.X;
			}

			if (CameraYaw > 360.0f || CameraYaw < -360.0f)
				CameraYaw = 0.0f;

			//if (gamepadState.Buttons.LeftShoulder == ButtonState.Pressed)
			//{
			//	camera.StrafeVert(0.2f);
			//}
			//if (gamepadState.Buttons.RightShoulder == ButtonState.Pressed)
			//{
			//	camera.StrafeVert(-0.2f);
			//}
		}
		private void MoveCamera(Vector3 movementVector)
		{
			// Create rotation matrix
			var rot = Matrix.CreateRotationY(MathHelper.ToRadians(CameraYaw));

			// Transform the movement vector
			movementVector = Vector3.Transform(movementVector, rot);

			// Add transformed vector to position
			CameraPosition += movementVector;
		}
	}
}
