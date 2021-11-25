using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModPlayer
{
	internal static class ModTracker
	{
		internal static TrackPlayer.SoundClip[] SoundClips;
		internal static int[] ChannelVolumes = new int[4];

		internal static void Load()
		{
			SoundClips = new TrackPlayer.SoundClip[ModSong.Samples.Length];

			for (var sample = 0; sample < SoundClips.Length; sample++)
			{
				SoundClips[sample] = new TrackPlayer.SoundClip
				{
					Data = ModSong.Samples[sample].Select(x => (byte)((int)x + 128)).ToArray()
				};

				if (ModSong.SampleRepeatLength[sample] > 2)
				{
					SoundClips[sample].RepeatCount = -1;
					SoundClips[sample].RepeatStart = ModSong.SampleRepeatStart[sample];
					SoundClips[sample].RepeatEnd = ModSong.SampleRepeatStart[sample] + ModSong.SampleRepeatLength[sample];
				}

				TrackPlayer.Load(SoundClips[sample]);
			}
		}

		internal static void Update()
		{
			for (var channel = 0; channel < ModPlayer.ChannelTriggers.Length; channel++)
			{
				if (ModPlayer.ChannelVolume[channel] != ChannelVolumes[channel])
				{
					TrackPlayer.Tracks[channel].SetVolume(ModPlayer.ChannelVolume[channel] / 64.0f);

					ChannelVolumes[channel] = ModPlayer.ChannelVolume[channel];
				}

				if (ModPlayer.ChannelTriggers[channel])
				{
					float speed = 1.0f / ((ModPlayer.ChannelPitches[channel] - ModPlayer.ChannelTunings[channel]) / 428.0f);

					//Console.WriteLine(channel.ToString() + ": " + ModPlayer.ChannelSamples[channel].ToString("X2") + ": " + speed);
					
					TrackPlayer.Play(SoundClips[ModPlayer.ChannelSamples[channel]], channel, speed, ModPlayer.ChannelStart[channel]);
				}
			}
		}
	}
}
