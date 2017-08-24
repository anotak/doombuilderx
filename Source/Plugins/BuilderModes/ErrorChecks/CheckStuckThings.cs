
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
using System.Threading;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check stuck things", true, 1000)]
	public class CheckStuckThings : ErrorChecker
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 10;
		private const float ALLOWED_STUCK_DISTANCE = 6.0f;

		enum StuckType
		{
			None = 0,
			Line,
			Thing
		}

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckStuckThings()
		{
			// Total progress is done when all things are checked
			SetTotalProgress(General.Map.Map.Things.Count / PROGRESS_STEP);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This runs the check
		public override void Run()
		{
			BlockMap<BlockEntry> blockmap = BuilderPlug.Me.ErrorCheckForm.BlockMap;
			int progress = 0;
			int stepprogress = 0;
			float maxradius = 0;

			foreach (ThingTypeInfo tti in General.Map.Data.ThingTypes)
			{
				if (tti.Radius > maxradius) maxradius = tti.Radius;
			}

			// Go for all the things
			foreach(Thing t in General.Map.Map.Things)
			{
				ThingTypeInfo info = General.Map.Data.GetThingInfo(t.Type);
				bool stuck = false;
				StuckType stucktype = StuckType.None;

				// Check this thing for getting stuck?
				if( (info.ErrorCheck == ThingTypeInfo.THING_ERROR_INSIDE_STUCK) &&
					(info.Blocking > ThingTypeInfo.THING_BLOCKING_NONE))
				{
					// Make square coordinates from thing
					float blockingsize = t.Size - ALLOWED_STUCK_DISTANCE;
					Vector2D lt = new Vector2D(t.Position.x - blockingsize, t.Position.y - blockingsize);
					Vector2D rb = new Vector2D(t.Position.x + blockingsize, t.Position.y + blockingsize);
					Vector2D bmlt = new Vector2D(t.Position.x - maxradius, t.Position.y - maxradius);
					Vector2D bmrb = new Vector2D(t.Position.x + maxradius, t.Position.y + maxradius);

					// Go for all the lines to see if this thing is stuck
					List<BlockEntry> blocks = blockmap.GetSquareRange(new RectangleF(bmlt.x, bmlt.y, (bmrb.x - bmlt.x), (bmrb.y - bmlt.y)));
					Dictionary<Linedef, Linedef> doneblocklines = new Dictionary<Linedef, Linedef>(blocks.Count * 3);

					foreach(BlockEntry b in blocks)
					{
						foreach(Linedef l in b.Lines)
						{
							// Only test when sinlge-sided, two-sided + impassable and not already checked
							if(((l.Back == null) || l.IsFlagSet(General.Map.Config.ImpassableFlag)) && !doneblocklines.ContainsKey(l))
							{
								// Test if line ends are inside the thing
								if(PointInRect(lt, rb, l.Start.Position) ||
								   PointInRect(lt, rb, l.End.Position))
								{
									// Thing stuck in line!
									stuck = true;
								}
								// Test if the line intersects the square
								else if(Line2D.GetIntersection(l.Start.Position, l.End.Position, lt.x, lt.y, rb.x, lt.y) ||
										Line2D.GetIntersection(l.Start.Position, l.End.Position, rb.x, lt.y, rb.x, rb.y) ||
										Line2D.GetIntersection(l.Start.Position, l.End.Position, rb.x, rb.y, lt.x, rb.y) ||
										Line2D.GetIntersection(l.Start.Position, l.End.Position, lt.x, rb.y, lt.x, lt.y))
								{
									// Thing stuck in line!
									stuck = true;
									stucktype = StuckType.Line;
								}
								
								// Checked
								doneblocklines.Add(l, l);
							}
						}

						// Check if thing is stuck in other things
						if(info.Blocking != ThingTypeInfo.THING_BLOCKING_NONE) {
							foreach (Thing ot in b.Things)
							{
								// Don't compare the thing with itself
								if (t.Index == ot.Index) continue;

								// Only check of items that can block
								if (General.Map.Data.GetThingInfo(ot.Type).Blocking == ThingTypeInfo.THING_BLOCKING_NONE) continue;

								// need to compare the flags
								/* TODO: skill settings
								Dictionary<string, bool> flags1 = t.GetFlags();
								Dictionary<string, bool> flags2 = ot.GetFlags();
								*/

								if (FlagsOverlap(t, ot) && ThingsOverlap(t, ot))
								{
									stuck = true;
									stucktype = StuckType.Thing;
								}
							}
						}
					}
				}
				
				// Stuck?
				if(stuck)
				{
					// Make result
					switch (stucktype)
					{
						case StuckType.Line:
							SubmitResult(new ResultStuckThingInLine(t));
							break;

						case StuckType.Thing:
							SubmitResult(new ResultStuckThingInThing(t));
							break;
					}
				}
				else
				{
					// Check this thing for being outside the map?
					if(info.ErrorCheck >= ThingTypeInfo.THING_ERROR_INSIDE)
					{
						// Get the nearest line to see if the thing is outside the map
						bool outside = false;
						Linedef l = General.Map.Map.NearestLinedef(t.Position);
						if(l.SideOfLine(t.Position) <= 0)
						{
							outside = (l.Front == null);
						}
						else
						{
							outside = (l.Back == null);
						}

						// Outside the map?
						if(outside)
						{
							// Make result
							SubmitResult(new ResultThingOutside(t));
						}
					}
				}
				
				// Handle thread interruption
				try { Thread.Sleep(0); }
				catch(ThreadInterruptedException) { return; }

				// We are making progress!
				if((++progress / PROGRESS_STEP) > stepprogress)
				{
					stepprogress = (progress / PROGRESS_STEP);
					AddProgress(1);
				}
			}
		}
		
		// Point in rect?
		private bool PointInRect(Vector2D lt, Vector2D rb, Vector2D p)
		{
			return (p.x >= lt.x) && (p.x <= rb.x) && (p.y <= lt.y) && (p.y >= rb.y);
		}

		// Checks if two things overlap
		private bool ThingsOverlap(Thing t1, Thing t2)
		{
			Vector3D p1 = t1.Position;
			Vector3D p2 = t2.Position;
			ThingTypeInfo t1info = General.Map.Data.GetThingInfo(t1.Type);
			ThingTypeInfo t2info = General.Map.Data.GetThingInfo(t2.Type);

			// simple bounding box collision detection
			if (	p1.x + t1.Size - ALLOWED_STUCK_DISTANCE < p2.x - t2.Size + ALLOWED_STUCK_DISTANCE ||
					p1.x - t1.Size + ALLOWED_STUCK_DISTANCE > p2.x + t2.Size - ALLOWED_STUCK_DISTANCE ||
					p1.y - t1.Size + ALLOWED_STUCK_DISTANCE > p2.y + t2.Size - ALLOWED_STUCK_DISTANCE ||
					p1.y + t1.Size - ALLOWED_STUCK_DISTANCE < p2.y - t2.Size + ALLOWED_STUCK_DISTANCE)
				return false;

			// if either thing blocks full height there's no need to check the z-axis
			if (t1info.Blocking == ThingTypeInfo.THING_BLOCKING_FULL || t2info.Blocking == ThingTypeInfo.THING_BLOCKING_FULL)
				return true;

			// check z-axis
			if (p1.z > p2.z + t2info.Height || p1.z + t1info.Height < p2.z)
				return false;

			return true;
		}

		// Checks if the flags of two things overlap (i.e. if they show up at the same time)
		private bool FlagsOverlap(Thing t1, Thing t2)
		{
			Dictionary<string, List<ThingFlagsCompare> > groups = new Dictionary<string, List<ThingFlagsCompare> >();
			int overlappinggroups = 0;

			// Create a summary which flags belong to which groups
			foreach (ThingFlagsCompare tfc in General.Map.Config.ThingFlagsCompare)
			{
				if (!groups.ContainsKey(tfc.Group))
					groups[tfc.Group] = new List<ThingFlagsCompare>();

				groups[tfc.Group].Add(tfc);
			}

			// Go through all flags in all groups and check if they overlap
			foreach (string g in groups.Keys)
			{
				foreach (ThingFlagsCompare tfc in groups[g])
				{
					if (tfc.Compare(t1, t2) > 0)
					{
						overlappinggroups++;
						break;
					}
				}
			}

			// All groups have to overlap for the things to show up
			// at the same time
			if (overlappinggroups == groups.Count)
				return true;

			return false;
		}
	
		#endregion
	}
}
