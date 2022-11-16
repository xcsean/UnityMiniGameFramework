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
    public partial struct Ray : IFormattable
    {
        private Vec3 m_Origin;
        private Vec3 m_Direction;

        // Creates a ray starting at /origin/ along /direction/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Ray(Vec3 origin, Vec3 direction)
        {
            m_Origin = origin;
            m_Direction = direction.normalized;
        }

        // The origin point of the ray.
        public Vec3 origin
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return m_Origin; }
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            set { m_Origin = value; }
        }

        // The direction of the ray.
        public Vec3 direction
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return m_Direction; }
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            set { m_Direction = value.normalized; }
        }

        // Returns a point at /distance/ units along the ray.
        public Vec3 GetPoint(float distance)
        {
            return m_Origin + m_Direction * distance;
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
            //return UnityString.Format("Origin: {0}, Dir: {1}", m_Origin.ToString(format, formatProvider), m_Direction.ToString(format, formatProvider));
            return $"Origin: {m_Origin.ToString(format, formatProvider)}, Dir: {m_Direction.ToString(format, formatProvider)}";
        }
    }
}
