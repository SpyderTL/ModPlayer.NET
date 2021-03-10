using System;
using System.Linq;

namespace ModPlayer
{
	internal static class ModPlayer
	{
		internal static bool Stopped;
		internal static int Position;
		internal static int Division;
		internal static long BeatsPerMinute;
		internal static int TicksPerDivision;
		internal static long Timer;
		internal static long Last;
		internal static bool[] ChannelTriggers;
		internal static int[] ChannelPitches;
		internal static int[] ChannelSamples;
		internal static int[] ChannelVolume;
		internal static long[] ChannelDelay;
		internal static long[] ChannelRepeat;
		internal static long[] ChannelRepeatTimer;

		internal static void Play()
		{
			Position = 0;
			BeatsPerMinute = 125;
			TicksPerDivision = 6;
			Timer = 0;
			Last = Environment.TickCount;

			ChannelTriggers = new bool[4];
			ChannelPitches = new int[4];
			ChannelSamples = new int[4];
			ChannelVolume = Enumerable.Repeat(64, 4).ToArray();
			ChannelDelay = new long[4];
			ChannelRepeat = new long[4];
			ChannelRepeatTimer = new long[4];

			Stopped = false;
		}

		internal static void Update()
		{
			if (Stopped)
				return;

			var current = Environment.TickCount;
			var elapsed = current - Last;
			Last = current;

			Timer -= elapsed;

			for (var channel2 = 0; channel2 < 4; channel2++)
				ChannelTriggers[channel2] = false;

			for (var channel2 = 0; channel2 < 4; channel2++)
			{
				if (ChannelDelay[channel2] > 0)
				{
					ChannelDelay[channel2] -= elapsed;

					if (ChannelDelay[channel2] <= 0)
						ChannelTriggers[channel2] = true;
				}
			}

			for (var channel2 = 0; channel2 < 4; channel2++)
			{
				if (ChannelRepeat[channel2] > 0)
				{
					ChannelRepeatTimer[channel2] -= elapsed;

					while (ChannelRepeatTimer[channel2] <= 0)
					{
						ChannelTriggers[channel2] = true;
						ChannelRepeatTimer[channel2] += ChannelRepeat[channel2];
					}
				}
			}

			if (Timer <= 0)
			{
				var nextPosition = Position;
				var nextDivision = Division + 1;

				if (nextDivision == 64)
				{
					nextPosition += 1;
					nextDivision = 0;

					//System.Diagnostics.Debug.WriteLine((nextPosition + 1).ToString());
				}

				for (var channel = 0; channel < 4; channel++)
				{
					var value = ModSong.Patterns[ModSong.Sequence[Position]].Divisions[Division].Channels[channel];

					if (value.Sample != 0)
					{
						ChannelTriggers[channel] = true;
						ChannelSamples[channel] = (int)value.Sample - 1;
						ChannelVolume[channel] = ModSong.SampleVolumes[value.Sample - 1];
						ChannelRepeat[channel] = 0;
						ChannelDelay[channel] = 0;
					}
					
					if (value.Effect != 0)
					{
						var type = value.Effect >> 8;
						var parameter = (value.Effect >> 4) & 0x0F;
						var parameter2 = value.Effect & 0x0F;

						switch (type)
						{
							case 0x00:
								// Arpegio
								break;

							case 0x01:
								// Pitch Bend Up
								break;

							case 0x02:
								// Pitch Bend Down
								break;

							case 0x03:
								// Pitch Bend To Note
								break;

							case 0x04:
								// Vibrato
								break;

							case 0x05:
								// Pitch Bend + Volume Slide
								break;

							case 0x06:
								// Vibrato + Volume Slide
								break;

							case 0x09:
								// Set Sample Start
								break;

							case 0x0A:
								if (parameter != 0)
								{
									ChannelVolume[channel] += (int)parameter * (TicksPerDivision - 1);

									if (ChannelVolume[channel] > 64)
										ChannelVolume[channel] = 64;
								}
								else
								{
									ChannelVolume[channel] -= (int)parameter2 * (TicksPerDivision - 1);

									if (ChannelVolume[channel] < 0)
										ChannelVolume[channel] = 0;
								}
								break;

							case 0x0B:
								// Jump
								nextPosition = (int)((parameter << 4) | parameter2);
								nextDivision = 0;
								break;

							case 0x0C:
								ChannelVolume[channel] = (int)((parameter << 4) | parameter2);
								break;

							case 0x0D:
								nextPosition = Position + 1;
								nextDivision = (int)((parameter * 10) + parameter2);

								//System.Diagnostics.Debug.WriteLine((nextPosition + 1).ToString());

								break;

							case 0x0E:
								switch (parameter)
								{
									case 0x00:
										// Filter Disable
										break;

									case 0x09:
										// Retrigger Sample
										var divisionsPerMinute = (24 * BeatsPerMinute) / TicksPerDivision;
										var ticksPerMinute = divisionsPerMinute * TicksPerDivision;
										var ticksPerMillisecond = ticksPerMinute / 60000.0f;

										ChannelRepeat[channel] = (long)(parameter2 / ticksPerMillisecond);
										break;

									case 0x0A:
										ChannelVolume[channel] += (int)parameter2;

										if (ChannelVolume[channel] > 64)
											ChannelVolume[channel] = 64;
										break;

									case 0x0B:
										ChannelVolume[channel] -= (int)parameter2;

										if (ChannelVolume[channel] < 0)
											ChannelVolume[channel] = 0;
										break;

									case 0x0C:
										ChannelVolume[channel] = 0;
										break;

									case 0x0D:
										// Delay Sample
										ChannelTriggers[channel] = false;

										divisionsPerMinute = (24 * BeatsPerMinute) / TicksPerDivision;
										ticksPerMinute = divisionsPerMinute * TicksPerDivision;
										ticksPerMillisecond = ticksPerMinute / 60000.0f;

										//ChannelDelay[channel] = (long)(parameter2 / ticksPerMillisecond);
										break;

									case 0x0E:
										// Delay Pattern
										break;

									case 0x0F:
										// Invert Loop
										break;

									default:
										break;
								}
								break;

							case 0x0F:
								var value2 = ((parameter << 4) | parameter2);

								Console.WriteLine(value2);

								if (value2 == 0)
									TicksPerDivision = 1;
								else if (value2 <= 32)
									TicksPerDivision = (int)value2;
								else
									BeatsPerMinute = value2;
								break;

							default:
								break;
						}
					}

					if (value.Parameter != 0)
					{
						ChannelPitches[channel] = (int)value.Parameter;
					}
				}

				Position = nextPosition;
				Division = nextDivision;

				if (Position >= ModSong.Sequence.Length)
				{
					ChannelVolume[0] = 0;
					ChannelVolume[1] = 0;
					ChannelVolume[2] = 0;
					ChannelVolume[3] = 0;

					Stopped = true;
				}
				else
				{
					var divisionsPerMinute = (24 * BeatsPerMinute) / TicksPerDivision;

					Timer += 60000L / divisionsPerMinute;
				}
			}
		}
	}
}