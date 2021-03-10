using System;

namespace ModPlayer
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			ModFile.Load(@"..\..\Examples\chcknbnk.mod");
			//ModFile.Load(@"..\..\Examples\spacedeb.mod");
			//ModFile.Load(@"..\..\Examples\hoffman_and_daytripper_-_professional_tracker.mod");
			ModReader.Read();
			ModTracker.Load();

			ProgramWindow.Show();
		}
	}
}
