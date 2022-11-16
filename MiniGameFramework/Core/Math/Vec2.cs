using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace MiniGameFramework
{
    static class MethodImplOptionsEx
    {
        public const short AggressiveInlining = 256;
    }

    public struct Vec2 : IEquatable<Vec2>, IFormattable
    {
        // X component of the vector.
        public float x;
        // Y component of the vector.
        public float y;

        // Access the /x/ or /y/ component using [0] or [1] respectively.
        public float this[int index]
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vec2 index!");
                }
            }

            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vec2 index!");
                }
            }
        }

        // Constructs a new vector with given x, y components.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Vec2(float x, float y) { this.x = x; this.y = y; }

        // Set x and y components of an existing Vec2.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Set(float newX, float newY) { x = newX; y = newY; }

        // Linearly interpolates between two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 Lerp(Vec2 a, Vec2 b, float t)
        {
            t = Mathfloat.Clamp01(t);
            return new Vec2(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t
            );
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 LerpUnclamped(Vec2 a, Vec2 b, float t)
        {
            return new Vec2(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t
            );
        }

        // Moves a point /current/ towards /target/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 MoveTowards(Vec2 current, Vec2 target, float maxDistanceDelta)
        {
            // avoid vector ops because current scripting backends are terrible at inlining
            float toVector_x = target.x - current.x;
            float toVector_y = target.y - current.y;

            float sqDist = toVector_x * toVector_x + toVector_y * toVector_y;

            if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float dist = (float)Math.Sqrt(sqDist);

            return new Vec2(current.x + toVector_x / dist * maxDistanceDelta,
                current.y + toVector_y / dist * maxDistanceDelta);
        }

        // Multiplies two vectors component-wise.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 Scale(Vec2 a, Vec2 b) { return new Vec2(a.x * b.x, a.y * b.y); }

        // Multiplies every component of this vector by the same component of /scale/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Scale(Vec2 scale) { x *= scale.x; y *= scale.y; }

        // Makes this vector have a ::ref::magnitude of 1.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Normalize()
        {
            float mag = magnitude;
            if (mag > kEpsilon)
                this = this / mag;
            else
                this = zero;
        }

        // Returns this vector with a ::ref::magnitude of 1 (RO).
        public Vec2 normalized
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                Vec2 v = new Vec2(x, y);
                v.Normalize();
                return v;
            }
        }

        /// *listonly*
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        // Returns a nicely formatted string for this vector.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            //return UnityString.Format("({0}, {1})", x.ToString(format, formatProvider), y.ToString(format, formatProvider));
            return $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)})";
        }

        // used to allow Vector2s to be used as keys in hash tables
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        // also required for being able to use Vector2s as keys in hash tables
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is Vec2)) return false;

            return Equals((Vec2)other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public bool Equals(Vec2 other)
        {
            return x == other.x && y == other.y;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 Reflect(Vec2 inDirection, Vec2 inNormal)
        {
            float factor = -2F * Dot(inNormal, inDirection);
            return new Vec2(factor * inNormal.x + inDirection.x, factor * inNormal.y + inDirection.y);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 Perpendicular(Vec2 inDirection)
        {
            return new Vec2(-inDirection.y, inDirection.x);
        }

        // Dot Product of two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Dot(Vec2 lhs, Vec2 rhs) { return lhs.x * rhs.x + lhs.y * rhs.y; }

        // Returns the length of this vector (RO).
        public float magnitude { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return (float)Math.Sqrt(x * x + y * y); } }
        // Returns the squared length of this vector (RO).
        public float sqrMagnitude { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return x * x + y * y; } }

        // Returns the angle in degrees between /from/ and /to/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Angle(Vec2 from, Vec2 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = Mathfloat.Clamp(Dot(from, to) / denominator, -1F, 1F);
            return (float)Math.Acos(dot) * Mathfloat.Rad2Deg;
        }

        // Returns the signed angle in degrees between /from/ and /to/. Always returns the smallest possible angle
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float SignedAngle(Vec2 from, Vec2 to)
        {
            float unsigned_angle = Angle(from, to);
            float sign = Mathfloat.Sign(from.x * to.y - from.y * to.x);
            return unsigned_angle * sign;
        }

        // Returns the distance between /a/ and /b/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Distance(Vec2 a, Vec2 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            return (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y);
        }

        // Returns a copy of /vector/ with its magnitude clamped to /maxLength/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 ClampMagnitude(Vec2 vector, float maxLength)
        {
            float sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > maxLength * maxLength)
            {
                float mag = (float)Math.Sqrt(sqrMagnitude);

                //these intermediate variables force the intermediate result to be
                //of float precision. without this, the intermediate result can be of higher
                //precision, which changes behavior.
                float normalized_x = vector.x / mag;
                float normalized_y = vector.y / mag;
                return new Vec2(normalized_x * maxLength,
                    normalized_y * maxLength);
            }
            return vector;
        }

        // [Obsolete("Use Vec2.sqrMagnitude")]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float SqrMagnitude(Vec2 a) { return a.x * a.x + a.y * a.y; }
        // [Obsolete("Use Vec2.sqrMagnitude")]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public float SqrMagnitude() { return x * x + y * y; }

        // Returns a vector that is made from the smallest components of two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 Min(Vec2 lhs, Vec2 rhs) { return new Vec2(Mathfloat.Min(lhs.x, rhs.x), Mathfloat.Min(lhs.y, rhs.y)); }

        // Returns a vector that is made from the largest components of two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 Max(Vec2 lhs, Vec2 rhs) { return new Vec2(Mathfloat.Max(lhs.x, rhs.x), Mathfloat.Max(lhs.y, rhs.y)); }

        //[uei.ExcludeFromDocs]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Vec2 SmoothDamp(Vec2 current, Vec2 target, ref Vec2 currentVelocity, float smoothTime, float maxSpeed)
        //{
        //    float deltaTime = Time.deltaTime;
        //    return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        //}

        //[uei.ExcludeFromDocs]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Vec2 SmoothDamp(Vec2 current, Vec2 target, ref Vec2 currentVelocity, float smoothTime)
        //{
        //    float deltaTime = Time.deltaTime;
        //    float maxSpeed = Mathfloat.Infinity;
        //    return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        //}

        public static Vec2 SmoothDamp(Vec2 current, Vec2 target, ref Vec2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Mathfloat.Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);

            float change_x = current.x - target.x;
            float change_y = current.y - target.y;
            Vec2 originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;

            float maxChangeSq = maxChange * maxChange;
            float sqDist = change_x * change_x + change_y * change_y;
            if (sqDist > maxChangeSq)
            {
                var mag = (float)Math.Sqrt(sqDist);
                change_x = change_x / mag * maxChange;
                change_y = change_y / mag * maxChange;
            }

            target.x = current.x - change_x;
            target.y = current.y - change_y;

            float temp_x = (currentVelocity.x + omega * change_x) * deltaTime;
            float temp_y = (currentVelocity.y + omega * change_y) * deltaTime;

            currentVelocity.x = (currentVelocity.x - omega * temp_x) * exp;
            currentVelocity.y = (currentVelocity.y - omega * temp_y) * exp;

            float output_x = target.x + (change_x + temp_x) * exp;
            float output_y = target.y + (change_y + temp_y) * exp;

            // Prevent overshooting
            float origMinusCurrent_x = originalTo.x - current.x;
            float origMinusCurrent_y = originalTo.y - current.y;
            float outMinusOrig_x = output_x - originalTo.x;
            float outMinusOrig_y = output_y - originalTo.y;

            if (origMinusCurrent_x * outMinusOrig_x + origMinusCurrent_y * outMinusOrig_y > 0)
            {
                output_x = originalTo.x;
                output_y = originalTo.y;

                currentVelocity.x = (output_x - originalTo.x) / deltaTime;
                currentVelocity.y = (output_y - originalTo.y) / deltaTime;
            }
            return new Vec2(output_x, output_y);
        }

        // Adds two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator +(Vec2 a, Vec2 b) { return new Vec2(a.x + b.x, a.y + b.y); }
        // Subtracts one vector from another.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator -(Vec2 a, Vec2 b) { return new Vec2(a.x - b.x, a.y - b.y); }
        // Multiplies one vector by another.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator *(Vec2 a, Vec2 b) { return new Vec2(a.x * b.x, a.y * b.y); }
        // Divides one vector over another.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator /(Vec2 a, Vec2 b) { return new Vec2(a.x / b.x, a.y / b.y); }
        // Negates a vector.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator -(Vec2 a) { return new Vec2(-a.x, -a.y); }
        // Multiplies a vector by a number.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator *(Vec2 a, float d) { return new Vec2(a.x * d, a.y * d); }
        // Multiplies a vector by a number.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator *(float d, Vec2 a) { return new Vec2(a.x * d, a.y * d); }
        // Divides a vector by a number.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec2 operator /(Vec2 a, float d) { return new Vec2(a.x / d, a.y / d); }
        // Returns true if the vectors are equal.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool operator ==(Vec2 lhs, Vec2 rhs)
        {
            // Returns false in the presence of NaN values.
            float diff_x = lhs.x - rhs.x;
            float diff_y = lhs.y - rhs.y;
            return (diff_x * diff_x + diff_y * diff_y) < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool operator !=(Vec2 lhs, Vec2 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // Converts a [[Vector3]] to a Vec2.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static implicit operator Vec2(Vec3 v)
        {
            return new Vec2(v.x, v.y);
        }

        // Converts a Vec2 to a [[Vector3]].
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static implicit operator Vec3(Vec2 v)
        {
            return new Vec3(v.x, v.y, 0);
        }

        static readonly Vec2 zeroVector = new Vec2(0F, 0F);
        static readonly Vec2 oneVector = new Vec2(1F, 1F);
        static readonly Vec2 upVector = new Vec2(0F, 1F);
        static readonly Vec2 downVector = new Vec2(0F, -1F);
        static readonly Vec2 leftVector = new Vec2(-1F, 0F);
        static readonly Vec2 rightVector = new Vec2(1F, 0F);
        static readonly Vec2 positiveInfinityVector = new Vec2(float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vec2 negativeInfinityVector = new Vec2(float.NegativeInfinity, float.NegativeInfinity);


        // Shorthand for writing @@Vec2(0, 0)@@
        public static Vec2 zero { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return zeroVector; } }
        // Shorthand for writing @@Vec2(1, 1)@@
        public static Vec2 one { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return oneVector; } }
        // Shorthand for writing @@Vec2(0, 1)@@
        public static Vec2 up { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return upVector; } }
        // Shorthand for writing @@Vec2(0, -1)@@
        public static Vec2 down { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return downVector; } }
        // Shorthand for writing @@Vec2(-1, 0)@@
        public static Vec2 left { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return leftVector; } }
        // Shorthand for writing @@Vec2(1, 0)@@
        public static Vec2 right { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return rightVector; } }
        // Shorthand for writing @@Vec2(float.PositiveInfinity, float.PositiveInfinity)@@
        public static Vec2 positiveInfinity { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return positiveInfinityVector; } }
        // Shorthand for writing @@Vec2(float.NegativeInfinity, float.NegativeInfinity)@@
        public static Vec2 negativeInfinity { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return negativeInfinityVector; } }

        // *Undocumented*
        public const float kEpsilon = 0.00001F;
        // *Undocumented*
        public const float kEpsilonNormalSqrt = 1e-15f;
    }
}
