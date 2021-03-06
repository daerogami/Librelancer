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
namespace LibreLancer
{
    public abstract class Texture : IDisposable
    {
        public uint ID;
        public SurfaceFormat Format { get; protected set; }
        static bool compressedChecked = false;
        protected static void CheckCompressed()
        {
            if (!compressedChecked)
            {
                GLExtensions.CheckExtensions();
                compressedChecked = true;
            }
        }
        public int LevelCount
        {
            get;
            protected set;
        }
        bool isDisposed = false;
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        public abstract void BindTo(int unit);

        internal static int CalculateMipLevels(int width, int height = 0)
        {
            int levels = 1;
            int size = Math.Max(width, height);
            while (size > 1)
            {
                size = size / 2;
                levels++;
            }
            return levels;
        }

		public override int GetHashCode()
		{
			unchecked
			{
				return (int)ID;
			}
		}
        public virtual void Dispose()
        {
			GL.DeleteTexture(ID);
            isDisposed = true;
        }
    }
}

