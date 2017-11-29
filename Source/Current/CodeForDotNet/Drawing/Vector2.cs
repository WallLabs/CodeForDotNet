using System;
using System.Diagnostics;
using System.Drawing;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// A 2D vector represented by angle and distance.
    /// </summary>
    public struct Vector2
    {
        #region Lifetime

        /// <summary>
        /// Creates structure of this type containing the specified values.
        /// </summary>
        public Vector2(float angle, float distance)
            : this()
        {
            Angle = angle;
            Distance = distance;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Compares two this object against the other by value.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Check nullability and type without causing endless recursion
            if (ReferenceEquals(null, obj) || !(obj is Vector2))
                return false;

            // Compare values
            var other = (Vector2)obj;
            return (Math.Abs(Angle - other.Angle) < Single.Epsilon && Math.Abs(Distance - other.Distance) < Single.Epsilon);
        }

        /// <summary>
        /// Gets a hash-code based on the current values.
        /// </summary>
        public override int GetHashCode()
        {
            return Angle.GetHashCode() ^ Distance.GetHashCode();
        }

        /// <summary>
        /// Compares two objects of this type for equality.
        /// </summary>
        public static bool operator ==(Vector2 ad1, Vector2 ad2)
        {
            return Equals(ad1, ad2);
        }

        /// <summary>
        /// Compares two objects of this type for inequality.
        /// </summary>
        public static bool operator !=(Vector2 ad1, Vector2 ad2)
        {
            return !Equals(ad1, ad2);
        }

        /// <summary>
        /// Overrides the addition operator, adding both angle and distance values.
        /// </summary>
        public static Vector2 operator +(Vector2 ad1, Vector2 ad2)
        {
            return new Vector2(ad1.Angle + ad2.Angle, ad1.Distance + ad2.Distance);
        }

        /// <summary>
        /// Overrides the subtraction operator, subtracting both angle and distance values.
        /// </summary>
        public static Vector2 operator -(Vector2 ad1, Vector2 ad2)
        {
            return new Vector2(ad1.Angle - ad2.Angle, ad1.Distance - ad2.Distance);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The angle of this vector.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// The distance of this vector.
        /// </summary>
        public float Distance { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the DxDy vector to an AngleDistance.
        /// </summary>
        public static Vector2 FromPointF(PointF point)
        {
            // Prepare result
            var result = new Vector2(0, 0);

            // Translate vector to 90 degree Quadrant
            Quadrant q;
            if (point.X < 0)
            {
                if (point.Y < 0)
                {
                    q = Quadrant.TopLeft;
                    point = new PointF(-point.X, -point.Y);
                }
                else
                {
                    q = Quadrant.BottomLeft;
                    point = new PointF(-point.X, point.Y);
                }
            }
            else
            {
                if (point.Y < 0)
                {
                    q = Quadrant.TopRight;
                    point = new PointF(point.X, -point.Y);
                }
                else
                {
                    q = Quadrant.BottomRight;
                }
            }

            // Calculate distance from X and Y
            result.Distance = Convert.ToInt32(Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2)));

            // Calculate angle from X and Y
            var radians = Math.Atan2(point.Y, point.X);
            result.Angle = Convert.ToSingle(radians * (180 / Math.PI));

            // Calculate angle from Y and distance
            //			adResult.Angle = (System.Math.Pow(dxdy.DY, 2) / System.Math.Pow(adResult.Distance, 2)) * 90;

            // Transform result back according to quadrant
            switch (q)
            {
                case Quadrant.TopRight:
                    result = new Vector2(89 - result.Angle, result.Distance);
                    break;
                case Quadrant.BottomRight:
                    result = new Vector2(90 + result.Angle, result.Distance);
                    break;
                case Quadrant.BottomLeft:
                    result = new Vector2(269 - result.Angle, result.Distance);
                    break;
                case Quadrant.TopLeft:
                    result = new Vector2(270 + result.Angle, result.Distance);
                    break;
            }

            // Range check
            Debug.Assert(result.Angle >= 0 && result.Angle < 360);

            // Return result
            return result;
        }

        /// <summary>
        /// Converts an AngleDistance vector this type.
        /// </summary>
        public PointF ToPointF()
        {
            // Change 360 degree Angle to 90 degree and Quadrant
            var vector = this;
            var quadrant = (Quadrant)((byte)(vector.Angle / 90));
            vector.Angle -= 90 * ((byte)quadrant);
            if ((quadrant == Quadrant.TopRight) || (quadrant == Quadrant.BottomLeft))
                vector.Angle = 90 - vector.Angle;

            // Use Matrix rotation to transform angle and distance to points
            //var points = new[] { new PointF(0, -Distance) };
            //using (var m = new Matrix())
            //{
            //    m.Rotate(Angle);
            //    m.TransformPoints(points);
            //}
            //result.X = (int)points[0].X;
            //result.Y = (int)points[0].Y;

            // Calculate Y from Distance and Angle
            var y = (int)Math.Sqrt(Math.Pow(vector.Distance, 2) * (vector.Angle / 90));

            // Calculate X from Distance and Angle
            var x = (int)Math.Sqrt(Math.Pow(vector.Distance, 2) * ((90 - vector.Angle) / 90));

            // Transform results according to quadrant
            switch (quadrant)
            {
                case Quadrant.TopRight:
                    y = -y;
                    break;

                case Quadrant.BottomLeft:
                    x = -x;
                    break;

                case Quadrant.TopLeft:
                    x = -x;
                    y = -y;
                    break;
            }

            // Return result
            return new PointF(x, y);
        }

        /// <summary>
        /// Adds the value to this instance.
        /// </summary>
        public void Add(Vector2 value)
        {
            Angle += value.Angle;
            while (Angle >= 360)
                Angle -= 360;
            Distance += value.Distance;
        }

        /// <summary>
        /// Subtracts the value from this instance.
        /// </summary>
        public void Subtract(Vector2 value)
        {
            Angle -= value.Angle;
            while (Angle < 0)
                Angle += 360;
            Distance -= value.Distance;
        }

        #endregion
    }
}
