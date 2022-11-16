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
    // TO DO : implement Quat with unity native code

    public struct Quat : IEquatable<Quat>
    {
        const float radToDeg = (float)(180.0 / Math.PI);
        const float degToRad = (float)(Math.PI / 180.0);

        public const float kEpsilon = 1E-06f; // should probably be used in the 0 tests in LookRotation or Slerp
        
        public Vec3 xyz
        {
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
            get
            {
                return new Vec3(x, y, z);
            }
        }
        /// <summary>
        ///   <para>X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float x;
        /// <summary>
        ///   <para>Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float y;
        /// <summary>
        ///   <para>Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float z;
        /// <summary>
        ///   <para>W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float w;
        
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    case 3:
                        return this.w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index: " + index + ", can use only 0,1,2,3");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index: " + index + ", can use only 0,1,2,3");
                }
            }
        }
        /// <summary>
        ///   <para>The identity rotation (RO).</para>
        /// </summary>
        public static Quat identity
        {
            get
            {
                return new Quat(0f, 0f, 0f, 1f);
            }
        }
        /// <summary>
        ///   <para>Returns the euler angle representation of the rotation.</para>
        /// </summary>
        public Vec3 eulerAngles
        {
            get
            {
                return Quat.ToEulerRad(this) * radToDeg;
            }
            set
            {
                this = Quat.FromEulerRad(value * degToRad);
            }
        }
        /// <summary>
        /// Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        /// <summary>
        /// Gets the square of the quaternion length (magnitude).
        /// </summary>
        
        public float LengthSquared
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }
        /// <summary>
        ///   <para>Constructs new Quat with given x,y,z,w components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Quat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        /// <summary>
        /// Construct a new Quat from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quat(Vec3 v, float w)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = w;
        }
        /// <summary>
        ///   <para>Set x, y, z and w components of an existing Quat.</para>
        /// </summary>
        /// <param name="new_x"></param>
        /// <param name="new_y"></param>
        /// <param name="new_z"></param>
        /// <param name="new_w"></param>
        public void Set(float new_x, float new_y, float new_z, float new_w)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }
        /// <summary>
        /// Scales the Quat to unit length.
        /// </summary>
        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            xyz *= scale;
            w *= scale;
        }
        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quat Normalize(Quat q)
        {
            Quat result;
            Normalize(ref q, out result);
            return result;
        }
        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref Quat q, out Quat result)
        {
            float scale = 1.0f / q.Length;
            result = new Quat(q.xyz * scale, q.w * scale);
        }
        /// <summary>
        ///   <para>The dot product between two rotations.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Dot(Quat a, Quat b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
        /// <summary>
        ///   <para>Creates a rotation which rotates /angle/ degrees around /axis/.</para>
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        public static Quat AngleAxis(float angle, Vec3 axis)
        {
            return Quat.AngleAxis(angle, ref axis);
        }
        private static Quat AngleAxis(float degress, ref Vec3 axis)
        {
            if (axis.sqrMagnitude == 0.0f)
                return identity;

            Quat result = identity;
            var radians = degress * degToRad;
            radians *= 0.5f;
            axis.Normalize();
            axis = axis * (float)System.Math.Sin(radians);
            result.x = axis.x;
            result.y = axis.y;
            result.z = axis.z;
            result.w = (float)System.Math.Cos(radians);

            return Normalize(result);
        }
        public void ToAngleAxis(out float angle, out Vec3 axis)
        {
            Quat.ToAxisAngleRad(this, out axis, out angle);
            angle *= radToDeg;
        }
        /// <summary>
        ///   <para>Creates a rotation which rotates from /fromDirection/ to /toDirection/.</para>
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        public static Quat FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            return RotateTowards(LookRotation(fromDirection), LookRotation(toDirection), float.MaxValue);
        }
        /// <summary>
        ///   <para>Creates a rotation which rotates from /fromDirection/ to /toDirection/.</para>
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            this = Quat.FromToRotation(fromDirection, toDirection);
        }
        /// <summary>
        ///   <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
        /// </summary>
        /// <param name="forward">The direction to look in.</param>
        /// <param name="upwards">The vector that defines in which direction up is.</param>
        public static Quat LookRotation(Vec3 forward, Vec3 upwards)
        {
            return Quat.LookRotation(ref forward, ref upwards);
        }
        public static Quat LookRotation(Vec3 forward)
        {
            Vec3 up = Vec3.up;
            return Quat.LookRotation(ref forward, ref up);
        }

        private static Quat LookRotation(ref Vec3 forward, ref Vec3 up)
        {

            forward = Vec3.Normalize(forward);
            Vec3 right = Vec3.Normalize(Vec3.Cross(up, forward));
            up = Vec3.Cross(forward, right);
            var m00 = right.x;
            var m01 = right.y;
            var m02 = right.z;
            var m10 = up.x;
            var m11 = up.y;
            var m12 = up.z;
            var m20 = forward.x;
            var m21 = forward.y;
            var m22 = forward.z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quat();
            if (num8 > 0f)
            {
                var num = (float)System.Math.Sqrt(num8 + 1f);
                quaternion.w = num * 0.5f;
                num = 0.5f / num;
                quaternion.x = (m12 - m21) * num;
                quaternion.y = (m20 - m02) * num;
                quaternion.z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = (float)System.Math.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.x = 0.5f * num7;
                quaternion.y = (m01 + m10) * num4;
                quaternion.z = (m02 + m20) * num4;
                quaternion.w = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)System.Math.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.x = (m10 + m01) * num3;
                quaternion.y = 0.5f * num6;
                quaternion.z = (m21 + m12) * num3;
                quaternion.w = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = (float)System.Math.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.x = (m20 + m02) * num2;
            quaternion.y = (m21 + m12) * num2;
            quaternion.z = 0.5f * num5;
            quaternion.w = (m01 - m10) * num2;
            return quaternion;
        }
        public void SetLookRotation(Vec3 view)
        {
            Vec3 up = Vec3.up;
            this.SetLookRotation(view, up);
        }
        /// <summary>
        ///   <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
        /// </summary>
        /// <param name="view">The direction to look in.</param>
        /// <param name="up">The vector that defines in which direction up is.</param>
        public void SetLookRotation(Vec3 view, Vec3 up)
        {
            this = Quat.LookRotation(view, up);
        }
        /// <summary>
        ///   <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is clamped to the range [0, 1].</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat Slerp(Quat a, Quat b, float t)
        {
            return Quat.Slerp(ref a, ref b, t);
        }
        private static Quat Slerp(ref Quat a, ref Quat b, float t)
        {
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return SlerpUnclamped(ref a, ref b, t);
        }
        /// <summary>
        ///   <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is not clamped.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat SlerpUnclamped(Quat a, Quat b, float t)
        {
            return Quat.SlerpUnclamped(ref a, ref b, t);
        }
        private static Quat SlerpUnclamped(ref Quat a, ref Quat b, float t)
        {
            // if either input is zero, return the other.
            if (a.LengthSquared == 0.0f)
            {
                if (b.LengthSquared == 0.0f)
                {
                    return identity;
                }
                return b;
            }
            else if (b.LengthSquared == 0.0f)
            {
                return a;
            }


            float cosHalfAngle = a.w * b.w + Vec3.Dot(a.xyz, b.xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return a;
            }
            else if (cosHalfAngle < 0.0f)
            {
                b.xyz = -b.xyz;
                b.w = -b.w;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - t;
                blendB = t;
            }

            Quat result = new Quat(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return identity;
        }
        /// <summary>
        ///   <para>Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is clamped to the range [0, 1].</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat Lerp(Quat a, Quat b, float t)
        {
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return Slerp(ref a, ref b, t); // TODO: use lerp not slerp, "Because quaternion works in 4D. Rotation in 4D are linear" ???
        }
        /// <summary>
        ///   <para>Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is not clamped.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat LerpUnclamped(Quat a, Quat b, float t)
        {
            return Slerp(ref a, ref b, t);
        }
        /// <summary>
        ///   <para>Rotates a rotation /from/ towards /to/.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        public static Quat RotateTowards(Quat from, Quat to, float maxDegreesDelta)
        {
            float num = Quat.Angle(from, to);
            if (num == 0f)
            {
                return to;
            }
            float t = Math.Min(1f, maxDegreesDelta / num);
            return Quat.SlerpUnclamped(from, to, t);
        }
        /// <summary>
        ///   <para>Returns the Inverse of /rotation/.</para>
        /// </summary>
        /// <param name="rotation"></param>
        public static Quat Inverse(Quat rotation)
        {
            float lengthSq = rotation.LengthSquared;
            if (lengthSq != 0.0)
            {
                float i = 1.0f / lengthSq;
                return new Quat(rotation.xyz * -i, rotation.w * i);
            }
            return rotation;
        }
        /// <summary>
        ///   <para>Returns a nicely formatted string of the Quat.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", this.x, this.y, this.z, this.w);
        }
        /// <summary>
        ///   <para>Returns a nicely formatted string of the Quat.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", this.x.ToString(format), this.y.ToString(format), this.z.ToString(format), this.w.ToString(format));
        }
        /// <summary>
        ///   <para>Returns the angle in degrees between two rotations /a/ and /b/.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Angle(Quat a, Quat b)
        {
            float f = Quat.Dot(a, b);
            return Mathfloat.Acos(Mathfloat.Min(Mathfloat.Abs(f), 1f)) * 2f * radToDeg;
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static Quat Euler(float x, float y, float z)
        {
            return Quat.FromEulerRad(new Vec3((float)x, (float)y, (float)z) * degToRad);
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="euler"></param>
        public static Quat Euler(Vec3 euler)
        {
            return Quat.FromEulerRad(euler * degToRad);
        }

        private static Vec3 ToEulerRad(Quat rotation)
        {
            float sqw = rotation.w * rotation.w;
            float sqx = rotation.x * rotation.x;
            float sqy = rotation.y * rotation.y;
            float sqz = rotation.z * rotation.z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = rotation.x * rotation.w - rotation.y * rotation.z;
            Vec3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.y = 2f * Mathfloat.Atan2(rotation.y, rotation.x);
                v.x = Mathfloat.PI / 2;
                v.z = 0;
                return NormalizeAngles(v * Mathfloat.Rad2Deg);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.y = -2f * Mathfloat.Atan2(rotation.y, rotation.x);
                v.x = -Mathfloat.PI / 2;
                v.z = 0;
                return NormalizeAngles(v * Mathfloat.Rad2Deg);
            }
            Quat q = new Quat(rotation.w, rotation.z, rotation.x, rotation.y);
            v.y = (float)System.Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
            v.x = (float)System.Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
            v.z = (float)System.Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
            return NormalizeAngles(v * Mathfloat.Rad2Deg);
        }
        private static Vec3 NormalizeAngles(Vec3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }
        private static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

        private static Quat FromEulerRad(Vec3 euler)
        {
            var yaw = euler.x;
            var pitch = euler.y;
            var roll = euler.z;
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)System.Math.Sin((float)rollOver2);
            float cosRollOver2 = (float)System.Math.Cos((float)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)System.Math.Sin((float)pitchOver2);
            float cosPitchOver2 = (float)System.Math.Cos((float)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)System.Math.Sin((float)yawOver2);
            float cosYawOver2 = (float)System.Math.Cos((float)yawOver2);
            Quat result;
            result.x = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
            result.z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.w = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            return result;

        }
        private static void ToAxisAngleRad(Quat q, out Vec3 axis, out float angle)
        {
            if (System.Math.Abs(q.w) > 1.0f)
                q.Normalize();
            angle = 2.0f * (float)System.Math.Acos(q.w); // angle
            float den = (float)System.Math.Sqrt(1.0 - q.w * q.w);
            if (den > 0.0001f)
            {
                axis = q.xyz / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                axis = new Vec3(1, 0, 0);
            }
        }
        #region Obsolete methods
        /*
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quat EulerRotation(float x, float y, float z)
        {
            return Quat.Internal_FromEulerRad(new Vec3(x, y, z));
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quat EulerRotation(Vec3 euler)
        {
            return Quat.Internal_FromEulerRad(euler);
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerRotation(float x, float y, float z)
        {
            this = Quaternion.Internal_FromEulerRad(new Vec3(x, y, z));
        }
        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerRotation(Vec3 euler)
        {
            this = Quaternion.Internal_FromEulerRad(euler);
        }
        [Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public Vec3 ToEuler()
        {
            return Quaternion.Internal_ToEulerRad(this);
        }
        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion EulerAngles(float x, float y, float z)
        {
            return Quaternion.Internal_FromEulerRad(new Vec3(x, y, z));
        }
        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion EulerAngles(Vec3 euler)
        {
            return Quaternion.Internal_FromEulerRad(euler);
        }
        [Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public void ToAxisAngle(out Vec3 axis, out float angle)
        {
            Quaternion.Internal_ToAxisAngleRad(this, out axis, out angle);
        }
        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerAngles(float x, float y, float z)
        {
            this.SetEulerRotation(new Vec3(x, y, z));
        }
        [Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerAngles(Vec3 euler)
        {
            this = Quaternion.EulerRotation(euler);
        }
        [Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public static Vec3 ToEulerAngles(Quaternion rotation)
        {
            return Quaternion.Internal_ToEulerRad(rotation);
        }
        [Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public Vec3 ToEulerAngles()
        {
            return Quaternion.Internal_ToEulerRad(this);
        }
        [Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quaternion AxisAngle(Vec3 axis, float angle)
        {
            return Quaternion.INTERNAL_CALL_AxisAngle(ref axis, angle);
        }
        private static Quaternion INTERNAL_CALL_AxisAngle(ref Vec3 axis, float angle)
        {
        }
        [Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetAxisAngle(Vec3 axis, float angle)
        {
            this = Quaternion.AxisAngle(axis, angle);
        }
        */
        #endregion
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
        }
        public override bool Equals(object other)
        {
            if (!(other is Quat))
            {
                return false;
            }
            Quat quaternion = (Quat)other;
            return this.x.Equals(quaternion.x) && this.y.Equals(quaternion.y) && this.z.Equals(quaternion.z) && this.w.Equals(quaternion.w);
        }
        public bool Equals(Quat other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
        }
        public static Quat operator *(Quat lhs, Quat rhs)
        {
            return new Quat(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }
        public static Vec3 operator *(Quat rotation, Vec3 point)
        {
            float num = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;
            Vec3 result;
            result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
            return result;
        }
        public static bool operator ==(Quat lhs, Quat rhs)
        {
            return Quat.Dot(lhs, rhs) > 0.999999f;
        }
        public static bool operator !=(Quat lhs, Quat rhs)
        {
            return Quat.Dot(lhs, rhs) <= 0.999999f;
        }
    }

    //public partial struct Quaternion : IEquatable<Quaternion>, IFormattable
    //{
    //    // X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
    //    public float x;
    //    // Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
    //    public float y;
    //    // Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
    //    public float z;
    //    // W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
    //    public float w;

    //    // Access the x, y, z, w components using [0], [1], [2], [3] respectively.
    //    public float this[int index]
    //    {
    //        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //        get
    //        {
    //            switch (index)
    //            {
    //                case 0: return x;
    //                case 1: return y;
    //                case 2: return z;
    //                case 3: return w;
    //                default:
    //                    throw new IndexOutOfRangeException("Invalid Quaternion index!");
    //            }
    //        }

    //        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //        set
    //        {
    //            switch (index)
    //            {
    //                case 0: x = value; break;
    //                case 1: y = value; break;
    //                case 2: z = value; break;
    //                case 3: w = value; break;
    //                default:
    //                    throw new IndexOutOfRangeException("Invalid Quaternion index!");
    //            }
    //        }
    //    }

    //    // Constructs new Quaternion with given x,y,z,w components.
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }

    //    // Set x, y, z and w components of an existing Quaternion.
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void Set(float newX, float newY, float newZ, float newW)
    //    {
    //        x = newX;
    //        y = newY;
    //        z = newZ;
    //        w = newW;
    //    }

    //    static readonly Quaternion identityQuaternion = new Quaternion(0F, 0F, 0F, 1F);

    //    // The identity rotation (RO). This quaternion corresponds to "no rotation": the object
    //    public static Quaternion identity
    //    {
    //        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //        get
    //        {
    //            return identityQuaternion;
    //        }
    //    }

    //    // Combines rotations /lhs/ and /rhs/.
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
    //    {
    //        return new Quaternion(
    //            lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
    //            lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
    //            lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
    //            lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
    //    }

    //    // Rotates the point /point/ with /rotation/.
    //    public static Vec3 operator *(Quaternion rotation, Vec3 point)
    //    {
    //        float x = rotation.x * 2F;
    //        float y = rotation.y * 2F;
    //        float z = rotation.z * 2F;
    //        float xx = rotation.x * x;
    //        float yy = rotation.y * y;
    //        float zz = rotation.z * z;
    //        float xy = rotation.x * y;
    //        float xz = rotation.x * z;
    //        float yz = rotation.y * z;
    //        float wx = rotation.w * x;
    //        float wy = rotation.w * y;
    //        float wz = rotation.w * z;

    //        Vec3 res;
    //        res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
    //        res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
    //        res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
    //        return res;
    //    }

    //    // *undocumented*
    //    public const float kEpsilon = 0.000001F;

    //    // Is the dot product of two quaternions within tolerance for them to be considered equal?
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    private static bool IsEqualUsingDot(float dot)
    //    {
    //        // Returns false in the presence of NaN values.
    //        return dot > 1.0f - kEpsilon;
    //    }

    //    // Are two quaternions equal to each other?
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static bool operator ==(Quaternion lhs, Quaternion rhs)
    //    {
    //        return IsEqualUsingDot(Dot(lhs, rhs));
    //    }

    //    // Are two quaternions different from each other?
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static bool operator !=(Quaternion lhs, Quaternion rhs)
    //    {
    //        // Returns true in the presence of NaN values.
    //        return !(lhs == rhs);
    //    }

    //    // The dot product between two rotations.
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static float Dot(Quaternion a, Quaternion b)
    //    {
    //        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    //    }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetLookRotation(Vec3 view)
    //    {
    //        Vec3 up = Vec3.up;
    //        SetLookRotation(view, up);
    //    }

    //    // Creates a rotation with the specified /forward/ and /upwards/ directions.
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetLookRotation(Vec3 view, Vec3 up)
    //    {
    //        this = LookRotation(view, up);
    //    }

    //    // Returns the angle in degrees between two rotations /a/ and /b/.
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static float Angle(Quaternion a, Quaternion b)
    //    {
    //        float dot = Mathfloat.Min(Mathfloat.Abs(Dot(a, b)), 1.0F);
    //        return IsEqualUsingDot(dot) ? 0.0f : Mathfloat.Acos(dot) * 2.0F * Mathfloat.Rad2Deg;
    //    }

    //    // Makes euler angles positive 0/360 with 0.0001 hacked to support old behaviour of QuaternionToEuler
    //    private static Vec3 Internal_MakePositive(Vec3 euler)
    //    {
    //        float negativeFlip = -0.0001f * Mathfloat.Rad2Deg;
    //        float positiveFlip = 360.0f + negativeFlip;

    //        if (euler.x < negativeFlip)
    //            euler.x += 360.0f;
    //        else if (euler.x > positiveFlip)
    //            euler.x -= 360.0f;

    //        if (euler.y < negativeFlip)
    //            euler.y += 360.0f;
    //        else if (euler.y > positiveFlip)
    //            euler.y -= 360.0f;

    //        if (euler.z < negativeFlip)
    //            euler.z += 360.0f;
    //        else if (euler.z > positiveFlip)
    //            euler.z -= 360.0f;

    //        return euler;
    //    }

    //    public Vec3 eulerAngles
    //    {
    //        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //        get { return Internal_MakePositive(Internal_ToEulerRad(this) * Mathfloat.Rad2Deg); }
    //        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //        set { this = Internal_FromEulerRad(value * Mathfloat.Deg2Rad); }
    //    }
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion Euler(float x, float y, float z) { return Internal_FromEulerRad(new Vec3(x, y, z) * Mathfloat.Deg2Rad); }
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion Euler(Vec3 euler) { return Internal_FromEulerRad(euler * Mathfloat.Deg2Rad); }
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void ToAngleAxis(out float angle, out Vec3 axis) { Internal_ToAxisAngleRad(this, out axis, out angle); angle *= Mathfloat.Rad2Deg; }
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection) { this = FromToRotation(fromDirection, toDirection); }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
    //    {
    //        float angle = Quaternion.Angle(from, to);
    //        if (angle == 0.0f) return to;
    //        return SlerpUnclamped(from, to, Mathfloat.Min(1.0f, maxDegreesDelta / angle));
    //    }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion Normalize(Quaternion q)
    //    {
    //        float mag = Mathfloat.Sqrt(Dot(q, q));

    //        if (mag < Mathfloat.Epsilon)
    //            return Quaternion.identity;

    //        return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
    //    }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void Normalize()
    //    {
    //        this = Normalize(this);
    //    }

    //    public Quaternion normalized
    //    {
    //        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //        get { return Normalize(this); }
    //    }

    //    // used to allow Quaternions to be used as keys in hash tables
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public override int GetHashCode()
    //    {
    //        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
    //    }

    //    // also required for being able to use Quaternions as keys in hash tables
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public override bool Equals(object other)
    //    {
    //        if (!(other is Quaternion)) return false;

    //        return Equals((Quaternion)other);
    //    }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public bool Equals(Quaternion other)
    //    {
    //        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    //    }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public override string ToString()
    //    {
    //        return ToString(null, null);
    //    }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public string ToString(string format)
    //    {
    //        return ToString(format, null);
    //    }

    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public string ToString(string format, IFormatProvider formatProvider)
    //    {
    //        if (string.IsNullOrEmpty(format))
    //            format = "F5";
    //        if (formatProvider == null)
    //            formatProvider = CultureInfo.InvariantCulture.NumberFormat;
    //        return UnityString.Format("({0}, {1}, {2}, {3})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
    //    }

    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    static public Quaternion EulerRotation(float x, float y, float z) { return Internal_FromEulerRad(new Vec3(x, y, z)); }
    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion EulerRotation(Vec3 euler) { return Internal_FromEulerRad(euler); }
    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetEulerRotation(float x, float y, float z) { this = Internal_FromEulerRad(new Vec3(x, y, z)); }
    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetEulerRotation(Vec3 euler) { this = Internal_FromEulerRad(euler); }
    //    [System.Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public Vec3 ToEuler() { return Internal_ToEulerRad(this); }
    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    static public Quaternion EulerAngles(float x, float y, float z) { return Internal_FromEulerRad(new Vec3(x, y, z)); }
    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion EulerAngles(Vec3 euler) { return Internal_FromEulerRad(euler); }
    //    [System.Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void ToAxisAngle(out Vec3 axis, out float angle) { Internal_ToAxisAngleRad(this, out axis, out angle); }
    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetEulerAngles(float x, float y, float z) { SetEulerRotation(new Vec3(x, y, z)); }
    //    [System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetEulerAngles(Vec3 euler) { this = EulerRotation(euler); }
    //    [System.Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Vec3 ToEulerAngles(Quaternion rotation) { return Quaternion.Internal_ToEulerRad(rotation); }
    //    [System.Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public Vec3 ToEulerAngles() { return Quaternion.Internal_ToEulerRad(this); }
    //    [System.Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public void SetAxisAngle(Vec3 axis, float angle) { this = AxisAngle(axis, angle); }
    //    [System.Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
    //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    //    public static Quaternion AxisAngle(Vec3 axis, float angle) { return AngleAxis(Mathfloat.Rad2Deg * angle, axis); }
    //}
}
