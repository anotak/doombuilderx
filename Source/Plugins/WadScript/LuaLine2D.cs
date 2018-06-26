using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeImp.DoomBuilder.Geometry;
using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Line2D for lua
    [MoonSharpUserData]
    public struct LuaLine2D
    {
        [MoonSharpHidden]
        public Line2D l2d;

        [MoonSharpHidden]
        public LuaLine2D(Vector2D v1, Vector2D v2)
        {
            l2d = new Line2D(v1, v2);
        }

        [MoonSharpHidden]
        public LuaLine2D(Line2D input)
        {
            l2d = input;
        }

        #region STATIC
        public static LuaLine2D From(float x1, float y1, float x2, float y2)
        {
            return new LuaLine2D(new Line2D(x1, y1, x2, y2));
        }

        public static LuaLine2D From(LuaVector2D v)
        {
            return new LuaLine2D(new Line2D(0f,0f, v.vec));
        }

        public static LuaLine2D From(LuaVector2D v, float x2, float y2)
        {
            return new LuaLine2D(new Line2D(v.vec, x2, y2));
        }

        public static LuaLine2D From(float x1, float y1, LuaVector2D v)
        {
            return new LuaLine2D(new Line2D(x1, y1, v.vec));
        }

        public static LuaLine2D From(LuaVector2D v1, LuaVector2D v2)
        {
            return new LuaLine2D(new Line2D(v1.vec,v2.vec));
        }

        public static LuaLine2D From(LuaLinedef linedef)
        {
            if (linedef.linedef.IsDisposed)
            {
                throw new Exception("Linedef is disposed, can't Line2D.From()!");
            }
            return new LuaLine2D(linedef.linedef.Line);
        }

        public static LuaLine2D From(LuaSidedef sidedef)
        {
            if (sidedef.sidedef.IsDisposed)
            {
                throw new Exception("Sidedef is disposed, can't Line2D.From()!");
            }
            if (sidedef.sidedef.Line.IsDisposed)
            {
                throw new Exception("Linedef is disposed, can't Line2D.From()!");
            }
            return new LuaLine2D(sidedef.sidedef.Line.Line);
        }
        #endregion

        public LuaVector2D GetV1()
        {
            return new LuaVector2D(l2d.v1);
        }

        public LuaVector2D GetV2()
        {
            return new LuaVector2D(l2d.v2);
        }

        public void SetV1(LuaVector2D v)
        {
            l2d.v1 = v.vec;
        }

        public void SetV2(LuaVector2D v)
        {
            l2d.v2 = v.vec;
        }

        public float GetX1()
        {
            return l2d.v1.x;
        }

        public float GetX2()
        {
            return l2d.v2.x;
        }

        public float GetY1()
        {
            return l2d.v1.y;
        }

        public float GetY2()
        {
            return l2d.v2.y;
        }

        public LuaLine2D WithX1(float f)
        {
            return new LuaLine2D(new Line2D(
                f,
                l2d.v1.y,
                l2d.v2.x,
                l2d.v2.y));
        }

        public LuaLine2D WithX2(float f)
        {
            return new LuaLine2D(new Line2D(
                l2d.v1.x,
                l2d.v1.y,
                f,
                l2d.v2.y));
        }

        public LuaLine2D WithY1(float f)
        {
            return new LuaLine2D(new Line2D(
                l2d.v1.x,
                f,
                l2d.v2.x,
                l2d.v2.y));
        }

        public LuaLine2D WithY2(float f)
        {
            return new LuaLine2D(new Line2D(
                l2d.v1.x,
                l2d.v1.y,
                l2d.v2.x,
                f));
        }
        
        public LuaLine2D WithV1(float x, float y)
        {
            return new LuaLine2D(new Line2D(
                x,
                y,
                l2d.v2.x,
                l2d.v2.y));
        }

        public LuaLine2D WithV1(LuaVector2D v1)
        {
            return new LuaLine2D(new Line2D(
                v1.vec,
                l2d.v2.x,
                l2d.v2.y
                ));
        }
        
        public LuaLine2D WithV2(float x, float y)
        {
            return new LuaLine2D(new Line2D(
                l2d.v1.x,
                l2d.v1.y,
                x,
                y));
        }

        public LuaLine2D WithV2(LuaVector2D v2)
        {
            return new LuaLine2D(new Line2D(
                l2d.v1.x,
                l2d.v1.y,
                v2.vec
                ));
        }

        public static LuaVector2D GetIntersectionPoint(LuaLine2D a, LuaLine2D b)
        {
            float u_ray = 0f;
            float u_line = 0f;

            bool intersected = a.l2d.GetIntersection(b.l2d, out u_ray, out u_line);

            Vector2D output = a.l2d.GetCoordinatesAt(u_line);
            
            return new LuaVector2D(output);
        }

        public LuaVector2D GetPerpendicular()
        {
            return new LuaVector2D(l2d.GetPerpendicular());
        }

        public float GetAngle()
        {
            return l2d.GetAngle();
        }

        public LuaVector2D GetDelta()
        {
            return new LuaVector2D(l2d.GetDelta());
        }

        public float GetLength()
        {
            return l2d.GetLength();
        }

        public float GetLengthSq()
        {
            return l2d.GetLengthSq();
        }

        public float GetSlope()
        {
            return (l2d.v2.y - l2d.v1.y) / (l2d.v2.x - l2d.v1.x);
        }

        public override string ToString()
        {
            return l2d.ToString();
        }

        // returns true/false, u_line, u_ray
        public DynValue GetIntersection(float x3, float y3, float x4, float y4)
        {
            DynValue[] output = new DynValue[3];
            float u_line = 0.0f;
            float u_ray = 0.0f;

            bool intersecting = l2d.GetIntersection(x3, y3, x4, y4, out u_ray, out u_line);

            if (float.IsInfinity(u_ray) || float.IsNaN(u_ray))
            {
                u_ray = -1f;
            }
            if (float.IsInfinity(u_line) || float.IsNaN(u_line))
            {
                u_line = -1f;
            }

            output[0] = DynValue.NewBoolean(intersecting);
            output[1] = DynValue.NewNumber(u_ray);
            output[2] = DynValue.NewNumber(u_line);

            return DynValue.NewTuple(output);
        }

        public DynValue GetIntersection(LuaLine2D ray)
        {
            return GetIntersection(ray.l2d.v1.x, ray.l2d.v1.y, ray.l2d.v2.x, ray.l2d.v2.y);
        }

        public DynValue GetIntersection(LuaVector2D v1, LuaVector2D v2)
        {
            return GetIntersection(v1.x, v1.y, v2.x, v2.y);
        }

        public float GetSideOfLine(LuaVector2D p)
        {
            return l2d.GetSideOfLine(p.vec);
        }

        public float GetDistanceToLine(LuaVector2D p, bool bounded)
        {
            return l2d.GetDistanceToLine(p.vec, bounded);
        }

        public float GetDistanceToLineSq(LuaVector2D p, bool bounded)
        {
            return l2d.GetDistanceToLineSq(p.vec, bounded);
        }

        public float GetNearestOnLine(LuaVector2D p)
        {
            return l2d.GetNearestOnLine(p.vec);
        }

        public LuaVector2D GetCoordinatesAt(float u)
        {
            Vector2D output = new Vector2D(l2d.GetCoordinatesAt(u));
            if (output.IsFinite())
            {
                return new LuaVector2D(output);
            }
            else
            {
                throw new ScriptRuntimeException("GetCoordinatesAt() called with bad u value of "
                    + u + 
                    ", (maybe you had a division by 0 somewhere?)");
            }
        }

        public LuaLine2D GetFlipped()
        {
            return new LuaLine2D(l2d.v2, l2d.v1);
        }
    } // struct
} // ns
