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
 * The Original Code is FlLApi code (http://flapi.sourceforge.net/).
 * Data structure from Freelancer UTF Editor by Cannon & Adoxa, continuing the work of Colin Sanby and Mario 'HCl' Brito (http://the-starport.net)
 * 
 * The Initial Developer of the Original Code is Malte Rupprecht (mailto:rupprema@googlemail.com).
 * Portions created by the Initial Developer are Copyright (C) 2011, 2012
 * the Initial Developer. All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace LibreLancer.Utf
{
    public class SphereConstruct : AbstractConstruct
    {
		public Vector3 Offset { get; private set; }
        public float Min1 { get; private set; }
        public float Max1 { get; private set; }
        public float Min2 { get; private set; }
        public float Max2 { get; private set; }
        public float Min3 { get; private set; }
        public float Max3 { get; private set; }

        public override Matrix4 Transform { get { return internalGetTransform(Rotation * Matrix4.CreateTranslation(Origin + Offset)); } }

        public SphereConstruct(BinaryReader reader, ConstructCollection constructs)
            : base(reader, constructs)
        {
            Offset = ConvertData.ToVector3(reader);
            Rotation = ConvertData.ToMatrix3x3(reader);

            Min1 = reader.ReadSingle();
            Max1 = reader.ReadSingle();
            Min2 = reader.ReadSingle();
            Max2 = reader.ReadSingle();
            Min3 = reader.ReadSingle();
            Max3 = reader.ReadSingle();
        }
		protected SphereConstruct(SphereConstruct cf) : base(cf) { }
		public override AbstractConstruct Clone(ConstructCollection newcol)
		{
			var newc = new SphereConstruct(this);
			newc.constructs = newcol;
			newc.Offset = Offset;
			newc.Min1 = Min1;
			newc.Min2 = Min2;
			newc.Min3 = Min3;
			newc.Max1 = Max1;
			newc.Max2 = Max2;
			newc.Max3 = Max3;
			return newc;
		}

        public override void Update(float distance)
        {
            //throw new NotImplementedException();
        }
    }
}
