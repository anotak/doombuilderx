using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Geometry;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Vector3D for lua
    [MoonSharpUserData]
    public struct LuaVector3D
    {
        [MoonSharpHidden]
        internal Vector3D vec;

        public float x
        {
            get
            {
                return vec.x;
            }
        }

        public float y
        {
            get
            {
                return vec.y;
            }
        }

        public float z
        {
            get
            {
                return vec.z;
            }
        }

        public LuaVector3D(float x, float y, float z)
        {
            vec = new Vector3D(x, y, z);
        }

        public LuaVector3D(float x, float y)
        {
            vec = new Vector3D(x, y, 0f);
        }

        [MoonSharpHidden]
        public LuaVector3D(Vector3D v)
        {
            vec = v;
        }

        [MoonSharpHidden]
        public LuaVector3D(Vector2D v)
        {
            vec.x = v.x;
            vec.y = v.y;
            vec.z = 0f;
        }

        public static LuaVector3D From(float a)
        {
            return new LuaVector3D(a, a);
        }

        public static LuaVector3D From(float x, float y)
        {
            return new LuaVector3D(x, y);
        }

        public static LuaVector3D From(float x, float y, float z)
        {
            return new LuaVector3D(x, y, z);
        }

        public static LuaVector3D From(LuaVector2D v, float z)
        {
            return new LuaVector3D(v.vec.x, v.vec.y, z);
        }

        public static LuaVector3D From(LuaVector2D v)
        {
            return new LuaVector3D(v.vec);
        }

        public static LuaVector3D operator +(LuaVector3D a, LuaVector3D b)
        {
            return new LuaVector3D(a.vec + b.vec);
        }

        public static LuaVector3D operator +(LuaVector3D a, LuaVector2D b)
        {
            return new LuaVector3D(a.vec + new Vector3D(b.vec.x, b.vec.y, 0f));
        }

        public static LuaVector3D operator +(LuaVector3D a, float b)
        {
            return new LuaVector3D(a.vec + b);
        }

        public static LuaVector3D operator +(float a, LuaVector3D b)
        {
            return new LuaVector3D(a + b.vec);
        }


        public static LuaVector3D operator -(LuaVector3D a, LuaVector3D b)
        {
            return new LuaVector3D(a.vec - b.vec);
        }

        public static LuaVector3D operator -(LuaVector3D a, LuaVector2D b)
        {
            return new LuaVector3D(a.vec - new Vector3D(b.vec.x, b.vec.y, 0f));
        }

        public static LuaVector3D operator -(LuaVector3D a, float b)
        {
            return new LuaVector3D(a.vec - b);
        }

        public static LuaVector3D operator -(float a, LuaVector3D b)
        {
            return new LuaVector3D(a - b.vec);
        }

        public static LuaVector3D operator -(LuaVector3D a)
        {
            return new LuaVector3D(-a.vec);
        }


        public static LuaVector3D operator *(LuaVector3D a, LuaVector3D b)
        {
            return new LuaVector3D(a.vec * b.vec);
        }

        public static LuaVector3D operator *(LuaVector3D a, LuaVector2D b)
        {
            return new LuaVector3D(a.vec * new Vector3D(b.vec.x, b.vec.y, 1f));
        }

        public static LuaVector3D operator *(LuaVector3D a, float b)
        {
            return new LuaVector3D(a.vec * b);
        }

        public static LuaVector3D operator *(float a, LuaVector3D b)
        {
            return new LuaVector3D(a * b.vec);
        }

        public static LuaVector3D operator /(LuaVector3D a, LuaVector3D b)
        {
            return new LuaVector3D(a.vec / b.vec);
        }

        public static LuaVector3D operator /(LuaVector3D a, LuaVector2D b)
        {
            return new LuaVector3D(a.vec / new Vector3D(b.vec.x, b.vec.y, 1f));
        }

        public static LuaVector3D operator /(LuaVector3D a, float b)
        {
            return new LuaVector3D(a.vec / b);
        }

        public static LuaVector3D operator /(float a, LuaVector3D b)
        {
            return new LuaVector3D(a / b.vec);
        }

        public static float DotProduct(LuaVector3D a, LuaVector3D b)
        {
            return Vector3D.DotProduct(a.vec, b.vec);
        }

        public static LuaVector3D CrossProduct(LuaVector3D a, LuaVector3D b)
        {
            return new LuaVector3D(Vector3D.CrossProduct(a.vec, b.vec));
        }

        public static bool operator ==(LuaVector3D a, LuaVector3D b)
        {
            return a.vec == b.vec;
        }

        public static bool operator !=(LuaVector3D a, LuaVector3D b)
        {
            return a.vec != b.vec;
        }

        public static LuaVector3D Reflect(LuaVector3D v, LuaVector3D m)
        {
            return new LuaVector3D(Vector3D.Reflect(v.vec, m.vec));
        }

        public static LuaVector3D Reversed(LuaVector3D r)
        {
            return new LuaVector3D(Vector3D.Reversed(r.vec));
        }

        public static LuaVector3D FromAngleXY(float angle)
        {
            return new LuaVector3D(Vector3D.FromAngleXY(angle));
        }

        public static LuaVector3D FromAngleXY(float angle, float length)
        {
            return new LuaVector3D(Vector3D.FromAngleXY(angle, length));
        }

        public static LuaVector3D SnappedToGrid(LuaVector3D v)
        {
            Vector2D snapped = General.Map.Grid.SnappedToGrid(v.vec);
            return new LuaVector3D(snapped.x, snapped.y, v.vec.z);
        }

        public static LuaVector3D SnappedToAccuracy(LuaVector3D v)
        {
            return new LuaVector3D(
                    new Vector3D(
                        (float)Math.Round(v.vec.x, General.Map.FormatInterface.VertexDecimals),
                        (float)Math.Round(v.vec.y, General.Map.FormatInterface.VertexDecimals),
                        v.vec.z
                    )
                );
        }

        public LuaVector3D WithX(float nx)
        {
            return new LuaVector3D(new Vector3D(nx, vec.y, vec.z));
        }

        public LuaVector3D WithY(float ny)
        {
            return new LuaVector3D(new Vector3D(vec.x, ny, vec.z));
        }

        public LuaVector3D WithZ(float nz)
        {
            return new LuaVector3D(new Vector3D(vec.x, vec.y, nz));
        }
        
        public float GetAngleXY()
        {
            return vec.GetAngleXY();
        }

        public float GetAngleZ()
        {
            return vec.GetAngleZ();
        }

        public float GetLength()
        {
            return vec.GetLength();
        }

        public float GetLengthSq()
        {
            return vec.GetLengthSq();
        }

        public float GetManhattanLength()
        {
            return vec.GetManhattanLength();
        }

        public LuaVector3D GetNormal()
        {
            return new LuaVector3D(vec.GetNormal());
        }

        public LuaVector3D GetScaled(float s)
        {
            return new LuaVector3D(vec.GetScaled(s));
        }

        public LuaVector3D GetFixedLength(float length)
        {
            return new LuaVector3D(vec.GetFixedLength(length));
        }

        public override string ToString()
        {
            return vec.ToString();
        }

        public bool IsNormalized()
        {
            return vec.IsNormalized();
        }

        public bool IsFinite()
        {
            return vec.IsFinite();
        }

        //mxd. Addeed to make compiler a bit more happy...
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // keeping compiler happy
        public override bool Equals(object obj)
        {
            if (obj is Vector3D)
            {
                return vec == (Vector3D)obj;
            }
            if (obj is LuaVector3D)
            {
                return vec == ((LuaVector3D)obj).vec;
            }
            return false;
        }
    } // struct
} // ns
