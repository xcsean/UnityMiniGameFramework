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
    public partial struct Plane : IFormattable
    {
        // sizeof(Plane) is not const in C# and so cannot be used in fixed arrays, so we define it here
        internal const int size = 16;

        Vec3 m_Normal;
        float m_Distance;

        // Normal vector of the plane.
        public Vec3 normal
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return m_Normal; }
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            set { m_Normal = value; }
        }
        // Distance from the origin to the plane.
        public float distance
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return m_Distance; }
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            set { m_Distance = value; }
        }

        // Creates a plane.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Plane(Vec3 inNormal, Vec3 inPoint)
        {
            m_Normal = Vec3.Normalize(inNormal);
            m_Distance = -Vec3.Dot(m_Normal, inPoint);
        }

        // Creates a plane.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Plane(Vec3 inNormal, float d)
        {
            m_Normal = Vec3.Normalize(inNormal);
            m_Distance = d;
        }

        // Creates a plane.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Plane(Vec3 a, Vec3 b, Vec3 c)
        {
            m_Normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
            m_Distance = -Vec3.Dot(m_Normal, a);
        }

        // Sets a plane using a point that lies within it plus a normal to orient it
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void SetNormalAndPosition(Vec3 inNormal, Vec3 inPoint)
        {
            m_Normal = Vec3.Normalize(inNormal);
            m_Distance = -Vec3.Dot(m_Normal, inPoint);
        }

        // Sets a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Set3Points(Vec3 a, Vec3 b, Vec3 c)
        {
            m_Normal = Vec3.Normalize(Vec3.Cross(b - a, c - a));
            m_Distance = -Vec3.Dot(m_Normal, a);
        }

        // Make the plane face the opposite direction
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Flip() { m_Normal = -m_Normal; m_Distance = -m_Distance; }

        // Return a version of the plane that faces the opposite direction
        public Plane flipped
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return new Plane(-m_Normal, -m_Distance); }
        }

        // Translates the plane into a given direction
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Translate(Vec3 translation) { m_Distance += Vec3.Dot(m_Normal, translation); }

        // Creates a plane that's translated into a given direction
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Plane Translate(Plane plane, Vec3 translation) { return new Plane(plane.m_Normal, plane.m_Distance += Vec3.Dot(plane.m_Normal, translation)); }

        // Calculates the closest point on the plane.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Vec3 ClosestPointOnPlane(Vec3 point)
        {
            var pointToPlaneDistance = Vec3.Dot(m_Normal, point) + m_Distance;
            return point - (m_Normal * pointToPlaneDistance);
        }

        // Returns a signed distance from plane to point.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public float GetDistanceToPoint(Vec3 point) { return Vec3.Dot(m_Normal, point) + m_Distance; }

        // Is a point on the positive side of the plane?
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public bool GetSide(Vec3 point) { return Vec3.Dot(m_Normal, point) + m_Distance > 0.0F; }

        // Are two points on the same side of the plane?
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public bool SameSide(Vec3 inPt0, Vec3 inPt1)
        {
            float d0 = GetDistanceToPoint(inPt0);
            float d1 = GetDistanceToPoint(inPt1);
            return (d0 > 0.0f && d1 > 0.0f) ||
                (d0 <= 0.0f && d1 <= 0.0f);
        }

        // Intersects a ray with the plane.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public bool Raycast(Ray ray, out float enter)
        {
            float vdot = Vec3.Dot(ray.direction, m_Normal);
            float ndot = -Vec3.Dot(ray.origin, m_Normal) - m_Distance;

            if (Mathfloat.Approximately(vdot, 0.0f))
            {
                enter = 0.0F;
                return false;
            }

            enter = ndot / vdot;

            return enter > 0.0F;
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
            //return UnityString.Format("(normal:{0}, distance:{1})", m_Normal.ToString(format, formatProvider), m_Distance.ToString(format, formatProvider));
            return $"(normal:{m_Normal.ToString(format, formatProvider)}, distance:{m_Distance.ToString(format, formatProvider)})";
        }
    }
}
