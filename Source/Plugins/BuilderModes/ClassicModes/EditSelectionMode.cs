	
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using System.Drawing;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Edit Selection Mode",
			  SwitchAction = "editselectionmode",
			  ButtonImage = "Selection3.png",
			  ButtonOrder = 50,
			  ButtonGroup = "002_tools",
			  Volatile = true,
			  UseByDefault = true,
			  Optional = false)]

	public class EditSelectionMode : BaseClassicMode
	{
		#region ================== Enums

		private enum ModifyMode : int
		{
			None,
			Dragging,
			Resizing,
			Rotating
		}

		private enum Grip : int
		{
			None,
			Main,
			SizeN,
			SizeS,
			SizeE,
			SizeW,
			RotateLT,
			RotateRT,
			RotateRB,
			RotateLB
		}

		#endregion

		#region ================== Constants

		private const float GRIP_SIZE = 9.0f;
		private const float ZERO_SIZE_ADDITION = 20.0f;
		private const byte RECTANGLE_ALPHA = 60;
		private const byte EXTENSION_LINE_ALPHA = 150;
		private readonly Cursor[] RESIZE_CURSORS = { Cursors.SizeNS, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNESW };
		
		#endregion

		#region ================== Variables

		// Modes
		private bool modealreadyswitching = false;
		private bool pasting = false;
		private PasteOptions pasteoptions;
		
		// Docker
		private EditSelectionPanel panel;
		private Docker docker;
		
		// Highlighted vertex
		private MapElement highlighted;
		private Vector2D highlightedpos;

		// Selection
		private ICollection<Vertex> selectedvertices;
		private ICollection<Thing> selectedthings;
		private ICollection<Linedef> selectedlines;
		private List<Vector2D> vertexpos;
		private List<Vector2D> thingpos;
		private List<float> thingangle;
		private ICollection<Vertex> unselectedvertices;
		private ICollection<Linedef> unselectedlines;

		// Modification
		private float rotation;
		private Vector2D offset;
		private Vector2D size;
		private Vector2D baseoffset;
		private Vector2D basesize;
		private bool linesflipped;
		
		// Modifying Modes
		private ModifyMode mode;
		private Vector2D dragoffset;
		private Vector2D resizefilter;
		private Vector2D resizevector;
		private Vector2D edgevector;
		private Line2D resizeaxis;
		private int stickcorner;
		private float rotategripangle;
		private bool autopanning;
		
		// Rectangle components
		private Vector2D[] originalcorners; // lefttop, righttop, rightbottom, leftbottom
		private Vector2D[] corners;
		private FlatVertex[] cornerverts;
		private RectangleF[] resizegrips;	// top, right, bottom, left
		private RectangleF[] rotategrips;   // lefttop, righttop, rightbottom, leftbottom
		private Line2D extensionline;

		// Options
		private bool snaptogrid;		// SHIFT to toggle
		private bool snaptonearest;		// CTRL to enable
		
		#endregion

		#region ================== Properties

		public override object HighlightedObject { get { return highlighted; } }
		
		public bool Pasting { get { return pasting; } set { pasting = value; } }
		public PasteOptions PasteOptions { get { return pasteoptions; } set { pasteoptions = value.Copy(); } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EditSelectionMode()
		{
			// Initialize
			mode = ModifyMode.None;
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Dispose base
				base.Dispose();
			}
		}

		#endregion
		
		#region ================== Methods
		
		// The following functions set different properties and update
		
		public void SetAbsPosX(float posx)
		{
			offset.x = posx;
			UpdateAllChanges();
		}
		
		public void SetAbsPosY(float posy)
		{
			offset.y = posy;
			UpdateAllChanges();
		}
		
		public void SetRelPosX(float posx)
		{
			offset.x = posx + baseoffset.x;
			UpdateAllChanges();
		}
		
		public void SetRelPosY(float posy)
		{
			offset.y = posy + baseoffset.y;
			UpdateAllChanges();
		}
		
		public void SetAbsSizeX(float sizex)
		{
			size.x = sizex;
			UpdateAllChanges();
		}
		
		public void SetAbsSizeY(float sizey)
		{
			size.y = sizey;
			UpdateAllChanges();
		}
		
		public void SetRelSizeX(float sizex)
		{
			size.x = basesize.x * (sizex / 100.0f);
			UpdateAllChanges();
		}
		
		public void SetRelSizeY(float sizey)
		{
			size.y = basesize.y * (sizey / 100.0f);
			UpdateAllChanges();
		}

		public void SetAbsRotation(float absrot)
		{
			rotation = absrot;
			UpdateAllChanges();
		}
		
		// This updates all after changes were made
		private void UpdateAllChanges()
		{
			UpdateGeometry();
			UpdateRectangleComponents();
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}

		// This returns the position of the highlighted item
		private Vector2D GetHighlightedPosition()
		{
			if(highlighted is Vertex)
				return (highlighted as Vertex).Position;
			else if(highlighted is Thing)
				return (highlighted as Thing).Position;
			else
				throw new Exception("Highlighted element type is not supported.");
		}
		
		// This highlights a new vertex
		protected void Highlight(MapElement h)
		{
			// Undraw previous highlight
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				if(highlighted is Vertex)
				{
					if(renderer.StartPlotter(false))
					{
						renderer.PlotVertex((highlighted as Vertex), renderer.DetermineVertexColor((highlighted as Vertex)));
						renderer.Finish();
					}
				}
				else
				{
					if(renderer.StartThings(false))
					{
						renderer.RenderThing((highlighted as Thing), renderer.DetermineThingColor((highlighted as Thing)), 1.0f);
						renderer.Finish();
					}
				}
			}
			
			// Set new highlight
			highlighted = h;

			// Render highlighted item
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				if(highlighted is Vertex)
				{
					if(renderer.StartPlotter(false))
					{
						renderer.PlotVertex((highlighted as Vertex), ColorCollection.HIGHLIGHT);
						renderer.Finish();
					}
				}
				else
				{
					if(renderer.StartThings(false))
					{
						renderer.RenderThing((highlighted as Thing), General.Colors.Highlight, 1.0f);
						renderer.Finish();
					}
				}
			}

			// Done
			renderer.Present();
		}
		
		// This updates the selection
		private void Update()
		{
			// Not in any modifying mode?
			if(mode == ModifyMode.None)
			{
				// Check what grip the mouse is over
				// and change cursor accordingly
				Grip mousegrip = CheckMouseGrip();
				switch(mousegrip)
				{
					case Grip.Main:
						
						// Find the nearest vertex within highlight range
						Vertex v = MapSet.NearestVertex(selectedvertices, mousemappos);
						
						// Find the nearest thing within range
						Thing t = MapSet.NearestThing(selectedthings, mousemappos);
						
						// Highlight the one that is closer
						if((v != null) && (t != null))
						{
							if(v.DistanceToSq(mousemappos) < t.DistanceToSq(mousemappos))
							{
								if(v != highlighted) Highlight(v);
							}
							else
							{
								if(t != highlighted) Highlight(t);
							}
						}
						else if(v != null)
						{
							if(v != highlighted) Highlight(v);
						}
						else
						{
							if(t != highlighted) Highlight(t);
						}
						
						General.Interface.SetCursor(Cursors.Hand);
						break;

					case Grip.RotateLB:
					case Grip.RotateLT:
					case Grip.RotateRB:
					case Grip.RotateRT:
						Highlight(null);
						General.Interface.SetCursor(Cursors.Cross);
						break;

					case Grip.SizeE:
					case Grip.SizeS:
					case Grip.SizeW:
					case Grip.SizeN:

						// Pick the best matching cursor depending on rotation and side
						float resizeangle = rotation;
						if((mousegrip == Grip.SizeE) || (mousegrip == Grip.SizeW)) resizeangle += Angle2D.PIHALF;
						resizeangle = Angle2D.Normalized(resizeangle);
						if(resizeangle > Angle2D.PI) resizeangle -= Angle2D.PI;
						resizeangle = Math.Abs(resizeangle + Angle2D.PI / 8.000001f);
						int cursorindex = (int)Math.Floor((resizeangle / Angle2D.PI) * 4.0f) % 4;
						General.Interface.SetCursor(RESIZE_CURSORS[cursorindex]);
						Highlight(null);
						break;

					default:
						Highlight(null);
						General.Interface.SetCursor(Cursors.Default);
						break;
				}
			}
			else
			{
				Vector2D snappedmappos = mousemappos;
				bool dosnaptogrid = snaptogrid;

				// Options
				snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
				snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

				// Change to crosshair cursor so we can clearly see around the mouse cursor
				General.Interface.SetCursor(Cursors.Cross);
				
				// Check what modifying mode we are in
				switch(mode)
				{
					// Dragging
					case ModifyMode.Dragging:

						// Change offset without snapping
						offset = mousemappos - dragoffset;
						
						// Calculate transformed position of highlighted vertex
						Vector2D transformedpos = TransformedPoint(highlightedpos);
						
						// Snap to nearest vertex?
						if(snaptonearest && (highlighted != null))
						{
							float vrange = BuilderPlug.Me.StitchRange / renderer.Scale;

							// Try the nearest vertex
							Vertex nv = MapSet.NearestVertexSquareRange(unselectedvertices, transformedpos, vrange);
							if(nv != null)
							{
								// Change offset to snap to target
								offset += nv.Position - transformedpos;
								dosnaptogrid = false;
							}
							else
							{
								// Find the nearest unselected line within range
								Linedef nl = MapSet.NearestLinedefRange(unselectedlines, transformedpos, BuilderPlug.Me.StitchRange / renderer.Scale);
								if(nl != null)
								{
									// Snap to grid?
									if(dosnaptogrid)
									{
										// Get grid intersection coordinates
										List<Vector2D> coords = nl.GetGridIntersections();
										
										// Find nearest grid intersection
										float found_distance = float.MaxValue;
										Vector2D found_pos = new Vector2D(float.NaN, float.NaN);
										foreach(Vector2D v in coords)
										{
											Vector2D dist = transformedpos - v;
											if(dist.GetLengthSq() < found_distance)
											{
												// Found a better match
												found_distance = dist.GetLengthSq();
												found_pos = v;
												
												// Do not snap to grid anymore
												dosnaptogrid = false;
											}
										}

										// Found something?
										if(!float.IsNaN(found_pos.x))
										{
											// Change offset to snap to target
											offset += found_pos - transformedpos;
										}
									}
									else
									{
										// Change offset to snap onto the line
										offset += nl.NearestOnLine(transformedpos) - transformedpos;
									}
								}
							}
						}

						// Snap to grid?
						if(dosnaptogrid && (highlighted != null))
						{
							// Change offset to align to grid
							offset += General.Map.Grid.SnappedToGrid(transformedpos) - transformedpos;
						}

						// Update
						UpdateGeometry();
						UpdateRectangleComponents();
						General.Interface.RedrawDisplay();
						break;

					// Resizing
					case ModifyMode.Resizing:

						// Snap to nearest vertex?
						if(snaptonearest)
						{
							float vrange = BuilderPlug.Me.StitchRange / renderer.Scale;
							
							// Try the nearest vertex
							Vertex nv = MapSet.NearestVertexSquareRange(unselectedvertices, snappedmappos, vrange);
							if(nv != null)
							{
								snappedmappos = nv.Position;
								dosnaptogrid = false;
							}
						}
						
						// Snap to grid?
						if(dosnaptogrid)
						{
							// Aligned to grid
							snappedmappos = General.Map.Grid.SnappedToGrid(snappedmappos);
						}
						
						// Keep corner position
						Vector2D oldcorner = corners[stickcorner];
						
						// Change size with the scale from the ruler
						float scale = resizeaxis.GetNearestOnLine(snappedmappos);
						size = (basesize * resizefilter) * scale + size * (1.0f - resizefilter);
						
						// Adjust corner position
						Vector2D newcorner = TransformedPoint(originalcorners[stickcorner]);
						offset -= newcorner - oldcorner;
						
						// Show the extension line so that the user knows what it is aligning to
						Vector2D sizefiltered = (size * resizefilter);
						float sizelength = sizefiltered.x + sizefiltered.y;
						Line2D edgeline = new Line2D(resizeaxis.v1 + resizevector * sizelength, resizeaxis.v1 + resizevector * sizelength - edgevector);
						float nearestonedge = edgeline.GetNearestOnLine(snappedmappos);
						if(nearestonedge > 0.5f)
							extensionline = new Line2D(edgeline.v1, snappedmappos);
						else
							extensionline = new Line2D(edgeline.v2, snappedmappos);
						
						// Update
						UpdateGeometry();
						UpdateRectangleComponents();
						General.Interface.RedrawDisplay();
						break;

					// Rotating
					case ModifyMode.Rotating:

						// Get angle from mouse to center
						Vector2D center = offset + size * 0.5f;
						Vector2D delta = snappedmappos - center;
						rotation = delta.GetAngle() - rotategripangle;
						
						// Snap rotation to grip?
						if(dosnaptogrid)
						{
							// We make 8 vectors that the rotation can snap to
							float founddistance = float.MaxValue;
							float foundrotation = rotation;
							for(int i = 0; i < 8; i++)
							{
								// Make the vectors
								float angle = (float)i * Angle2D.PI * 0.25f;
								Vector2D gridvec = Vector2D.FromAngle(angle);
								Vector3D rotvec = Vector2D.FromAngle(rotation);
								
								// Check distance
								float dist = 2.0f - Vector2D.DotProduct(gridvec, rotvec);
								if(dist < founddistance)
								{
									foundrotation = angle;
									founddistance = dist;
								}
							}
							
							// Keep rotation
							rotation = foundrotation;
						}
						
						// Update
						UpdateGeometry();
						UpdateRectangleComponents();
						General.Interface.RedrawDisplay();
						break;
				}
			}
		}
		
		// This checks and returns the grip the mouse pointer is in
		private Grip CheckMouseGrip()
		{
			if(PointInRectF(resizegrips[0], mousemappos))
				return Grip.SizeN;
			else if(PointInRectF(resizegrips[2], mousemappos))
				return Grip.SizeS;
			else if(PointInRectF(resizegrips[1], mousemappos))
				return Grip.SizeE;
			else if(PointInRectF(resizegrips[3], mousemappos))
				return Grip.SizeW;
			else if(PointInRectF(rotategrips[0], mousemappos))
				return Grip.RotateLT;
			else if(PointInRectF(rotategrips[1], mousemappos))
				return Grip.RotateRT;
			else if(PointInRectF(rotategrips[2], mousemappos))
				return Grip.RotateRB;
			else if(PointInRectF(rotategrips[3], mousemappos))
				return Grip.RotateLB;
			else if(Tools.PointInPolygon(corners, mousemappos))
				return Grip.Main;
			else
				return Grip.None;
		}
		
		// This applies the current rotation and resize to a point
		private Vector2D TransformedPoint(Vector2D p)
		{
			// Resize
			p = (p - baseoffset) * (size / basesize) + baseoffset;
			
			// Rotate
			Vector2D center = baseoffset + size * 0.5f;
			Vector2D po = p - center;
			p = po.GetRotated(rotation);
			p += center;
			
			// Translate
			p += offset - baseoffset;
			
			return p;
		}

		// This applies the current rotation and resize to a point
		private Vector2D TransformedPointNoScale(Vector2D p)
		{
			// Rotate
			Vector2D center = baseoffset + size * 0.5f;
			Vector2D po = p - center;
			p = po.GetRotated(rotation);
			p += center;

			// Translate
			p += offset - baseoffset;

			return p;
		}
		
		// This applies the current rotation and resize to a point
		private Vector2D TransformedPointNoRotate(Vector2D p)
		{
			// Resize
			p = (p - baseoffset) * (size / basesize) + baseoffset;
			
			// Translate
			p += offset - baseoffset;
			
			return p;
		}

		// This applies the current rotation and resize to a point
		private Vector2D TransformedPointNoRotateNoScale(Vector2D p)
		{
			// Translate
			p += offset - baseoffset;

			return p;
		}
		
		// This checks if a point is in a rect
		private bool PointInRectF(RectangleF rect, Vector2D point)
		{
			return (point.x >= rect.Left) && (point.x <= rect.Right) && (point.y >= rect.Top) && (point.y <= rect.Bottom);
		}
		
		// This updates the values in the panel
		private void UpdatePanel()
		{
			Vector2D relsize = (size / basesize) * 100.0f;
			if(panel != null)
				panel.ShowCurrentValues(offset, offset - baseoffset, size, relsize, rotation);
		}

		// This moves all things and vertices to match the current transformation
		private unsafe void UpdateGeometry()
		{
			float[] newthingangle = thingangle.ToArray();
			int index;

			// Flip things horizontally
			if(size.x < 0.0f)
			{
				for(index = 0; index < newthingangle.Length; index++)
				{
					// Check quadrant
					if((newthingangle[index] >= 0f) && (newthingangle[index] < Angle2D.PIHALF))
						newthingangle[index] = newthingangle[index] - (newthingangle[index] * 2);
					else if((newthingangle[index] >= Angle2D.PIHALF) && (newthingangle[index] <= Angle2D.PI))
						newthingangle[index] = newthingangle[index] + (Angle2D.PI - newthingangle[index]) * 2;
					else if((newthingangle[index] >= Angle2D.PI) && (newthingangle[index] <= Angle2D.PI + Angle2D.PIHALF))
						newthingangle[index] = newthingangle[index] - (newthingangle[index] - Angle2D.PI) * 2;
					else
						newthingangle[index] = newthingangle[index] + (Angle2D.PI2 - newthingangle[index]) * 2;
				}
			}

			// Flip things vertically
			if(size.y < 0.0f)
			{
				for(index = 0; index < newthingangle.Length; index++)
				{
					// Check quadrant
					if((newthingangle[index] >= 0f) && (newthingangle[index] < Angle2D.PIHALF))
						newthingangle[index] = newthingangle[index] + (Angle2D.PI - newthingangle[index] * 2);
					else if((newthingangle[index] >= Angle2D.PIHALF) && (newthingangle[index] <= Angle2D.PI))
						newthingangle[index] = newthingangle[index] - (newthingangle[index] - Angle2D.PIHALF) * 2;
					else if((newthingangle[index] >= Angle2D.PI) && (newthingangle[index] <= Angle2D.PI + Angle2D.PIHALF))
						newthingangle[index] = newthingangle[index] + (Angle2D.PI - (newthingangle[index] - Angle2D.PI) * 2);
					else
						newthingangle[index] = newthingangle[index] - (newthingangle[index] - (Angle2D.PI + Angle2D.PIHALF)) * 2;
				}
			}

			// We use optimized versions of the TransformedPoint depending on what needs to be done.
			// This is mainly done because 0.0 rotation and 1.0 scale may still give slight inaccuracies.
			bool norotate = Math.Abs(rotation) < 0.0001f;
			bool noscale = Math.Abs(size.x - basesize.x) + Math.Abs(size.y - basesize.y) < 0.0001f;
			if(norotate && noscale)
			{
				index = 0;
				foreach(Vertex v in selectedvertices)
				{
					v.Move(TransformedPointNoRotateNoScale(vertexpos[index++]));
				}
				index = 0;
				foreach(Thing t in selectedthings)
				{
					t.Move(TransformedPointNoRotateNoScale(thingpos[index++]));
				}
			}
			else if(norotate)
			{
				index = 0;
				foreach(Vertex v in selectedvertices)
				{
					v.Move(TransformedPointNoRotate(vertexpos[index++]));
				}
				index = 0;
				foreach(Thing t in selectedthings)
				{
					t.Move(TransformedPointNoRotate(thingpos[index++]));
				}
			}
			else if(noscale)
			{
				index = 0;
				foreach(Vertex v in selectedvertices)
				{
					v.Move(TransformedPointNoScale(vertexpos[index++]));
				}
				index = 0;
				foreach(Thing t in selectedthings)
				{
					newthingangle[index] = Angle2D.Normalized(newthingangle[index] + rotation);
					t.Move(TransformedPointNoScale(thingpos[index++]));
				}
			}
			else
			{
				index = 0;
				foreach(Vertex v in selectedvertices)
				{
					v.Move(TransformedPoint(vertexpos[index++]));
				}
				index = 0;
				foreach(Thing t in selectedthings)
				{
					newthingangle[index] = Angle2D.Normalized(newthingangle[index] + rotation);
					t.Move(TransformedPoint(thingpos[index++]));
				}
			}

			// This checks if the lines should be flipped
			bool shouldbeflipped = (size.x < 0.0f) ^ (size.y < 0.0f);
			if(shouldbeflipped != linesflipped) FlipLinedefs();

			// Apply new thing rotations
			index = 0;
			foreach(Thing t in selectedthings)
			{
				t.Rotate(Angle2D.Normalized(newthingangle[index++]));
			}
			
			UpdatePanel();
			General.Map.Map.Update(true, false);
		}
		
		// This updates the selection rectangle components
		private void UpdateRectangleComponents()
		{
			float gripsize = GRIP_SIZE / renderer.Scale;
			PixelColor rectcolor = General.Colors.Highlight.WithAlpha(RECTANGLE_ALPHA);

			// Original (untransformed) corners
			originalcorners = new Vector2D[4];
			originalcorners[0] = new Vector2D(baseoffset.x, baseoffset.y);
			originalcorners[1] = new Vector2D(baseoffset.x + basesize.x, baseoffset.y);
			originalcorners[2] = new Vector2D(baseoffset.x + basesize.x, baseoffset.y + basesize.y);
			originalcorners[3] = new Vector2D(baseoffset.x, baseoffset.y + basesize.y);

			// Corners
			corners = new Vector2D[4];
			for(int i = 0; i < 4; i++)
				corners[i] = TransformedPoint(originalcorners[i]);

			// Vertices
			cornerverts = new FlatVertex[6];
			for(int i = 0; i < 6; i++)
			{
				cornerverts[i] = new FlatVertex();
				cornerverts[i].z = 1.0f;
				cornerverts[i].c = rectcolor.ToInt();
			}
			cornerverts[0].x = corners[0].x;
			cornerverts[0].y = corners[0].y;
			cornerverts[1].x = corners[1].x;
			cornerverts[1].y = corners[1].y;
			cornerverts[2].x = corners[2].x;
			cornerverts[2].y = corners[2].y;
			cornerverts[3].x = corners[0].x;
			cornerverts[3].y = corners[0].y;
			cornerverts[4].x = corners[2].x;
			cornerverts[4].y = corners[2].y;
			cornerverts[5].x = corners[3].x;
			cornerverts[5].y = corners[3].y;
			
			// Middle points between corners
			Vector2D middle01 = corners[0] + (corners[1] - corners[0]) * 0.5f;
			Vector2D middle12 = corners[1] + (corners[2] - corners[1]) * 0.5f;
			Vector2D middle23 = corners[2] + (corners[3] - corners[2]) * 0.5f;
			Vector2D middle30 = corners[3] + (corners[0] - corners[3]) * 0.5f;
			
			// Resize grips
			resizegrips = new RectangleF[4];
			resizegrips[0] = new RectangleF(middle01.x - gripsize * 0.5f,
											middle01.y - gripsize * 0.5f,
											gripsize, gripsize);
			resizegrips[1] = new RectangleF(middle12.x - gripsize * 0.5f,
											middle12.y - gripsize * 0.5f,
											gripsize, gripsize);
			resizegrips[2] = new RectangleF(middle23.x - gripsize * 0.5f,
											middle23.y - gripsize * 0.5f,
											gripsize, gripsize);
			resizegrips[3] = new RectangleF(middle30.x - gripsize * 0.5f,
											middle30.y - gripsize * 0.5f,
											gripsize, gripsize);

			// Rotate grips
			rotategrips = new RectangleF[4];
			rotategrips[0] = new RectangleF(corners[0].x - gripsize * 0.5f,
											corners[0].y - gripsize * 0.5f,
											gripsize, gripsize);
			rotategrips[1] = new RectangleF(corners[1].x - gripsize * 0.5f,
											corners[1].y - gripsize * 0.5f,
											gripsize, gripsize);
			rotategrips[2] = new RectangleF(corners[2].x - gripsize * 0.5f,
											corners[2].y - gripsize * 0.5f,
											gripsize, gripsize);
			rotategrips[3] = new RectangleF(corners[3].x - gripsize * 0.5f,
											corners[3].y - gripsize * 0.5f,
											gripsize, gripsize);
		}
		
		// This flips all linedefs in the selection (used for mirroring)
		private void FlipLinedefs()
		{
			// Flip linedefs
			foreach(Linedef ld in selectedlines)
				ld.FlipVertices();
			
			// Done
			linesflipped = !linesflipped;
		}
		
		#endregion

		#region ================== Events

		public override void OnHelp()
		{
			General.ShowHelp("e_editselection.html");
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			
			bool autodrag = (pasting && mouseinside && BuilderPlug.Me.AutoDragOnPaste);
			
			// Add toolbar buttons
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.FlipSelectionH);
			General.Interface.AddButton(BuilderPlug.Me.MenusForm.FlipSelectionV);
			
			// Add docker
			panel = new EditSelectionPanel(this);
			docker = new Docker("editselection", "Edit Selection", panel);
			General.Interface.AddDocker(docker);
			General.Interface.SelectDocker(docker);
			
			// We don't want to record this for undoing while we move the geometry around.
			// This will be set back to normal when we're done.
			General.Map.UndoRedo.IgnorePropChanges = true;
			
			// Convert geometry selection
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedVertices(true, true);
			General.Map.Map.MarkSelectedThings(true, true);
			General.Map.Map.MarkSelectedLinedefs(true, true);
			General.Map.Map.MarkSelectedSectors(true, true);
			ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(true);
			foreach(Vertex v in verts) v.Marked = true;
			ICollection<Sector> sects = General.Map.Map.GetSelectedSectors(true);
			foreach(Sector s in sects)
			{
				foreach(Sidedef sd in s.Sidedefs)
				{
					sd.Line.Marked = true;
					sd.Line.Start.Marked = true;
					sd.Line.End.Marked = true;
				}
			}
			selectedvertices = General.Map.Map.GetMarkedVertices(true);
			selectedthings = General.Map.Map.GetMarkedThings(true);
			unselectedvertices = General.Map.Map.GetMarkedVertices(false);
			
			// Make sure everything is selected so that it turns up red
			foreach(Vertex v in selectedvertices) v.Selected = true;
			ICollection<Linedef> markedlines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
			foreach(Linedef l in markedlines) l.Selected = true;
			selectedlines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
			unselectedlines = General.Map.Map.LinedefsFromMarkedVertices(true, false, false);
			
			// Array to keep original coordinates
			vertexpos = new List<Vector2D>(selectedvertices.Count);
			thingpos = new List<Vector2D>(selectedthings.Count);
			thingangle = new List<float>(selectedthings.Count);

			// A selection must be made!
			if((selectedvertices.Count > 0) || (selectedthings.Count > 0))
			{
				// Initialize offset and size
				offset.x = float.MaxValue;
				offset.y = float.MaxValue;
				Vector2D right;
				right.x = float.MinValue;
				right.y = float.MinValue;
				
				foreach(Vertex v in selectedvertices)
				{
					// Find left-top and right-bottom
					if(v.Position.x < offset.x) offset.x = v.Position.x;
					if(v.Position.y < offset.y) offset.y = v.Position.y;
					if(v.Position.x > right.x) right.x = v.Position.x;
					if(v.Position.y > right.y) right.y = v.Position.y;
					
					// Keep original coordinates
					vertexpos.Add(v.Position);
				}

				foreach(Thing t in selectedthings)
				{
					// Find left-top and right-bottom
					if((t.Position.x - t.Size) < offset.x) offset.x = t.Position.x - t.Size;
					if((t.Position.y - t.Size) < offset.y) offset.y = t.Position.y - t.Size;
					if((t.Position.x + t.Size) > right.x) right.x = t.Position.x + t.Size;
					if((t.Position.y + t.Size) > right.y) right.y = t.Position.y + t.Size;

					// Keep original coordinates
					thingpos.Add(t.Position);
					thingangle.Add(t.Angle);
				}
				
				// Calculate size
				size = right - offset;
				
				// If the width of a dimension is zero, add a little
				if(Math.Abs(size.x) < 1.0f)
				{
					size.x += ZERO_SIZE_ADDITION;
					offset.x -= ZERO_SIZE_ADDITION / 2;
				}
				
				if(Math.Abs(size.y) < 1.0f)
				{
					size.y += ZERO_SIZE_ADDITION;
					offset.y -= ZERO_SIZE_ADDITION / 2;
				}
				
				basesize = size;
				baseoffset = offset;
				
				// When pasting, we want to move the geometry so it is visible
				if(pasting)
				{
					// Mouse in screen?
					if(mouseinside)
					{
						offset = mousemappos - size / 2;
					}
					else
					{
						Vector2D viewmappos = new Vector2D(renderer.OffsetX, renderer.OffsetY);
						offset = viewmappos - size / 2;
					}

					UpdateGeometry();
					General.Map.Data.UpdateUsedTextures();

					if(!autodrag)
						General.Map.Map.Update();
				}
				
				// Set presentation
				if(selectedthings.Count > 0)
					renderer.SetPresentation(Presentation.Things);
				else
					renderer.SetPresentation(Presentation.Standard);
				
				// Update
				panel.ShowOriginalValues(baseoffset, basesize);
				UpdateRectangleComponents();
				UpdatePanel();
				Update();
				
				// When pasting and mouse is in screen, drag selection immediately
				if(autodrag) OnSelectBegin();
			}
			else
			{
				General.Interface.MessageBeep(MessageBeepType.Default);
				General.Interface.DisplayStatus(StatusType.Info, "A selection is required for this action.");
				
				// Cancel now
				General.Editing.CancelMode();
			}
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Paste operation?
			if(pasting)
			{
				// Resume normal undo/redo recording
				General.Map.UndoRedo.IgnorePropChanges = false;
				
				// Remove the geometry
				int index = 0;
				foreach(Vertex v in selectedvertices)
					v.Dispose();

				index = 0;
				foreach(Thing t in selectedthings)
					t.Dispose();
				
				// Withdraw the undo
				if(General.Map.UndoRedo.NextUndo != null)
					General.Map.UndoRedo.WithdrawUndo();
			}
			else
			{
				// Reset geometry in original position
				int index = 0;
				foreach(Vertex v in selectedvertices)
					v.Move(vertexpos[index++]);

				index = 0;
				foreach(Thing t in selectedthings)
				{
					t.Rotate(thingangle[index]);
					t.Move(thingpos[index++]);
				}
				
				// Resume normal undo/redo recording
				General.Map.UndoRedo.IgnorePropChanges = false;
			}
			
			General.Map.Map.Update(true, true);
			
			// Return to previous stable mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// When accepted
		public override void OnAccept()
		{
			base.OnAccept();

			// Anything to do?
			if((selectedthings.Count > 0) || (selectedvertices.Count > 0))
			{
				Vector2D tl = new Vector2D(General.Map.Config.RightBoundary, General.Map.Config.BottomBoundary);
				Vector2D br = new Vector2D(General.Map.Config.LeftBoundary, General.Map.Config.RightBoundary);

				foreach (Vertex v in selectedvertices)
				{
					if (v.Position.x < tl.x) tl.x = (int)v.Position.x;
					if (v.Position.x > br.x) br.x = (int)v.Position.x;
					if (v.Position.y > tl.y) tl.y = (int)v.Position.y;
					if (v.Position.y < br.y) br.y = (int)v.Position.y;
				}

				foreach (Thing t in selectedthings)
				{
					if (t.Position.x < tl.x) tl.x = (int)t.Position.x;
					if (t.Position.x > br.x) br.x = (int)t.Position.x;
					if (t.Position.y > tl.y) tl.y = (int)t.Position.y;
					if (t.Position.y < br.y) br.y = (int)t.Position.y;
				}

				// Check if the selection is outside the map boundaries
				if (tl.x < General.Map.Config.LeftBoundary || br.x > General.Map.Config.RightBoundary ||
					tl.y > General.Map.Config.TopBoundary || br.y < General.Map.Config.BottomBoundary)
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Error: selection out of map boundaries.");

					// If we're in the process of switching to another mode, reset to selection
					// to its old position
					if (modealreadyswitching == true)
					{
						// Reset geometry in original position
						int index = 0;
						foreach (Vertex v in selectedvertices)
							v.Move(vertexpos[index++]);

						index = 0;
						foreach (Thing t in selectedthings)
						{
							t.Rotate(thingangle[index]);
							t.Move(thingpos[index++]);
						}

						// Resume normal undo/redo recording
						General.Map.UndoRedo.IgnorePropChanges = false;

						General.Map.Map.Update(true, true);
					}

					return;
				}

				Cursor.Current = Cursors.AppStarting;

				if(!pasting)
				{
					// Reset geometry in original position to create an undo
					if(linesflipped) FlipLinedefs();		// Flip linedefs back if they were flipped
					int index = 0;
					foreach(Vertex v in selectedvertices)
						v.Move(vertexpos[index++]);

					index = 0;
					foreach(Thing t in selectedthings)
					{
						t.Rotate(thingangle[index]);
						t.Move(thingpos[index++]);
					}
					General.Map.Map.Update(true, true);
					
					// Make undo
					General.Map.UndoRedo.CreateUndo("Edit selection");
				}

				// Resume normal undo/redo recording
				General.Map.UndoRedo.IgnorePropChanges = false;

				// Mark selected geometry
				General.Map.Map.ClearAllMarks(false);
				General.Map.Map.MarkAllSelectedGeometry(true, true, true, true, false);
				
				// Move geometry to new position
				UpdateGeometry();
				General.Map.Map.Update(true, true);
				
				// When pasting, we want to join with the parent sector
				// where the sidedefs are referencing a virtual sector
				if(pasting)
				{
					Sector parent = null;
					Sector vsector = null;
					General.Settings.FindDefaultDrawSettings();

					// Go for all sidedes in the new geometry
					List<Sidedef> newsides = General.Map.Map.GetMarkedSidedefs(true);
					for(int i = 0; i < newsides.Count; i++)
					{
						Sidedef s = newsides[i];
						
						// Connected to a virtual sector?
						if(s.Marked && s.Sector.Fields.ContainsKey(MapSet.VirtualSectorField))
						{
							bool joined = false;
							
							// Keep reference to virtual sector
							vsector = s.Sector;
							
							// Not virtual on both sides?
							// Pascal 3-1-08: I can't remember why I have this check here, but it causes problems when
							// pasting a single linedef that refers to the same sector on both sides (the line then
							// loses both its sidedefs because it doesn't join any sector)
							//if((s.Other != null) && !s.Other.Sector.Fields.ContainsKey(MapSet.VirtualSectorField))
							{
								Sidedef joinsidedef = null;
								
								// Find out in which sector this was pasted
								Vector2D testpoint = s.Line.GetSidePoint(!s.IsFront);
								Linedef nl = MapSet.NearestLinedef(General.Map.Map.GetMarkedLinedefs(false), testpoint);
								if(nl != null)
								{
									if(nl.SideOfLine(testpoint) <= 0)
										joinsidedef = nl.Front;
									else
										joinsidedef = nl.Back;

									// Join?
									if(joinsidedef != null)
									{
										// Join!
										s.SetSector(joinsidedef.Sector);
										s.Marked = false;
										joined = true;

										// If we have no parent sector yet, then this is it!
										if(parent == null) parent = joinsidedef.Sector;
									}
								}
							}
							
							// Not joined any sector?
							if(!joined)
							{
								Linedef l = s.Line;

								// Remove the sidedef
								s.Dispose();

								// Correct the linedef
								if((l.Front == null) && (l.Back != null))
								{
									l.FlipVertices();
									l.FlipSidedefs();
								}

								// Correct the sided flags
								l.ApplySidedFlags();
							}
						}
					}
					
					// Do we have a virtual and parent sector?
					if((vsector != null) && (parent != null))
					{
						// Adjust the floor and ceiling heights of all new sectors
						if(pasteoptions.AdjustHeights)
						{
							ICollection<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
							foreach(Sector s in newsectors)
							{
								s.CeilHeight += parent.CeilHeight - vsector.CeilHeight;
								s.FloorHeight += parent.FloorHeight - vsector.FloorHeight;
							}
						}
					}
					
					// Remove any virtual sectors
					General.Map.Map.RemoveVirtualSectors();
				}
				
				// Stitch geometry
				if(snaptonearest) General.Map.Map.StitchGeometry();

				// Make corrections for backward linedefs
				MapSet.FlipBackwardLinedefs(General.Map.Map.Linedefs);
				
				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();
				
				// Update cached values
				General.Map.Data.UpdateUsedTextures();
				General.Map.Map.Update();
				General.Map.ThingsFilter.Update();
				
				// Make normal selection
				General.Map.Map.ClearAllSelected();
				foreach(Vertex v in selectedvertices) if(!v.IsDisposed) v.Selected = true;
				foreach(Linedef l in selectedlines) { if(!l.IsDisposed) { l.Start.Selected = true; l.End.Selected = true; } }
				foreach(Thing t in selectedthings) if(!t.IsDisposed) t.Selected = true;
				General.Map.Map.SelectionType = SelectionType.Vertices | SelectionType.Things;
				
				// Done
				selectedvertices = new List<Vertex>();
				selectedthings = new List<Thing>();
				selectedlines = new List<Linedef>();
				Cursor.Current = Cursors.Default;
				General.Map.IsChanged = true;
			}
			
			if(!modealreadyswitching)
			{
				// Return to previous stable mode
				General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
			}
		}
		
		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Remove toolbar buttons
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.FlipSelectionH);
			General.Interface.RemoveButton(BuilderPlug.Me.MenusForm.FlipSelectionV);
			
			// Remove docker
			General.Interface.RemoveDocker(docker);
			panel.Dispose();
			panel = null;
			
			// When not cancelled manually, we assume it is accepted
			if(!cancelled)
			{
				modealreadyswitching = true;
				//this.OnAccept();	// BAD! Any other plugins won't know this mode was accepted
				General.Editing.AcceptMode();
			}

			// Update
			General.Map.ThingsFilter.Update();
			General.Interface.RedrawDisplay();
			
			// Hide highlight info
			General.Interface.HideInfo();
			General.Interface.SetCursor(Cursors.Default);
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			UpdateRectangleComponents();

			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if(highlighted is Vertex) renderer.PlotVertex((highlighted as Vertex), ColorCollection.HIGHLIGHT);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				if(highlighted is Thing) renderer.RenderThing((highlighted as Thing), General.Colors.Highlight, 1.0f);
				renderer.Finish();
			}

			// Render selection
			if(renderer.StartOverlay(true))
			{
				// Rectangle
				PixelColor rectcolor = General.Colors.Highlight.WithAlpha(RECTANGLE_ALPHA);
				renderer.RenderGeometry(cornerverts, null, true);
				renderer.RenderLine(corners[0], corners[1], 4, rectcolor, true);
				renderer.RenderLine(corners[1], corners[2], 4, rectcolor, true);
				renderer.RenderLine(corners[2], corners[3], 4, rectcolor, true);
				renderer.RenderLine(corners[3], corners[0], 4, rectcolor, true);
				
				// Extension line
				if(extensionline.GetLengthSq() > 0.0f)
					renderer.RenderLine(extensionline.v1, extensionline.v2, 1, General.Colors.Indication.WithAlpha(EXTENSION_LINE_ALPHA), true);
				
				// Grips
				for(int i = 0; i < 4; i++)
				{
					renderer.RenderRectangleFilled(resizegrips[i], General.Colors.Background, true);
					renderer.RenderRectangle(resizegrips[i], 2, General.Colors.Highlight, true);
					renderer.RenderRectangleFilled(rotategrips[i], General.Colors.Background, true);
					renderer.RenderRectangle(rotategrips[i], 2, General.Colors.Indication, true);
				}
				
				renderer.Finish();
			}

			renderer.Present();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			Update();
		}

		// Mouse leaves the display
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			
			// Reset cursor
			General.Interface.SetCursor(Cursors.Default);
		}

		// When edit button is pressed
		protected override void OnEditBegin()
		{
			base.OnEditBegin();
			OnSelectBegin();
		}

		// When edit button is released
		protected override void OnEditEnd()
		{
			base.OnEditEnd();
			OnSelectEnd();
		}

		// When select button is pressed
		protected override void OnSelectBegin()
		{
			base.OnSelectBegin();

			if(mode != ModifyMode.None) return;

			// Used in many cases:
			Vector2D center = offset + size * 0.5f;
			Vector2D delta;

			// Check what grip the mouse is over
			switch(CheckMouseGrip())
			{
				// Drag main rectangle
				case Grip.Main:
					
					// Find the original position of the highlighted element
					if(highlighted is Vertex)
					{
						int index = 0;
						foreach(Vertex v in selectedvertices)
						{
							if(v == highlighted) highlightedpos = vertexpos[index];
							index++;
						}
					}
					else if(highlighted is Thing)
					{
						int index = 0;
						foreach(Thing t in selectedthings)
						{
							if(t == highlighted) highlightedpos = thingpos[index];
							index++;
						}
					}
					
					dragoffset = mousemappos - offset;
					mode = ModifyMode.Dragging;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Resize
				case Grip.SizeN:

					// The resize vector is a unit vector in the direction of the resize.
					// We multiply this with the sign of the current size, because the
					// corners may be reversed when the selection is flipped.
					resizevector = corners[1] - corners[2];
					resizevector = resizevector.GetNormal() * Math.Sign(size.y);
					
					// The edgevector is a vector with length and direction of the edge perpendicular to the resizevector
					edgevector = corners[1] - corners[0];
					
					// Make the resize axis. This is a line with the length and direction
					// of basesize used to calculate the resize percentage.
					resizeaxis = new Line2D(corners[2], corners[2] + resizevector * basesize.y);

					// Original axis filter
					resizefilter = new Vector2D(0.0f, 1.0f);

					// This is the corner that must stay in the same position
					stickcorner = 2;

					Highlight(null);
					mode = ModifyMode.Resizing;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Resize
				case Grip.SizeE:
					// See description above
					resizevector = corners[1] - corners[0];
					resizevector = resizevector.GetNormal() * Math.Sign(size.x);
					edgevector = corners[1] - corners[2];
					resizeaxis = new Line2D(corners[0], corners[0] + resizevector * basesize.x);
					resizefilter = new Vector2D(1.0f, 0.0f);
					stickcorner = 0;
					Highlight(null);
					mode = ModifyMode.Resizing;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Resize
				case Grip.SizeS:
					// See description above
					resizevector = corners[2] - corners[1];
					resizevector = resizevector.GetNormal() * Math.Sign(size.y);
					edgevector = corners[2] - corners[3];
					resizeaxis = new Line2D(corners[1], corners[1] + resizevector * basesize.y);
					resizefilter = new Vector2D(0.0f, 1.0f);
					stickcorner = 0;
					Highlight(null);
					mode = ModifyMode.Resizing;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Resize
				case Grip.SizeW:
					// See description above
					resizevector = corners[0] - corners[1];
					resizevector = resizevector.GetNormal() * Math.Sign(size.x);
					edgevector = corners[0] - corners[3];
					resizeaxis = new Line2D(corners[1], corners[1] + resizevector * basesize.x);
					resizefilter = new Vector2D(1.0f, 0.0f);
					stickcorner = 1;
					Highlight(null);
					mode = ModifyMode.Resizing;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Rotate
				case Grip.RotateLB:
					delta = corners[3] - center;
					rotategripangle = delta.GetAngle() - rotation;
					Highlight(null);
					mode = ModifyMode.Rotating;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Rotate
				case Grip.RotateLT:
					delta = corners[0] - center;
					rotategripangle = delta.GetAngle() - rotation;
					Highlight(null);
					mode = ModifyMode.Rotating;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Rotate
				case Grip.RotateRB:
					delta = corners[2] - center;
					rotategripangle = delta.GetAngle() - rotation;
					Highlight(null);
					mode = ModifyMode.Rotating;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Rotate
				case Grip.RotateRT:
					delta = corners[1] - center;
					rotategripangle = delta.GetAngle() - rotation;
					Highlight(null);
					mode = ModifyMode.Rotating;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Outside the selection?
				default:
					// Accept and be done with it
					General.Editing.AcceptMode();
					break;
			}
		}

		// When selected button is released
		protected override void OnSelectEnd()
		{
			base.OnSelectEnd();
			
			// Remove extension line
			extensionline = new Line2D();

			if(autopanning)
			{
				DisableAutoPanning();
				autopanning = false;
			}
			
			// No modifying mode
			mode = ModifyMode.None;
			
			// Redraw
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}

		// When a key is released
		public override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		// When a key is pressed
		public override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if((snaptogrid != (General.Interface.ShiftState ^ General.Interface.SnapToGrid)) ||
			   (snaptonearest != (General.Interface.CtrlState ^ General.Interface.AutoMerge))) Update();
		}

		
		
		#endregion

		#region ================== Actions

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Accept changes
			General.Editing.AcceptMode();
			General.Map.Map.ClearAllSelected();
			General.Interface.RedrawDisplay();
		}

		// Flip vertically
		[BeginAction("flipselectionv")]
		public void FlipVertically()
		{
			// Flip the selection
			offset.y += size.y;
			size.y = -size.y;
			
			// Update
			UpdateGeometry();
			UpdateRectangleComponents();
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}

		// Flip horizontally
		[BeginAction("flipselectionh")]
		public void FlipHorizontally()
		{
			// Flip the selection
			offset.x += size.x;
			size.x = -size.x;

			// Update
			UpdateGeometry();
			UpdateRectangleComponents();
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
