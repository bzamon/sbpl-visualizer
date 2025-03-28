﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sbpl_visualizer
{
	public class SBPLContext
	{
		public int ScaleX { get; set; } = 1;
		public int ScaleY { get; set; } = 1;

		public int X { get; set; }
		public int Y { get; set; }
		public int OffsetX { get; set; } = 0;
		public int OffsetY { get; set; } = 0;
		public Font CurrentFont { get; set; } = new Font("Arial", 12);
		public string FontCode { get; set; } = "M"; // Default to Medium
		public int Rotation { get; set; } = 0; // 0, 90, 180, 270 degrees

	}
}
