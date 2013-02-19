using System;
using Microsoft.Xna.Framework;

namespace VisualForge.XNA.Camera
{
	public class FirstPersonCamera : Camera
	{
		public Vector3 cameraReference;

		public float leftRightRot;
		public float upDownRot;

		public float rotationSpeed = 0.05f;
		public float translationSpeed = 1f;

		public FirstPersonCamera(Game game, Vector3 position, Vector3 target, Vector3 upVector)
			: base(game, position, target, upVector)
		{
			cameraReference = target;
		}

		public void Update(Vector3 translation, float leftRightRot, float upDownRot)
		{
			if (Math.Abs(this.upDownRot + (upDownRot * rotationSpeed)) < 1.0f)
				this.upDownRot += upDownRot * rotationSpeed;
			else
			{
				if (this.upDownRot < 0)
					this.upDownRot += upDownRot * rotationSpeed;
				else 
					if (this.upDownRot > 0 && upDownRot < 0)
						this.upDownRot += upDownRot * rotationSpeed;
			}

			this.leftRightRot += leftRightRot * rotationSpeed;
			this.upDownRot += upDownRot * rotationSpeed;
			var rotationMatrix = Matrix.CreateRotationX(this.upDownRot) * Matrix.CreateRotationY(this.leftRightRot);
			var transformedReference = Vector3.Transform(cameraReference, rotationMatrix);
			position += Vector3.Transform(translation, rotationMatrix) * translationSpeed;
			target = transformedReference + position;

			UpdateViewMatrix();
		}
	}
}
