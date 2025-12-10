using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace CodeForDotNet.Numerics;

/// <summary>
/// A two-dimentional vector represented by angle and length.
/// </summary>
public struct AngleVector2 : IEquatable<AngleVector2>
{
    #region Public Constructors

    /// <summary>
    /// Creates an instance with the specified values.
    /// </summary>
    public AngleVector2(float angle, float length)
        : this()
    {
        Angle = angle;
        Length = length;
    }

    /// <summary>
    /// Creates a structure by converting from a <see cref="PointF"/>.
    /// </summary>
    public AngleVector2(PointF point)
    {
        this = From(point.X, point.Y);
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Angle.
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    /// Length.
    /// </summary>
    public float Length { get; set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Creates an angle based vector from a point based vector..
    /// </summary>					  
    public static AngleVector2 From(PointF point)
    {
        return From(point.X, point.Y);
    }

    /// <summary>
    /// Creates an angle based vector from a point based vector..
    /// </summary>					  
    public static AngleVector2 From(float x, float y)
    {
        // Translate vector to a zero-based quadrant number in order to determine angle offset.
        byte quadrant0;
        float adjacent, opposite;
        if (x < 0)
        {
            if (y <= 0)
            {
                quadrant0 = 1;
                adjacent = -y;
                opposite = -x;
            }
            else
            {
                quadrant0 = 2;
                adjacent = -x;
                opposite = y;
            }
        }
        else
        {
            if (y <= 0)
            {
                quadrant0 = 0;
                adjacent = x;
                opposite = -y;
            }
            else
            {
                quadrant0 = 3;
                adjacent = y;
                opposite = -x;
            }
        }
        var offset = 90 * quadrant0;

        // Calculate length.
        var adjacent2 = Math.Pow(adjacent, 2);
        var opposite2 = Math.Pow(opposite, 2);
        var length2 = adjacent2 + opposite2;
        var length = Math.Sqrt(length2);

        // Calculate angle.
        var quadrantAngle = opposite2 == 0 ? 0 : opposite2 / length2 * 90;
        var angle = quadrantAngle + offset;

        // Return result.
        return new AngleVector2(Convert.ToSingle(angle), Convert.ToSingle(length));
    }

    /// <summary>
    /// Overrides the subtraction operator, subtracting both angle and distance values.
    /// </summary>
    public static AngleVector2 operator -(AngleVector2 ad1, AngleVector2 ad2)
    {
        return new AngleVector2(ad1.Angle - ad2.Angle, ad1.Length - ad2.Length);
    }

    /// <summary>
    /// Compares two objects of this type for inequality.
    /// </summary>
    public static bool operator !=(AngleVector2 ad1, AngleVector2 ad2)
    {
        return !Equals(ad1, ad2);
    }

    /// <summary>
    /// Overrides the addition operator, adding both angle and distance values.
    /// </summary>
    public static AngleVector2 operator +(AngleVector2 ad1, AngleVector2 ad2)
    {
        return new AngleVector2(ad1.Angle + ad2.Angle, ad1.Length + ad2.Length);
    }

    /// <summary>
    /// Compares two objects of this type for equality.
    /// </summary>
    public static bool operator ==(AngleVector2 ad1, AngleVector2 ad2)
    {
        return Equals(ad1, ad2);
    }

    /// <summary>
    /// Adds the value to this instance.
    /// </summary>
    public void Add(AngleVector2 value)
    {
        Angle += value.Angle;
        while (Angle >= 360)
            Angle -= 360;
        Length += value.Length;
    }

    /// <summary>
    /// Compares this object with another.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    public override readonly bool Equals(object other)
    {
        return other is AngleVector2 angleVector && Equals(angleVector);
    }

    /// <summary>
    /// Compares this object with another of the same type.
    /// </summary>
    public readonly bool Equals(AngleVector2 other)
    {
        return Math.Abs(Angle - other.Angle) < float.Epsilon && Math.Abs(Length - other.Length) < float.Epsilon;
    }

    /// <summary>
    /// Gets a hash-code based on the current values.
    /// </summary>
    public override readonly int GetHashCode()
    {
        return Angle.GetHashCode() ^ Length.GetHashCode();
    }

    /// <summary>
    /// Subtracts the value from this instance.
    /// </summary>
    public void Subtract(AngleVector2 value)
    {
        Angle -= value.Angle;
        while (Angle < 0)
            Angle += 360;
        Length -= value.Length;
    }

    /// <summary>
    /// Converts this angle based vector to a point based vector.
    /// </summary>
    public readonly PointF ToPointF()
    {
        // Return empty when length is null.
        if (Length == 0) return PointF.Empty;

        // Change 360 degree angle to standard position (90 degree and quadrant).
        var angle = Angle >= 360 ? Angle % 360 : Angle;
        var quadrant0 = angle > 0 ? (byte)(angle / 90) : 0;
        var offset = quadrant0 * 90;
        var quadrantAngle = angle - offset;

        // Use pythagoras theory to calculate length of sides.
        var length2 = Math.Pow(Length, 2);
        var ratio = quadrantAngle / 90;
        var inverseRatio = (90 - quadrantAngle) / 90;
        var adjacent = Math.Sqrt(length2 * inverseRatio);
        var opposite = Math.Sqrt(length2 * ratio);

        // Transform results according to quadrant.
        double x, y;
        switch (quadrant0)
        {
            case 0:
                x = adjacent;
                y = -opposite;
                break;

            case 1:
                y = -adjacent;
                x = -opposite;
                break;

            case 2:
                x = -adjacent;
                y = opposite;
                break;

            case 3:
                y = adjacent;
                x = opposite;
                break;

            default:
                throw new NotImplementedException();
        }

        // Return result
        return new PointF(Convert.ToSingle(x), Convert.ToSingle(y));
    }

    #endregion Public Methods
}
