﻿/* Copyright 2012 Aaron Dierking
 * 
 * This file is part of Liberty.
 * 
 * Liberty is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Liberty is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with VisualForge.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * End Note:
 * This class library was taken from Liberty (http://liberty.codeplex.com/), of which I am a contributor.
 */

namespace VisualForge.Core.Helpers.VectorMath
{
	public struct Vector3
	{
		public float X;
		public float Y;
		public float Z;

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Adds the components of two vectors to create a new vector.
		/// </summary>
		/// <param name="one">The first vector</param>
		/// <param name="two">The second vector</param>
		/// <returns>A vector containing the components added together.</returns>
		public static Vector3 Add(Vector3 one, Vector3 two)
		{
			var x = one.X + two.X;
			var y = one.Y + two.Y;
			var z = one.Z + two.Z;
			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Subtracts the components of a vector from the components of another.
		/// </summary>
		/// <param name="one">The vector to subtract from</param>
		/// <param name="two">The vector to subtract</param>
		/// <returns>A vector containing the subtracted components.</returns>
		public static Vector3 Subtract(Vector3 one, Vector3 two)
		{
			var x = one.X - two.X;
			var y = one.Y - two.Y;
			var z = one.Z - two.Z;
			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Returns the cross-product between two vectors.
		/// </summary>
		/// <param name="one">The first vector</param>
		/// <param name="two">The second vector</param>
		/// <returns>The cross-product</returns>
		public static Vector3 Cross(Vector3 one, Vector3 two)
		{
			Vector3 result;
			result.X = one.Y * two.Z - one.Z * two.Y;
			result.Y = one.Z * two.X - one.X * two.Z;
			result.Z = one.X * two.Y - one.Y * two.X;
			return result;
		}

#pragma warning disable 659
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			var other = (Vector3)obj;
			return (X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z));
		}
#pragma warning restore 659
	}
}
