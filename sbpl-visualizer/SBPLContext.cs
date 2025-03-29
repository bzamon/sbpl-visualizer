using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sbpl_visualizer
{
	public class SBPLContext
	{
		public int X { get; set; }
		public int Y { get; set; }
		public Font CurrentFont { get; set; } = new Font("Arial", 12);
		public string FontCode { get; set; } = "M"; // Default to Medium


	}
}
