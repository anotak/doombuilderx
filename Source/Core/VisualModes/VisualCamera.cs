#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	/// <summary>
	/// This class provides the camera in Visual Mode
	/// </summary>
	public class VisualCamera
	{
		#region ================== Constants

		private const float ANGLE_FROM_MOUSE = 0.0001f;
		public const float MAX_ANGLEZ_LOW = 91f / Angle2D.PIDEG;
		public const float MAX_ANGLEZ_HIGH = (360f - 91f) / Angle2D.PIDEG;
		public const float THING_Z_OFFSET = 41.0f;
		
		#endregion

		#region ================== Variables

		// Properties
		private Vector3D position;
		private Vector3D target;
		private Vector3D movemultiplier;
		private float anglexy, anglez;
		private Sector sector;
		
		#endregion

		#region ================== Properties

		public Vector3D Position { get { return position; } set { position = value; } }
		public Vector3D Target { get { return target; } }
		public float AngleXY { get { return anglexy; } set { anglexy = value; } }
		public float AngleZ { get { return anglez; } set { anglez = value; } }
		public Sector Sector { get { return sector; } internal set { sector = value; } }
		public Vector3D MoveMultiplier { get { return movemultiplier; } set { movemultiplier = value; } }
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public VisualCamera()
		{
			// Initialize
			this.movemultiplier = new Vector3D(1.0f, 1.0f, 1.0f);
			this.position = position;
			this.anglexy = 0.0f;
			this.anglez = Angle2D.PI;
			this.sector = null;
			
			PositionAtThing();
		}
		
		#endregion

		#region ================== Methods

		// Mouse input
		internal void ProcessMouseInput(Vector2D delta)
		{
			// Change camera angles with the mouse changes
			anglexy -= delta.x * ANGLE_FROM_MOUSE;
			if(General.Settings.InvertYAxis)
				anglez -= delta.y * ANGLE_FROM_MOUSE;
			else
				anglez += delta.y * ANGLE_FROM_MOUSE;

			// Normalize angles
			anglexy = Angle2D.Normalized(anglexy);
			anglez = Angle2D.Normalized(anglez);

			// Limit vertical angle
			if(anglez < MAX_ANGLEZ_LOW) anglez = MAX_ANGLEZ_LOW;
			if(anglez > MAX_ANGLEZ_HIGH) anglez = MAX_ANGLEZ_HIGH;
		}

		// Key input
		internal void ProcessMovement(Vector3D deltavec)
		{
			// Calculate camera direction vectors
			Vector3D camvec = Vector3D.FromAngleXYZ(anglexy, anglez);

			// Position the camera
			position += deltavec;
			
			// Target the camera
			target = position + camvec;
		}

		// This applies the position and angle from the 3D Camera Thing
		// Returns false when it couldn't find a 3D Camera Thing
		public virtual bool PositionAtThing()
		{
			Thing modething = null;
			Vector3D delta;
			
			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
				if(t.Type == General.Map.Config.Start3DModeThingType) modething = t;

			// Found one?
			if(modething != null)
			{
				modething.DetermineSector();
				float z = modething.Position.z;
				if(modething.Sector != null)
					z = modething.Position.z + (float)modething.Sector.FloorHeight;
				
				// Position camera here
				Vector3D wantedposition = new Vector3D(modething.Position.x, modething.Position.y, z + THING_Z_OFFSET);
				delta = position - wantedposition;
				if(delta.GetLength() > 1.0f) position = wantedposition;
				
				// Change angle
				float wantedanglexy = modething.Angle + Angle2D.PI;
				if(anglexy != wantedanglexy)
				{
					anglexy = wantedanglexy;
					anglez = Angle2D.PI;
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		
		// This applies the camera position and angle to the 3D Camera Thing
		// Returns false when it couldn't find a 3D Camera Thing
		public virtual bool ApplyToThing()
		{
			Thing modething = null;
			
			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
				if(t.Type == General.Map.Config.Start3DModeThingType) modething = t;

			// Found one?
			if(modething != null)
			{
				int z = 0;
				if(sector != null)
					z = (int)position.z - sector.FloorHeight;

				// Position the thing to match camera
				modething.Move((int)position.x, (int)position.y, z - THING_Z_OFFSET);
				modething.Rotate(anglexy - Angle2D.PI);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		#endregion
	}
}
