using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        public float r;
        public float s;

        public readonly override string ToString()
        {
            return $"Range({r}, {s})";
        }

        public static explicit operator Range(Vector2 vector)
        {
            return new Range { r = vector.X, s = vector.Y };
        }

        public static explicit operator Vector2(Range range)
        {
            return new Vector2(range.r, range.s);
        }

        public float this[int index]
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

        public readonly int R => (int)(rgba >> 0) & 0xff;
        public readonly int G => (int)(rgba >> 8) & 0xff;
        public readonly int B => (int)(rgba >> 16) & 0xff;
        public readonly int A => (int)(rgba >> 24) & 0xff;

        public readonly int ARGB => A + (R << 8) + (G << 16) + (B << 24);
        public readonly int ABGR => A + (B << 8) + (G << 16) + (R << 24);
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
        public float m00;
        public float m01;
        public float m02;
        public float m03;
        public float m10;
        public float m11;
        public float m12;
        public float m13;
        public float m20;
        public float m21;
        public float m22;
        public float m23;
        public float m30;
        public float m31;
        public float m32;
        public float m33;

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
    }


    [StructLayout(LayoutKind.Explicit, Size = 80)]
    public struct OBB
    {
        [FieldOffset(0)]
        private mat4 coord;
        /// <summary>
        /// The full size of the box (from one edge to the other)
        /// </summary>
        [FieldOffset(64)]
        private Vector3 extent;

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
    }


    // Size=16
    public struct Sphere
    {
        public Vector3 pos;
        public float r;

        public Vector3 Pos { readonly get => pos; set => pos = value; }
        public float R { readonly get => r; set => r = value; }

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
        [FieldOffset(0)]
        public Vector3 minpos;
        [FieldOffset(16)]
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

        public static readonly AABB MaxMin = new ReeLib.via.AABB(new System.Numerics.Vector3(float.MaxValue), new System.Numerics.Vector3(float.MinValue));

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


    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Capsule
    {
        [FieldOffset(0)]
        public Vector3 p0;
        [FieldOffset(16)]
        public Vector3 p1;
        [FieldOffset(32)]
        public float r;

        public Vector3 P0 { readonly get => p0; set => p0 = value; }
        public Vector3 P1 { readonly get => p1; set => p1 = value; }
        public float R { readonly get => r; set => r = value; }

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

        public readonly override string ToString() => $"Line({from} -> {dir})";
    }


    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct LineSegment
    {
        [FieldOffset(0)]
        public Vector3 start;
        [FieldOffset(16)]
        public Vector3 end;

        public readonly override string ToString() => $"LineSegment({start} -> {end})";
    }


    // Size=16
    public struct Plane
    {
        public Vector3 normal;
        public float dist;

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
    }


    // Size=16
    public struct KeyFrame
    {
        public float value;
        public uint time_type;
        public uint inNormal;
        public uint outNormal;
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
