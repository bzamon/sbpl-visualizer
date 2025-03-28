
using System.Drawing;

namespace sbpl_visualizer
{
	public interface ISBPLCommand
	{
		void Execute(Graphics g, SBPLContext context, string argument);
	}
}
