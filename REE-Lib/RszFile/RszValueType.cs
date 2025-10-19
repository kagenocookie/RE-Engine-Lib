using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using ReeLib.Pfb;
using ReeLib.Scn;

namespace ReeLib.via
{
    public static class ValueTypeUtils
    {
        public static void CheckIndex(int index, int length)
        {
            if (index < 0 || index >= length) throw new IndexOutOfRangeException($"Index must be 0..{length-1}, got {index}");
        }

        public static IndexOutOfRangeException IndexError(int index, int length)
        {
            return new IndexOutOfRangeException($"Index must be 0..{length-1}, got {index}");
        }
    }


    public struct Int2
    {
        public int x;
        public int y;

        public readonly override string ToString()
        {
            return $"<{x}, {y}>";
        }

        public int this[int index]
        {
            readonly get
            {
                ValueTypeUtils.CheckIndex(index, 2);
                return index == 0 ? x : y;
            }
            set
            {
                ValueTypeUtils.CheckIndex(index, 2);
                if (index == 0) x = value;
                else y = value;
            }
        }
    }


    public struct Int3
    {
        public int x;
        public int y;
        public int z;

        public readonly override string ToString()
        {
            return $"<{x}, {y}, {z}>";
        }

        public int this[int index]
        {
            readonly get => index switch {
                0 => x,
                1 => y,
                2 => z,
                _ => throw ValueTypeUtils.IndexError(index, 3)
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default: throw ValueTypeUtils.IndexError(index, 3);
                }
            }
        }
    }


    public struct Int4
    {
        public int x;
        public int y;
        public int z;
        public int w;

        public readonly override string ToString()
        {
            return $"<{x}, {y}, {z}, {w}>";
        }

        public int this[int index]
        {
            readonly get => index switch {
                0 => x,
                1 => y,
                2 => z,
                3 => w,
                _ => throw ValueTypeUtils.IndexError(index, 4)
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default: throw ValueTypeUtils.IndexError(index, 4);
                }
            }
        }
    }


    public struct Uint2
    {
        public uint x;
        public uint y;

        public readonly override string ToString()
        {
            return $"<{x}, {y}>";
        }

        public uint this[int index]
        {
            readonly get
            {
                ValueTypeUtils.CheckIndex(index, 2);
                return index == 0 ? x : y;
            }
            set
            {
                ValueTypeUtils.CheckIndex(index, 2);
                if (index == 0) x = value;
                else y = value;
            }
        }
    }


    public struct Uint3
    {
        public uint x;
        public uint y;
        public uint z;

        public readonly override string ToString()
        {
            return $"<{x}, {y}, {z}>";
        }

        public uint this[int index]
        {
            readonly get => index switch {
                0 => x,
                1 => y,
                2 => z,
                _ => throw ValueTypeUtils.IndexError(index, 3)
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default: throw ValueTypeUtils.IndexError(index, 3);
                }
            }
        }
    }


    public struct Uint4
    {
        public uint x;
        public uint y;
        public uint z;
        public uint w;

        public readonly override string ToString()
        {
            return $"<{x}, {y}, {z}, {w}>";
        }

        public uint this[int index]
        {
            readonly get => index switch {
                0 => x,
                1 => y,
                2 => z,
                3 => w,
                _ => throw ValueTypeUtils.IndexError(index, 4)
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default: throw ValueTypeUtils.IndexError(index, 4);
                }
            }
        }
    }


    // Size=8
    public struct Range
    {
        public float s;
        public float r;

        public readonly override string ToString()
        {
            return $"Range({s}, {r})";
        }

        public static explicit operator Range(Vector2 vector)
        {
            return new Range { s = vector.X, r = vector.Y };
        }

        public static explicit operator Vector2(Range range)
        {
            return new Vector2(range.s, range.r);
        }

        public float this[int index]
        {
            readonly get
            {
                ValueTypeUtils.CheckIndex(index, 2);
                return index == 0 ? s : r;
            }
            set
            {
                ValueTypeUtils.CheckIndex(index, 2);
                if (index == 0) s = value;
                else r = value;
            }
        }
    }


    // Size=8
    public struct RangeI
    {
        public int r;
        public int s;

        public readonly override string ToString()
        {
            return $"Range({r}, {s})";
        }

        public int this[int index]
        {
            readonly get
            {
                ValueTypeUtils.CheckIndex(index, 2);
                return index == 0 ? r : s;
            }
            set
            {
                ValueTypeUtils.CheckIndex(index, 2);
                if (index == 0) r = value;
                else s = value;
            }
        }
    }


    // Size=4
    public struct Color : IEquatable<Color>
    {
        public uint rgba;

        public Color(uint rgba)
        {
            this.rgba = rgba;
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            rgba = r + ((uint)(g) << 8) + ((uint)(b) << 16) + ((uint)(a) << 24);
        }

        [JsonIgnore]
        public int R { readonly get => (int)(rgba >> 0) & 0xff; set => rgba = (rgba & 0xffffff00) | ((uint)value & 0xff); }
        [JsonIgnore]
        public int G { readonly get => (int)(rgba >> 8) & 0xff; set => rgba = (rgba & 0xffff00ff) | (((uint)value & 0xff) << 8); }
        [JsonIgnore]
        public int B { readonly get => (int)(rgba >> 16) & 0xff; set => rgba = (rgba & 0xff00ffff) | (((uint)value & 0xff) << 16); }
        [JsonIgnore]
        public int A { readonly get => (int)(rgba >> 24) & 0xff; set => rgba = (rgba & 0x00ffffff) | (((uint)value & 0xff) << 24); }

        [JsonIgnore]
        public readonly int ARGB => A + (R << 8) + (G << 16) + (B << 24);
        [JsonIgnore]
        public readonly int ABGR => A + (B << 8) + (G << 16) + (R << 24);
        [JsonIgnore]
        public readonly int BGRA => B + (G << 8) + (R << 16) + (A << 24);

        public readonly string Hex()
        {
            return rgba.ToString("X8");
        }

        public readonly override string ToString()
        {
            return Hex();
        }

        /// <summary>
        /// Parse a hex string
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color Parse(string hex)
        {
            return new Color { rgba = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber) };
        }

        public static bool TryParse(string hex, out Color color)
        {
            if (uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsed)) {
                color = new Color { rgba = parsed };
                return true;
            }
            color = default;
            return false;
        }

        public readonly Vector4 ToVector4() => new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);
        public readonly Color Inverse() => new Color((byte)(255 - R), (byte)(255 - G), (byte)(255 - B), (byte)A);

        public static Color FromVector4(Vector4 vec) => new Color((byte)(vec.X * 255), (byte)(vec.Y * 255), (byte)(vec.Z * 255), (byte)(vec.W * 255));

        public bool Equals(Color other) => other.rgba == rgba;
        public override bool Equals(object? obj) => obj is Color col && col.rgba == rgba;
        public static bool operator ==(Color left, Color right) => left.rgba == right.rgba;
        public static bool operator !=(Color left, Color right) => left.rgba != right.rgba;
        public override int GetHashCode() => rgba.GetHashCode();
    }


    // Size=64
    public struct mat4 {
        /// <summary>Row 1, Column 1</summary>
        public float m00;
        /// <summary>Row 1, Column 2</summary>
        public float m01;
        /// <summary>Row 1, Column 3</summary>
        public float m02;
        /// <summary>Row 1, Column 4</summary>
        public float m03;
        /// <summary>Row 2, Column 1</summary>
        public float m10;
        /// <summary>Row 2, Column 2</summary>
        public float m11;
        /// <summary>Row 2, Column 3</summary>
        public float m12;
        /// <summary>Row 2, Column 4</summary>
        public float m13;
        /// <summary>Row 3, Column 1</summary>
        public float m20;
        /// <summary>Row 3, Column 2</summary>
        public float m21;
        /// <summary>Row 3, Column 3</summary>
        public float m22;
        /// <summary>Row 3, Column 4</summary>
        public float m23;
        /// <summary>Row 4, Column 1</summary>
        public float m30;
        /// <summary>Row 4, Column 2</summary>
        public float m31;
        /// <summary>Row 4, Column 3</summary>
        public float m32;
        /// <summary>Row 4, Column 4</summary>
        public float m33;

        public static readonly mat4 Identity = new mat4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        public mat4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33) : this()
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m03 = m03;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m30 = m30;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        public float this[int index]
        {
            get
            {
                ValueTypeUtils.CheckIndex(index, 16);
                ref float ptr = ref m00;
                return Unsafe.Add(ref ptr, index);
            }
            set
            {
                ValueTypeUtils.CheckIndex(index, 16);
                ref float ptr = ref m00;
                Unsafe.Add(ref ptr, index) = value;
            }
        }

        [JsonIgnore]
        public unsafe Vector4 Row0 { readonly get => new Vector4(m00, m01, m02, m03); set { fixed(void* x = &m00) Unsafe.Write<Vector4>(x, value); } }
        [JsonIgnore]
        public unsafe Vector4 Row1 { readonly get => new Vector4(m10, m11, m12, m13); set { fixed(void* x = &m10) Unsafe.Write<Vector4>(x, value); } }
        [JsonIgnore]
        public unsafe Vector4 Row2 { readonly get => new Vector4(m20, m21, m22, m23); set { fixed(void* x = &m20) Unsafe.Write<Vector4>(x, value); } }
        [JsonIgnore]
        public unsafe Vector4 Row3 { readonly get => new Vector4(m30, m31, m32, m33); set { fixed(void* x = &m30) Unsafe.Write<Vector4>(x, value); } }

        /* public float this[int row, int col]
        {
            get => this[row * 4 + col];
            set => this[row * 4 + col] = value;
        } */

        public readonly Vector3 Multiply(Vector3 vector)
        {
            return new Vector3(
                m00 * vector.X + m10 * vector.Y + m20 * vector.Z + m30,
                m01 * vector.X + m11 * vector.Y + m21 * vector.Z + m31,
                m02 * vector.X + m12 * vector.Y + m22 * vector.Z + m32) / (m03 * vector.X + m13 * vector.Y + m23 * vector.Z + m33);
        }

        public readonly Matrix4x4 ToSystem()
        {
            return new Matrix4x4(
                m00, m01, m02, m03,
                m10, m11, m12, m13,
                m20, m21, m22, m23,
                m30, m31, m32, m33
            );
        }

        public static implicit operator mat4(Matrix4x4 matrix) => new mat4(
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
        );

        public override string ToString() => $"[Pos: {m30} {m31} {m32}]";
    }


    [StructLayout(LayoutKind.Explicit, Size = 80)]
    public struct OBB
    {
        [FieldOffset(0), JsonIgnore]
        private mat4 coord;
        /// <summary>
        /// The full size of the box (from one edge to the other)
        /// </summary>
        [FieldOffset(64), JsonIgnore]
        private Vector3 extent;

        public OBB()
        {
            coord = mat4.Identity;
            extent = Vector3.One;
        }

        public OBB(mat4 coord, Vector3 extent)
        {
            this.coord = coord;
            this.extent = extent;
        }

        public mat4 Coord { readonly get => coord; set => coord = value; }
        public Vector3 Extent { readonly get => extent; set => extent = value; }

        public readonly AABB GetBounds(float margin = 0)
        {
            var aabb = AABB.MaxMin;
            // there's probably faster ways to do this but I'm no math guru ¯\_(ツ)_/¯
            var size = extent / 2 + new Vector3(margin);
            aabb = aabb.Extend(coord.Multiply(new Vector3(size.X, size.Y, size.Z)));
            aabb = aabb.Extend(coord.Multiply(new Vector3(size.X, size.Y, -size.Z)));
            aabb = aabb.Extend(coord.Multiply(new Vector3(size.X, -size.Y, size.Z)));
            aabb = aabb.Extend(coord.Multiply(new Vector3(size.X, -size.Y, -size.Z)));
            aabb = aabb.Extend(coord.Multiply(new Vector3(-size.X, size.Y, size.Z)));
            aabb = aabb.Extend(coord.Multiply(new Vector3(-size.X, size.Y, -size.Z)));
            aabb = aabb.Extend(coord.Multiply(new Vector3(-size.X, -size.Y, size.Z)));
            aabb = aabb.Extend(coord.Multiply(new Vector3(-size.X, -size.Y, -size.Z)));
            return aabb;
        }

        public override string ToString() => $"[Pos: {coord.m30} {coord.m31} {coord.m32}, Extent: {extent}]";
    }


    // Size=16
    public struct Sphere
    {
        [JsonIgnore]
        public Vector3 pos;
        [JsonIgnore]
        public float r;

        public Vector3 Pos { readonly get => pos; set => pos = value; }
        public float R { readonly get => r; set => r = value; }

        public Sphere(Vector3 pos, float r)
        {
            this.pos = pos;
            this.r = r;
        }

        public Sphere()
        {
            r = 1;
        }

        public readonly override string ToString()
        {
            return $"Sphere({pos}, {r})";
        }

        public readonly AABB GetBounds(float margin = 0)
        {
            var vec = new Vector3(r + margin);
            return new AABB(pos - vec, pos + vec);
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct AABB
    {
        [FieldOffset(0), JsonIgnore]
        public Vector3 minpos;
        [FieldOffset(16), JsonIgnore]
        public Vector3 maxpos;

        public AABB()
        {
        }

        public AABB(Vector3 minpos, Vector3 maxpos)
        {
            this.minpos = minpos;
            this.maxpos = maxpos;
        }

        public Vector3 Minpos { readonly get => minpos; set => minpos = value; }
        public Vector3 Maxpos { readonly get => maxpos; set => maxpos = value; }

        public readonly Vector3 Size => maxpos - minpos;
        public readonly Vector3 Center => (minpos + maxpos) / 2;

        public readonly bool IsEmpty => minpos == maxpos;
        public readonly bool IsInvalid => minpos.X == float.MaxValue;

        public static readonly AABB MaxMin = new ReeLib.via.AABB(new System.Numerics.Vector3(float.MaxValue), new System.Numerics.Vector3(float.MinValue));
        public static readonly AABB Invalid = new ReeLib.via.AABB(new System.Numerics.Vector3(float.MaxValue), new System.Numerics.Vector3(float.MaxValue));

        public readonly AABB Extend(Vector3 point)
        {
            return new AABB(Vector3.Min(minpos, point), Vector3.Max(maxpos, point));
        }

        public readonly AABB Extend(AABB other)
        {
            return new AABB(Vector3.Min(minpos, other.minpos), Vector3.Max(maxpos, other.maxpos));
        }

        public readonly AABB Margin(float margin)
        {
            var mv = new Vector3(margin);
            return new AABB(minpos - mv, maxpos + mv);
        }

        public static AABB Combine(IEnumerable<AABB> bounds) => bounds.Aggregate(MaxMin, (bound, item) => bound.Extend(item));

        public readonly override string ToString() => $"AABB({minpos} {maxpos})";
    }

    public record struct AABBVec4(Vector4 Minpos, Vector4 Maxpos)
    {
        public readonly bool IsEmpty => Minpos == Vector4.Zero && Maxpos == Vector4.Zero;
        public readonly AABB AsAABB => new AABB(new Vector3(Minpos.X, Minpos.Y, Minpos.Z), new Vector3(Maxpos.X, Maxpos.Y, Maxpos.Z));

        public static implicit operator AABBVec4(AABB src) => new AABBVec4(new Vector4(src.minpos.X, src.minpos.Y, src.minpos.Z, 1), new Vector4(src.maxpos.X, src.maxpos.Y, src.maxpos.Z, 1));
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Capsule
    {
        [FieldOffset(0), JsonIgnore]
        public Vector3 p0;
        [FieldOffset(16), JsonIgnore]
        public Vector3 p1;
        [FieldOffset(32), JsonIgnore]
        public float r;

        public Vector3 P0 { readonly get => p0; set => p0 = value; }
        public Vector3 P1 { readonly get => p1; set => p1 = value; }
        public float R { readonly get => r; set => r = value; }

        public Capsule()
        {
            r = 1;
        }

        public Capsule(Vector3 p0, Vector3 p1, float r)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.r = r;
        }

        public readonly AABB GetBounds(float margin = 0)
        {
            AABB b1 = new Sphere { r = r, pos = p0 }.GetBounds(margin);
            AABB b2 = new Sphere { r = r, pos = p1 }.GetBounds(margin);
            return b1.Extend(b2);
        }

        public readonly override string ToString() => $"Capsule(P1={p0}, P2={p1}, R={r})";
    }


    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Area
    {
        [FieldOffset(0)]
        public Vector2 p0;
        [FieldOffset(8)]
        public Vector2 p1;
        [FieldOffset(16)]
        public Vector2 p2;
        [FieldOffset(24)]
        public Vector2 p3;
        [FieldOffset(32)]
        public float height;
        [FieldOffset(36)]
        public float bottom;

        public Vector2 P0 { readonly get => p0; set => p0 = value; }
        public Vector2 P1 { readonly get => p1; set => p1 = value; }
        public Vector2 P2 { readonly get => p2; set => p2 = value; }
        public Vector2 P3 { readonly get => p3; set => p3 = value; }
        public float Height { readonly get => height; set => height = value; }
        public float Bottom { readonly get => bottom; set => bottom = value; }
    }


    public struct Position : IEquatable<Position>
    {
        public double x;
        public double y;
        public double z;

        public double this[int index]
        {
            readonly get => index switch {
                0 => x,
                1 => y,
                2 => z,
                _ => throw ValueTypeUtils.IndexError(index, 3)
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default: throw ValueTypeUtils.IndexError(index, 3);
                }
            }
        }

        public Vector3 AsVector3 => new Vector3((float)x, (float)y, (float)z);

        public bool Equals(Position other) => other == this;
        public static bool operator==(Position p1, Position p2) => p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
        public static bool operator!=(Position p1, Position p2) => p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;

        public readonly override string ToString() => $"Position({x}, {y}, {z})";

        public override bool Equals(object? obj) => obj is Position && (Position)obj == this;
        public override int GetHashCode() => HashCode.Combine(x, y, z);
    }


    // Size=32
    public struct TaperedCapsule
    {
        private Vector4 vertexRadiusA;
        private Vector4 vertexRadiusB;

        public Vector4 VertexRadiusA { readonly get => vertexRadiusA; set => vertexRadiusA = value; }
        public Vector4 VertexRadiusB { readonly get => vertexRadiusB; set => vertexRadiusB = value; }

        public readonly override string ToString() => $"TaperedCapsule({vertexRadiusA}, {vertexRadiusB})";
    }


    // Size=32
    public struct Cone
    {
        public Vector3 p0;
        public float r0;
        public Vector3 p1;
        public float r1;

        public readonly override string ToString() => $"Cone({p0} ({r0}), {p1} ({r1}))";
    }


    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Line
    {
        [FieldOffset(0)]
        public Vector3 from;
        [FieldOffset(16)]
        public Vector3 dir;

        public Line(Vector3 from, Vector3 dir)
        {
            this.from = from;
            this.dir = dir;
        }

        public readonly override string ToString() => $"Line({from} -> {dir})";
    }


    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct LineSegment
    {
        [FieldOffset(0)]
        public Vector3 start;
        [FieldOffset(16)]
        public Vector3 end;

        public LineSegment(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
        }

        public readonly override string ToString() => $"LineSegment({start} -> {end})";
    }


    // Size=16
    public struct Plane
    {
        public Vector3 normal;
        public float dist;

        public Plane(Vector3 normal, float dist)
        {
            this.normal = normal;
            this.dist = dist;
        }

        public Plane(float x, float y, float z, float dist)
        {
            this.normal = new Vector3(x, y, z);
            this.dist = dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInFront(Vector3 point) => Vector3.Dot(point, normal) + dist > 0;

        public Plane Normalize()
        {
            float invLen = 1.0f / normal.Length();
            return new Plane(normal * invLen, dist * invLen);
        }

        public readonly override string ToString() => $"Plane({normal}, Dist = {dist})>";
    }


    // Size=4
    public struct PlaneXZ
    {
        public float dist;

        public readonly override string ToString() => dist.ToString();
    }


    // Size=8
    public struct Point
    {
        public float x;
        public float y;

        public readonly override string ToString() => $"<{x}, {y}>";
    }


    // Size=8
    public struct Size
    {
        public float w;
        public float h;

        public float this[int index]
        {
            readonly get
            {
                ValueTypeUtils.CheckIndex(index, 2);
                return index == 0 ? w : h;
            }
            set
            {
                ValueTypeUtils.CheckIndex(index, 2);
                if (index == 0) w = value;
                else h = value;
            }
        }

        public readonly override string ToString() => $"<{w}, {h}>";
    }


    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Ray
    {
        [FieldOffset(0)]
        public Vector3 from;
        [FieldOffset(16)]
        public Vector3 dir;

        public readonly override string ToString() => $"Ray({from} -> {dir})";
    }


    // Size=16
    public struct RayY
    {
        public Vector3 from;
        public float dir;

        public readonly override string ToString() => $"RayY({from}, Dir={dir})";
    }


    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Segment
    {
        [FieldOffset(0)]
        public Vector4 from;
        [FieldOffset(16)]
        public Vector3 dir;

        public readonly override string ToString() => $"Segment({from} -> {dir})";
    }


    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Triangle
    {
        [FieldOffset(0)]
        public Vector3 p0;
        [FieldOffset(16)]
        public Vector3 p1;
        [FieldOffset(32)]
        public Vector3 p2;

        public readonly override string ToString() => $"Triangle({p1} {p1} {p2})";
    }


    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Cylinder
    {
        [FieldOffset(0)]
        public Vector3 p0;
        [FieldOffset(16)]
        public Vector3 p1;
        [FieldOffset(32)]
        public float r;

        public Cylinder()
        {
            r = 1;
        }

        public Cylinder(Vector3 p0, Vector3 p1, float r)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.r = r;
        }

        public readonly override string ToString() => $"Cylinder(P1={p0}, P2={p1}, R={r})";
    }


    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Ellipsoid
    {
        [FieldOffset(0)]
        public Vector3 pos;
        [FieldOffset(16)]
        public Vector3 r;

        public readonly override string ToString() => $"Ellipsoid(Pos={pos}, R={r})";
    }


    // Size=32
    public struct Torus
    {
        public Vector3 pos;
        public float r;
        public Vector3 axis;
        public float cr;

        public readonly override string ToString() => $"Torus(pos={pos}, r={r}, axis={axis}, cr={cr})";
    }


    // Size=16
    public struct Rect
    {
        public float left;
        public float top;
        public float right;
        public float bottom;

        public Rect()
        {
        }

        public Rect(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public float this[int index]
        {
            get
            {
                ValueTypeUtils.CheckIndex(index, 4);
                ref float ptr = ref left;
                return Unsafe.Add(ref ptr, index);
            }
            set
            {
                ValueTypeUtils.CheckIndex(index, 4);
                ref float ptr = ref left;
                Unsafe.Add(ref ptr, index) = value;
            }
        }

        public readonly Vector4 AsVector => new Vector4(left, top, right, bottom);

        public readonly override string ToString() => AsVector.ToString();
    }


    // Size=32
    public struct Rect3D
    {
        public Vector3 normal;
        public float sizeW;
        public Vector3 center;
        public float sizeH;
    }


    // Size=96
    public struct Frustum
    {
        public Plane plane0;
        public Plane plane1;
        public Plane plane2;
        public Plane plane3;
        public Plane plane4;
        public Plane plane5;

        public Plane this[int index]
        {
            get
            {
                ref Plane ptr = ref plane0;
                return Unsafe.Add(ref ptr, index);
            }
            set
            {
                ref Plane ptr = ref plane0;
                Unsafe.Add(ref ptr, index) = value;
            }
        }
    }


    // Size=16
    public struct KeyFrame
    {
        public float value;
        public short curveType;
        public Half time;
        public Half inNormalY;
        public Half inNormalX;
        public Half outNormalY;

        public override string ToString() => $"{time}: {value}";
    }


#pragma warning disable CS8981
    public struct sfix
    {
        public int v;
    }
#pragma warning restore CS8981


    public struct Sfix2
    {
        public sfix x;
        public sfix y;
    }


    public struct Sfix3
    {
        public sfix x;
        public sfix y;
        public sfix z;
    }


    public struct Sfix4
    {
        public sfix x;
        public sfix y;
        public sfix z;
        public sfix w;
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Transform
    {
        [FieldOffset(0)]
        public Vector3 pos;
        [FieldOffset(16)]
        public Quaternion rot;
        [FieldOffset(32)]
        public Vector3 scale;

        public Transform(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            this.pos = pos;
            this.rot = rot;
            this.scale = scale;
        }

        public override string ToString() => $"[T: {pos}] [R: {rot}] [S: {scale}]";
    }

    public class GameObjectRef
    {
        public Guid guid;
        public IGameObject? target;

        public GameObjectRef() { }

        public GameObjectRef(Guid guid)
        {
            this.guid = guid;
        }

        public GameObjectRef(Guid guid, IGameObject target)
        {
            this.guid = guid;
            this.target = target;
        }

        public GameObjectRef(ScnGameObject gameObj)
        {
            this.guid = gameObj.Guid;
            target = gameObj;
        }

        public GameObjectRef(PfbGameObject gameObj)
        {
            target = gameObj;
        }

        public override string ToString() => $"Ref<{guid} => {target}>";
    }
}
