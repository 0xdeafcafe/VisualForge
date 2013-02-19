using Microsoft.Xna.Framework;

namespace VisualForge.XNA.Camera
{
	public class Camera : GameComponent
	{
		public Vector3 position;
		public Vector3 target;
		public Vector3 upVector;

		public Matrix viewMatrix;
		public Matrix projectionMatrix;
		public float fieldOfView = MathHelper.PiOver4;
		public float nearPlaneDistance = 1f;
		public float farPlaneDistance = 500f;

		public Camera(Game game, Vector3 position, Vector3 target, Vector3 upVector)
			: base(game)
		{
			this.position = position;
			this.target = target;
			this.upVector = upVector;
		}

		public override void Initialize()
		{
			UpdateViewMatrix();
			var aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
			projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);

			base.Initialize();
		}

		protected void UpdateViewMatrix()
		{
			viewMatrix = Matrix.CreateLookAt(position, target, upVector);
		}
	}
}
