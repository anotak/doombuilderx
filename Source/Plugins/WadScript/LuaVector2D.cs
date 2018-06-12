using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Geometry;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Vector2D for lua
    // todo: function to get grid points within a rectangle
    [MoonSharpUserData]
    public struct LuaVector2D
    {
        [MoonSharpHidden]
        internal Vector2D vec;

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

        public LuaVector2D(float x, float y)
        {
            vec = new Vector2D(x, y);
        }

        [MoonSharpHidden]
        public LuaVector2D(Vector2D v)
        {
            vec = v;
        }        

        public static LuaVector2D From(float a)
        {
            return new LuaVector2D(a, a);
        }

        public static LuaVector2D From(float x, float y)
        {
            return new LuaVector2D(x, y);
        }

        public static LuaVector2D operator +(LuaVector2D a, LuaVector2D b)
        {
            return new LuaVector2D(a.vec + b.vec);
        }

        public static LuaVector2D operator +(LuaVector2D a, float b)
        {
            return new LuaVector2D(a.vec + b);
        }
        
        public static LuaVector2D operator +(float a, LuaVector2D b)
        {
            return new LuaVector2D(a + b.vec);
        }


        public static LuaVector2D operator -(LuaVector2D a, LuaVector2D b)
        {
            return new LuaVector2D(a.vec - b.vec);
        }

        public static LuaVector2D operator -(LuaVector2D a, float b)
        {
            return new LuaVector2D(a.vec - b);
        }
        
        public static LuaVector2D operator -(float a, LuaVector2D b)
        {
            return new LuaVector2D(a - b.vec);
        }

        public static LuaVector2D operator -(LuaVector2D a)
        {
            return new LuaVector2D(-a.vec);
        }


        public static LuaVector2D operator *(LuaVector2D a, LuaVector2D b)
        {
            return new LuaVector2D(a.vec * b.vec);
        }

        public static LuaVector2D operator *(LuaVector2D a, float b)
        {
            return new LuaVector2D(a.vec * b);
        }
        
        public static LuaVector2D operator *(float a, LuaVector2D b)
        {
            return new LuaVector2D(a * b.vec);
        }

        public static LuaVector2D operator /(LuaVector2D a, LuaVector2D b)
        {
            return new LuaVector2D(a.vec / b.vec);
        }

        public static LuaVector2D operator /(LuaVector2D a, float b)
        {
            return new LuaVector2D(a.vec / b);
        }

        public static LuaVector2D operator /(float a, LuaVector2D b)
        {
            return new LuaVector2D(a / b.vec);
        }

        public static float DotProduct(LuaVector2D a, LuaVector2D b)
        {
            return Vector2D.DotProduct(a.vec, b.vec);
        }

        public static LuaVector2D CrossProduct(LuaVector2D a, LuaVector2D b)
        {
            return new LuaVector2D(Vector2D.CrossProduct(a.vec, b.vec));
        }

        public static bool operator ==(LuaVector2D a, LuaVector2D b)
        {
            return a.vec == b.vec;
        }

        public static bool operator !=(LuaVector2D a, LuaVector2D b)
        {
            return a.vec != b.vec;
        }

        public static LuaVector2D Reflect(LuaVector2D v, LuaVector2D m)
        {
            return new LuaVector2D(Vector2D.Reflect(v.vec, m.vec));
        }

        public static LuaVector2D Reversed(LuaVector2D r)
        {
            return new LuaVector2D(Vector2D.Reversed(r.vec));
        }

        public static LuaVector2D FromAngle(float angle)
        {
            return new LuaVector2D(Vector2D.FromAngle(angle));
        }

        public static LuaVector2D FromAngle(float angle, float length)
        {
            return new LuaVector2D(Vector2D.FromAngle(angle, length));
        }

        public static LuaVector2D SnappedToGrid(LuaVector2D v)
        {
            return new LuaVector2D(General.Map.Grid.SnappedToGrid(v.vec));
        }

        public static LuaVector2D SnappedToAccuracy(LuaVector2D v)
        {
            return new LuaVector2D(
                    new Vector2D(
                        (float)Math.Round(v.vec.x, General.Map.FormatInterface.VertexDecimals),
                        (float)Math.Round(v.vec.y, General.Map.FormatInterface.VertexDecimals)
                    )
                );
        }

        public static float GetAngle(LuaVector2D a, LuaVector2D b)
        {
            return Vector2D.GetAngle(a.vec, b.vec);
        }

        public static float DistanceSq(LuaVector2D a, LuaVector2D b)
        {
            return Vector2D.DistanceSq(a.vec, b.vec);
        }

        public static float Distance(LuaVector2D a, LuaVector2D b)
        {
            return Vector2D.Distance(a.vec, b.vec);
        }

        public static float ManhattanDistance(LuaVector2D a, LuaVector2D b)
        {
            return Vector2D.ManhattanDistance(a.vec, b.vec);
        }

        public LuaVector2D WithX(float nx)
        {
            return new LuaVector2D(new Vector2D(nx, vec.y));
        }

        public LuaVector2D WithY(float ny)
        {
            return new LuaVector2D(new Vector2D(vec.x, ny));
        }

        public LuaVector2D GetPerpendicular()
        {
            return new LuaVector2D(vec.GetPerpendicular());
        }

        public LuaVector2D GetSign()
        {
            return new LuaVector2D(vec.GetSign());
        }

        public float GetAngle()
        {
            return vec.GetAngle();
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

        public LuaVector2D GetNormal()
        {
            return new LuaVector2D(vec.GetNormal());
        }

        public LuaVector2D GetScaled(float s)
        {
            return new LuaVector2D(vec.GetScaled(s));
        }

        public override string ToString()
        {
            return vec.ToString();
        }

        public LuaVector2D GetTransformed(float offsetx, float offsety, float scalex, float scaley)
        {
            return new LuaVector2D(vec.GetTransformed(offsetx, offsety, scalex, scaley));
        }

        public LuaVector2D GetInvTransformed(float invoffsetx, float invoffsety, float invscalex, float invscaley)
        {
            return new LuaVector2D(vec.GetInvTransformed(invoffsetx,invoffsety,invscalex,invscaley));
        }

        public LuaVector2D GetRotated(float theta)
        {
            return new LuaVector2D(vec.GetRotated(theta));
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
            if(obj == null)
            {
                return false;
            }
            if (obj is Vector2D)
            {
                return vec == (Vector2D)obj;
            }
            if (obj is LuaVector2D)
            {
                return vec == ((LuaVector2D)obj).vec;
            }
            return false;
        }
    } // struct
} // ns
