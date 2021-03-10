using System;
using System.Linq;
using System.Windows.Forms;

namespace ModPlayer
{
	internal static class ProgramWindow
	{
		internal static SongForm Form;
		internal static System.Threading.Timer Timer = new System.Threading.Timer(Timer_Elapsed);

		internal static void Show()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(true);

			TrackPlayer.Enable(4, 8287, 1, 1);

			Form = new SongForm();

			Form.PlayButton.Click += PlayButton_Click;
			Form.stopButton.Click += StopButton_Click;

			Application.Run(Form);
		}

		private static void StopButton_Click(object sender, EventArgs e)
		{
			Timer.Change(0, System.Threading.Timeout.Infinite);
		}

		private static void PlayButton_Click(object sender, EventArgs e)
		{
			ModPlayer.Play();

			Timer.Change(0, 10);
		}

		private static void Timer_Elapsed(object state)
		{
			Timer.Change(0, System.Threading.Timeout.Infinite);

			ModPlayer.Update();
			ModTracker.Update();

			Timer.Change(0, 10);
		}
	}
}