using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeImp.DoomBuilder.Map;

using MoonSharp.Interpreter;

namespace CodeImp.DoomBuilder.DBXLua
{
    // wrapper over Sector for lua
    [MoonSharpUserData]
    public class LuaSector
    {
        [MoonSharpHidden]
        internal Sector sector;

        public bool selected
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("sector has been disposed, can't get selected status.");
                }

                return sector.Selected;
            }

            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("sector has been disposed, can't set selected status.");
                }

                if (value == sector.Selected)
                {
                    return;
                }
                if (value == true)
                {
                    sector.Selected = true;

                    // "simple" in case of truth
                    foreach (Sidedef side in sector.Sidedefs)
                    {
                        side.Line.Selected = true;
                        side.Line.Start.Selected = true;
                        side.Line.End.Selected = true;
                    }
                }
                else
                {
                    // false case less trivial, lines must remain selected if
                    // their other side is selected, and vertices must
                    // remain selected if they are connected to outside selected lines
                    sector.Selected = false;

                    HashSet<Vertex> deselect_vertices = new HashSet<Vertex>();
                    foreach (Sidedef side in sector.Sidedefs)
                    {
                        // skip disposed ones, of course
                        if (side.IsDisposed)
                        {
                            continue;
                        }
                        Linedef l = side.Line;

                        if (l.Back == null || l.Back.IsDisposed)
                        {
                            l.Selected = false;
                        }
                        else if (l.Front == null || l.Front.IsDisposed)
                        {
                            // this case probably shouldn't be possible but
                            // i want lua to be more careful about doing things
                            // that can cause crashes
                            l.Selected = false;
                        }
                        else
                        {
                            // 2sided
                            l.Selected = l.Front.Sector.Selected && l.Back.Sector.Selected;

                            deselect_vertices.Add(l.Start);
                            deselect_vertices.Add(l.End);
                        }
                    }

                    // we have to handle vertices in a second loop once the lines
                    // have already all been deselected if needed
                    foreach (Vertex v in deselect_vertices)
                    {
                        v.Selected = false;
                        foreach (Linedef l in v.Linedefs)
                        {
                            if (l.Selected)
                            {
                                v.Selected = true;
                                break;
                            }
                        }
                        // done w this vertex
                    }
                } // done with true case
                
                
            } // done with selected setter
        }

        public bool marked
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("sector has been disposed, can't get marked status.");
                }

                return sector.Marked;
            }

            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("sector has been disposed, can't set marked status.");
                }

                sector.Marked = value;
            }
        }

        public int floorheight
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get floorheight.");
                }
                return sector.FloorHeight;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set floorheight.");
                }
                sector.FloorHeight = value;
            }
        }

        public int ceilheight
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get ceilheight.");
                }
                return sector.CeilHeight;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set ceilheight.");
                }
                sector.CeilHeight = value;
            }
        }

        public string floortex
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get floortex.");
                }
                return sector.FloorTexture;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set floortex.");
                }
                sector.SetFloorTexture(value);
            }
        }

        public string ceiltex
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get ceiltex.");
                }
                return sector.CeilTexture;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set ceiltex.");
                }
                sector.SetCeilTexture(value);
            }
        }
        public int effect
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get effect.");
                }
                return sector.Effect;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set effect.");
                }
                sector.Effect = value;
            }
        }
        public int tag
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get tag.");
                }
                return sector.Tag;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set tag.");
                }
                sector.Tag = value;
            }
        }
        public int brightness
        {
            get
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't get brightness.");
                }
                return sector.Brightness;
            }
            set
            {
                if (sector.IsDisposed)
                {
                    throw new ScriptRuntimeException("Sector has been disposed, can't set brightness.");
                }
                sector.Brightness = value;
            }
        }

        [MoonSharpHidden]
        public LuaSector(Sector s)
        {
            sector = s;
        }

        public bool IsDisposed()
        {
            return sector.IsDisposed;
        }

        public int GetIndex()
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetIndex().");
            }

            return sector.Index;
        }

        public DynValue GetUDMFField(string key)
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetUDMFField().");
            }
            return LuaTypeConversion.GetUDMFField(sector, key, General.Map.Config.SectorFields);
        }

        public void SetUDMFField(string key, DynValue value)
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't SetUDMFField().");
            }
            // ano - bc of like floor scaling or whatever
            sector.UpdateNeeded = true;
            LuaTypeConversion.SetUDMFField(sector, key, value);
        }

        public Table GetUDMFTable()
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetUDMFTable().");
            }

            return LuaTypeConversion.GetUDMFTable(sector);
        }

        public Table GetSidedefs()
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetSidedefs().");
            }

            Table output = new Table(ScriptContext.context.script);
            foreach (Sidedef s in sector.Sidedefs)
            {
                if (!s.IsDisposed)
                {
                    output.Append(DynValue.FromObject(ScriptContext.context.script, new LuaSidedef(s)));
                }
            }

            return output;
        }

        public Table GetLinedefs()
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't GetLinedefs().");
            }

            HashSet<Linedef> linedefs = new HashSet<Linedef>();

            Table output = new Table(ScriptContext.context.script);
            foreach (Sidedef s in sector.Sidedefs)
            {
                if (!s.IsDisposed && !s.Line.IsDisposed)
                {
                    if (linedefs.Add(s.Line))
                    {
                        output.Append(DynValue.FromObject(ScriptContext.context.script, new LuaLinedef(s.Line)));
                    }
                }
            }

            return output;
        }

        public bool Intersect(LuaVector2D p)
        {
            if (sector.IsDisposed)
            {
                throw new ScriptRuntimeException("Sector has been disposed, can't Intersect().");
            }
            return sector.Intersect(p.vec);
        }

        public override string ToString()
        {
            if (sector.IsDisposed)
            {
                return "Disposed " + sector.ToString();
            }
            return sector.ToString();
        }

        public LuaVector2D GetCenter()
        {
            float xmin = General.Map.Config.RightBoundary;
            float xmax = General.Map.Config.LeftBoundary;
            float ymin = General.Map.Config.TopBoundary;
            float ymax = General.Map.Config.BottomBoundary;

            foreach (Sidedef s in sector.Sidedefs)
            {
                if (!s.IsDisposed
                    && !s.Line.IsDisposed
                    && !s.Line.Start.IsDisposed
                    && !s.Line.End.IsDisposed)
                {
                    xmin = Math.Min(xmin, s.Line.Start.Position.x);
                    xmin = Math.Min(xmin, s.Line.End.Position.x);

                    xmax = Math.Max(xmax, s.Line.Start.Position.x);
                    xmax = Math.Max(xmax, s.Line.End.Position.x);

                    ymin = Math.Min(ymin, s.Line.Start.Position.y);
                    ymin = Math.Min(ymin, s.Line.End.Position.y);

                    ymax = Math.Max(ymax, s.Line.Start.Position.y);
                    ymax = Math.Max(ymax, s.Line.End.Position.y);
                }
            }

            return new LuaVector2D((xmax + xmin) / 2f, (ymax + ymin) / 2f);
        }
        
        // TODO  join, createbbox

    } // sector
} // ns
