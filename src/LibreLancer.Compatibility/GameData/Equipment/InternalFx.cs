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
 * Portions created by the Initial Developer are Copyright (C) 2013-2017
 * the Initial Developer. All Rights Reserved.
 */
using System;
using LibreLancer.Ini;
namespace LibreLancer.Compatibility.GameData.Equipment
{
	public class InternalFx : AbstractEquipment
	{
		public string UseAnimation;
		public string UseSound;
		public InternalFx(Section section, FreelancerData gdata)
			: base(section)
		{
			foreach (Entry e in section)
			{
				if (!parentEntry(e))
				{
					switch (e.Name.ToLowerInvariant())
					{
						case "use_animation":
							UseAnimation = e[0].ToString();
							break;
						case "use_sound":
							UseSound = e[0].ToString();
							break;
					}
				}
			}
		}
	}
}
