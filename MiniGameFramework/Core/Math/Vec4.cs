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
    public partial struct Vec4 : IEquatable<Vec4>, IFormattable
    {
        // *undocumented*
        public const float kEpsilon = 0.00001F;

        // X component of the vector.
        public float x;
        // Y component of the vector.
        public float y;
        // Z component of the vector.
        public float z;
        // W component of the vector.
        public float w;

        // Access the x, y, z, w components using [0], [1], [2], [3] respectively.
        public float this[int index]
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vec4 index!");
                }
            }

            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vec4 index!");
                }
            }
        }

        // Creates a new vector with given x, y, z, w components.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Vec4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        // Creates a new vector with given x, y, z components and sets /w/ to zero.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Vec4(float x, float y, float z) { this.x = x; this.y = y; this.z = z; this.w = 0F; }
        // Creates a new vector with given x, y components and sets /z/ and /w/ to zero.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Vec4(float x, float y) { this.x = x; this.y = y; this.z = 0F; this.w = 0F; }

        // Set x, y, z and w components of an existing Vec4.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Set(float newX, float newY, float newZ, float newW) { x = newX; y = newY; z = newZ; w = newW; }

        // Linearly interpolates between two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 Lerp(Vec4 a, Vec4 b, float t)
        {
            t = Mathfloat.Clamp01(t);
            return new Vec4(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t
            );
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 LerpUnclamped(Vec4 a, Vec4 b, float t)
        {
            return new Vec4(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t
            );
        }

        // Moves a point /current/ towards /target/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 MoveTowards(Vec4 current, Vec4 target, float maxDistanceDelta)
        {
            float toVector_x = target.x - current.x;
            float toVector_y = target.y - current.y;
            float toVector_z = target.z - current.z;
            float toVector_w = target.w - current.w;

            float sqdist = (toVector_x * toVector_x +
                toVector_y * toVector_y +
                toVector_z * toVector_z +
                toVector_w * toVector_w);

            if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
                return target;

            var dist = (float)Math.Sqrt(sqdist);

            return new Vec4(current.x + toVector_x / dist * maxDistanceDelta,
                current.y + toVector_y / dist * maxDistanceDelta,
                current.z + toVector_z / dist * maxDistanceDelta,
                current.w + toVector_w / dist * maxDistanceDelta);
        }

        // Multiplies two vectors component-wise.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 Scale(Vec4 a, Vec4 b)
        {
            return new Vec4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        // Multiplies every component of this vector by the same component of /scale/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Scale(Vec4 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }

        // used to allow Vector4s to be used as keys in hash tables
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
        }

        // also required for being able to use Vector4s as keys in hash tables
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is Vec4)) return false;

            return Equals((Vec4)other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public bool Equals(Vec4 other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        // *undoc* --- we have normalized property now
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 Normalize(Vec4 a)
        {
            float mag = Magnitude(a);
            if (mag > kEpsilon)
                return a / mag;
            else
                return zero;
        }

        // Makes this vector have a ::ref::magnitude of 1.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Normalize()
        {
            float mag = Magnitude(this);
            if (mag > kEpsilon)
                this = this / mag;
            else
                this = zero;
        }

        // Returns this vector with a ::ref::magnitude of 1 (RO).
        public Vec4 normalized
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                return Vec4.Normalize(this);
            }
        }

        // Dot Product of two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Dot(Vec4 a, Vec4 b) { return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w; }

        // Projects a vector onto another vector.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 Project(Vec4 a, Vec4 b) { return b * (Dot(a, b) / Dot(b, b)); }

        // Returns the distance between /a/ and /b/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Distance(Vec4 a, Vec4 b) { return Magnitude(a - b); }

        // *undoc* --- there's a property now
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Magnitude(Vec4 a) { return (float)Math.Sqrt(Dot(a, a)); }

        // Returns the length of this vector (RO).
        public float magnitude
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return (float)Math.Sqrt(Dot(this, this)); }
        }

        // Returns the squared length of this vector (RO).
        public float sqrMagnitude
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return Dot(this, this); }
        }

        // Returns a vector that is made from the smallest components of two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 Min(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(Mathfloat.Min(lhs.x, rhs.x), Mathfloat.Min(lhs.y, rhs.y), Mathfloat.Min(lhs.z, rhs.z), Mathfloat.Min(lhs.w, rhs.w));
        }

        // Returns a vector that is made from the largest components of two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 Max(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(Mathfloat.Max(lhs.x, rhs.x), Mathfloat.Max(lhs.y, rhs.y), Mathfloat.Max(lhs.z, rhs.z), Mathfloat.Max(lhs.w, rhs.w));
        }

        static readonly Vec4 zeroVector = new Vec4(0F, 0F, 0F, 0F);
        static readonly Vec4 oneVector = new Vec4(1F, 1F, 1F, 1F);
        static readonly Vec4 positiveInfinityVector = new Vec4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vec4 negativeInfinityVector = new Vec4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        // Shorthand for writing @@Vec4(0,0,0,0)@@
        public static Vec4 zero { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return zeroVector; } }
        // Shorthand for writing @@Vec4(1,1,1,1)@@
        public static Vec4 one { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return oneVector; } }
        // Shorthand for writing @@Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)@@
        public static Vec4 positiveInfinity { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return positiveInfinityVector; } }
        // Shorthand for writing @@Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity)@@
        public static Vec4 negativeInfinity { [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return negativeInfinityVector; } }

        // Adds two vectors.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 operator +(Vec4 a, Vec4 b) { return new Vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w); }
        // Subtracts one vector from another.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 operator -(Vec4 a, Vec4 b) { return new Vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w); }
        // Negates a vector.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 operator -(Vec4 a) { return new Vec4(-a.x, -a.y, -a.z, -a.w); }
        // Multiplies a vector by a number.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 operator *(Vec4 a, float d) { return new Vec4(a.x * d, a.y * d, a.z * d, a.w * d); }
        // Multiplies a vector by a number.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 operator *(float d, Vec4 a) { return new Vec4(a.x * d, a.y * d, a.z * d, a.w * d); }
        // Divides a vector by a number.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Vec4 operator /(Vec4 a, float d) { return new Vec4(a.x / d, a.y / d, a.z / d, a.w / d); }

        // Returns true if the vectors are equal.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool operator ==(Vec4 lhs, Vec4 rhs)
        {
            // Returns false in the presence of NaN values.
            float diffx = lhs.x - rhs.x;
            float diffy = lhs.y - rhs.y;
            float diffz = lhs.z - rhs.z;
            float diffw = lhs.w - rhs.w;
            float sqrmag = diffx * diffx + diffy * diffy + diffz * diffz + diffw * diffw;
            return sqrmag < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool operator !=(Vec4 lhs, Vec4 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // Converts a [[Vec3]] to a Vec4.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static implicit operator Vec4(Vec3 v)
        {
            return new Vec4(v.x, v.y, v.z, 0.0F);
        }

        // Converts a Vec4 to a [[Vec3]].
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static implicit operator Vec3(Vec4 v)
        {
            return new Vec3(v.x, v.y, v.z);
        }

        // Converts a [[Vec2]] to a Vec4.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static implicit operator Vec4(Vec2 v)
        {
            return new Vec4(v.x, v.y, 0.0F, 0.0F);
        }

        // Converts a Vec4 to a [[Vec2]].
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static implicit operator Vec2(Vec4 v)
        {
            return new Vec2(v.x, v.y);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

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
            //return UnityString.Format("({0}, {1}, {2}, {3})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
            return $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)}, {z.ToString(format, formatProvider)}, {w.ToString(format, formatProvider)})";
        }

        // *undoc* --- there's a property now
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float SqrMagnitude(Vec4 a) { return Vec4.Dot(a, a); }
        // *undoc* --- there's a property now
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public float SqrMagnitude() { return Dot(this, this); }
    }
}
