﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2016
 * the Initial Developer. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using LibreLancer.Thorn;
namespace LibreLancer
{
	public class ThnScript
	{
		#region Runtime
		public static Dictionary<string,object> ThnEnv = new Dictionary<string, object>();
		static ThnScript()
		{
			//ThnObjectFlags
			ThnEnv.Add("LIT_DYNAMIC", ThnObjectFlags.LitDynamic);
			ThnEnv.Add("LIT_AMBIENT", ThnObjectFlags.LitAmbient);
			ThnEnv.Add("HIDDEN", ThnObjectFlags.Hidden);
			ThnEnv.Add("REFERENCE", ThnObjectFlags.Reference);
			ThnEnv.Add("SPATIAL", ThnObjectFlags.Spatial);
			//EventFlags
			ThnEnv.Add("LOOP", EventFlags.Loop);
			//LightTypes
			ThnEnv.Add("L_DIRECT", LightTypes.Direct);
			ThnEnv.Add("L_POINT", LightTypes.Point);
			ThnEnv.Add("L_SPOT", LightTypes.Spotlight);
			//TargetTypes
			ThnEnv.Add("HARDPOINT", TargetTypes.Hardpoint);
			ThnEnv.Add("PART", TargetTypes.Part);
			ThnEnv.Add("ROOT", TargetTypes.Root);
			//AttachFlags
			ThnEnv.Add("POSITION", AttachFlags.Position);
			ThnEnv.Add("ORIENTATION", AttachFlags.Orientation);
			ThnEnv.Add("LOOK_AT", AttachFlags.LookAt);
			ThnEnv.Add("ENTITY_RELATIVE", AttachFlags.EntityRelative);
			ThnEnv.Add("ORIENTATION_RELATIVE", AttachFlags.OrientationRelative);
			ThnEnv.Add("PARENT_CHILD", AttachFlags.ParentChild);
			//EntityTypes
			ThnEnv.Add("CAMERA", EntityTypes.Camera);
			ThnEnv.Add("PSYS", EntityTypes.PSys);
			ThnEnv.Add("MONITOR", EntityTypes.Monitor);
			ThnEnv.Add("SCENE", EntityTypes.Scene);
			ThnEnv.Add("MARKER", EntityTypes.Marker);
			ThnEnv.Add("COMPOUND", EntityTypes.Compound);
			ThnEnv.Add("LIGHT", EntityTypes.Light);
			ThnEnv.Add("MOTION_PATH", EntityTypes.MotionPath);
			ThnEnv.Add("DEFORMABLE", EntityTypes.Deformable);
			ThnEnv.Add("SOUND", EntityTypes.Sound);
			//FogModes
			ThnEnv.Add("F_EXP2", FogModes.Exp2);
			ThnEnv.Add("F_LINEAR", FogModes.Linear);
			//EventTypes
			ThnEnv.Add("SET_CAMERA", EventTypes.SetCamera);
			ThnEnv.Add("ATTACH_ENTITY", EventTypes.AttachEntity);
			ThnEnv.Add("START_SPATIAL_PROP_ANIM", EventTypes.StartSpatialPropAnim);
			ThnEnv.Add("START_LIGHT_PROP_ANIM", EventTypes.StartLightPropAnim);
			ThnEnv.Add("START_PSYS", EventTypes.StartPSys);
			ThnEnv.Add("START_PSYS_PROP_ANIM", EventTypes.StartPSysPropAnim);
			ThnEnv.Add("START_PATH_ANIMATION", EventTypes.StartPathAnimation);
			ThnEnv.Add("START_MOTION", EventTypes.StartMotion);
			ThnEnv.Add("START_FOG_PROP_ANIM", EventTypes.StartFogPropAnim);
			ThnEnv.Add("START_CAMERA_PROP_ANIM", EventTypes.StartCameraPropAnim);
			ThnEnv.Add("START_SOUND", EventTypes.StartSound);
			//Axis
			ThnEnv.Add("X_AXIS", VectorMath.UnitX);
			ThnEnv.Add("Y_AXIS", VectorMath.UnitY);
			ThnEnv.Add("Z_AXIS", VectorMath.UnitZ);
			ThnEnv.Add("NEG_X_AXIS", -VectorMath.UnitX);
			ThnEnv.Add("NEG_Y_AXIS", -VectorMath.UnitY);
			ThnEnv.Add("NEG_Z_AXIS", -VectorMath.UnitZ);
			//Booleans
			ThnEnv.Add("Y", true);
			ThnEnv.Add("N", false);

		}
		#endregion

		public double Duration;
		public Dictionary<string, ThnEntity> Entities = new Dictionary<string, ThnEntity>(StringComparer.OrdinalIgnoreCase);
		public List<ThnEvent> Events = new List<ThnEvent>();
		public ThnScript (string scriptfile)
		{
			var runner = new LuaRunner (ThnEnv);
			var output = runner.DoFile (scriptfile);
			Duration = (float)output["duration"];
			var entities = (LuaTable)output["entities"];
			for (int i = 0; i < entities.Capacity; i++)
			{
				var ent = (LuaTable)entities[i];
				var e = GetEntity(ent);
				Entities.Add(e.Name, e);
			}
			var events = (LuaTable)output["events"];
			for (int i = 0; i < events.Capacity; i++)
			{
				var ev = (LuaTable)events[i];
				var e = GetEvent(ev);
				Events.Add(e);
			}
			Events.Sort((x, y) => x.Time.CompareTo(y.Time));
		}
		ThnEvent GetEvent(LuaTable table)
		{
			var e = new ThnEvent();
			e.Time = (float)table[0];
			e.Type = ThnEnum.Check<EventTypes>(table[1]);
			e.Targets = (LuaTable)table[2];
			if (table.Capacity >= 4)
			{
				e.Properties = (LuaTable)table[3];
				//Get properties common to most events
				object tmp;
				if (e.Properties.TryGetValue("param_curve", out tmp))
				{
					e.ParamCurve = new ParameterCurve((LuaTable)tmp);
					if (e.Properties.TryGetValue("pcurve_period", out tmp))
					{
						e.ParamCurve.Period = (float)tmp;
					}
				}
				if (e.Properties.TryGetValue("duration", out tmp))
				{
					e.Duration = (float)tmp;
				}
			}
			return e;
		}
		//Flags are stored differently internally between Freelancer and Librelancer
		ThnObjectFlags ConvertFlags(EntityTypes type, LuaTable table)
		{
			var val = (int)(float)table["flags"];
			if (val == 1) return ThnObjectFlags.Reference; //Should be for all types
			if (type == EntityTypes.Sound)
			{
				switch (val)
				{
					case 2:
						return ThnObjectFlags.Spatial;
					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				switch (val)
				{
					case 4:
						return ThnObjectFlags.LitDynamic;
					case (2 | 4):
						return ThnObjectFlags.LitDynamic | ThnObjectFlags.LitAmbient;
					default:
						throw new NotImplementedException();
				}
			}
		}

		ThnEntity GetEntity(LuaTable table)
		{
			var e = new ThnEntity();
			e.Name = (string)table["entity_name"];
			e.Type = ThnEnum.Check<EntityTypes>(table["type"]);
			e.LightGroup = (int)(float)table["lt_grp"];
			e.SortGroup = (int)(float)table["srt_grp"];
			e.UserFlag = (int)(float)table["usr_flg"];
			Vector3 tmp;
			if (table.TryGetVector3("ambient", out tmp))
			{
				e.Ambient = tmp;
			}
			if (table.TryGetVector3("up", out tmp))
			{
				e.Up = tmp;
			}
			if (table.TryGetVector3("front", out tmp))
			{
				e.Front = tmp;
			}
			object o;
			if (table.TryGetValue("template_name", out o))
			{
				e.Template = (string)o;
			}
			if (table.TryGetValue("flags", out o))
			{
				if (o is float)
				{
					e.ObjectFlags = ConvertFlags(e.Type, table);
				}
				else
				{
					e.ObjectFlags = (ThnObjectFlags)o;
				}
			}
			if (table.TryGetValue("userprops", out o))
			{
				var usrprops = (LuaTable)o;
				if (usrprops.TryGetValue("category", out o))
				{
					e.MeshCategory = (string)o;
				}
			}

			if (table.TryGetValue("spatialprops", out o))
			{
				var spatialprops = (LuaTable)o;
				if (spatialprops.TryGetVector3("pos", out tmp))
				{
					e.Position = tmp;
				}
				if (spatialprops.TryGetValue("orient", out o))
				{
					var orient = (LuaTable)o;
					var m11 = (float)((LuaTable)orient[0])[0];
					var m12 = (float)((LuaTable)orient[0])[1];
					var m13 = (float)((LuaTable)orient[0])[2];

					var m21 = (float)((LuaTable)orient[1])[0];
					var m22 = (float)((LuaTable)orient[1])[1];
					var m23 = (float)((LuaTable)orient[1])[2];

					var m31 = (float)((LuaTable)orient[2])[0];
					var m32 = (float)((LuaTable)orient[2])[1];
					var m33 = (float)((LuaTable)orient[2])[2];
					e.RotationMatrix = new Matrix4(
						m11, m12, m13, 0,
						m21, m22, m23, 0,
						m31, m32, m33, 0,
						0  , 0  , 0  , 1
					);
				}
			}

			if (table.TryGetValue("cameraprops", out o))
			{
				var cameraprops = (LuaTable)o;
				if (cameraprops.TryGetValue("fovh", out o))
				{
					e.FovH = (float)o;
				}
				if (cameraprops.TryGetValue("hvaspect", out o))
				{
					e.HVAspect = (float)o;
				}
			}
			if (table.TryGetValue("lightprops", out o))
			{
				var lightprops = (LuaTable)o;
				e.LightProps = new ThnLightProps();
				if (lightprops.TryGetValue("on", out o))
				{
					e.LightProps.On = ThnEnum.Check<bool>(o);
				}
				else
					e.LightProps.On = true;
				var r = new RenderLight();
				r.Position = e.Position.Value;
				if (lightprops.TryGetValue("type", out o))
				{
					var tp = ThnEnum.Check<LightTypes>(o);
					if (tp == LightTypes.Point)
						r.Kind = LightKind.Point;
					if (tp == LightTypes.Direct)
						r.Kind = LightKind.Directional;
					if (tp == LightTypes.Spotlight)
					{
						r.Kind = LightKind.Spotlight;
						r.Falloff = 1f;
					}
				}
				else
					throw new Exception("Light without type");
				if (lightprops.TryGetVector3("diffuse", out tmp))
					r.Color = new Color4(tmp.X, tmp.Y, tmp.Z, 1);
				if (lightprops.TryGetVector3("direction", out tmp))
					r.Direction = tmp;
				if (lightprops.TryGetValue("range", out o))
					r.Range = (int)(float)o;
				if (lightprops.TryGetValue("theta", out o))
					r.Theta = r.Phi = (float)o;
				if (lightprops.TryGetVector3("atten", out tmp))
				{
					r.Attenuation = new Vector4(tmp, 0);
				}
				e.LightProps.Render = r;
			}
			if (table.TryGetValue("pathprops", out o))
			{
				var pathprops = (LuaTable)o;
				if (pathprops.TryGetValue("path_data", out o))
				{
					e.Path = new MotionPath((string)o);
				}
			}
			return e;
		}

	}
}

