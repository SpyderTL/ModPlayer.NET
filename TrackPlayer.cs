using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace ModPlayer
{
	internal static class TrackPlayer
	{
		internal static XAudio2 Audio = new XAudio2();
		internal static MasteringVoice Voice;
		internal static SourceVoice[] Tracks;
		internal static WaveFormat Format;
		internal static int FramesPerSecond;
		internal static int ChannelsPerFrame;
		internal static int BytesPerChannel;

		internal static void Enable(int tracks, int framesPerSecond, int channelsPerFrame, int bytesPerChannel)
		{
			FramesPerSecond = framesPerSecond;
			ChannelsPerFrame = channelsPerFrame;
			BytesPerChannel = bytesPerChannel;

			Audio.StartEngine();

			Voice = new MasteringVoice(Audio);
			Format = new WaveFormat(framesPerSecond, bytesPerChannel * 8, channelsPerFrame);
			Tracks = new SourceVoice[tracks];

			for (var track = 0; track < tracks; track++)
				Tracks[track] = new SourceVoice(Audio, Format, VoiceFlags.None, 6.0f);
		}

		internal static void Disable()
		{
			Audio.StopEngine();
		}

		internal static void Play(SoundClip soundClip, int track, float speed = 1.0f, int start = 0)
		{
			if (soundClip.Buffers == null)
				Load(soundClip);

			Tracks[track].Stop();
			Tracks[track].FlushSourceBuffers();

			if (start < soundClip.Pointer.Size)
			{
				Tracks[track].SetFrequencyRatio(speed);
				soundClip.Buffers[track].PlayBegin = start;
				Tracks[track].SubmitSourceBuffer(soundClip.Buffers[track], null);

				Tracks[track].Start();
			}
		}

		internal static void Stop()
		{
			for (var track = 0; track < Tracks.Length; track++)
			{
				Tracks[track].Stop();
				Tracks[track].FlushSourceBuffers();
			}
		}

		internal static void Load(SoundClip soundClip)
		{
			if (Tracks == null)
				return;

			soundClip.Pointer = new DataPointer(Utilities.AllocateMemory(soundClip.Data.Length), soundClip.Data.Length);

			if(soundClip.Data.Length != 0)
				soundClip.Pointer.CopyFrom(soundClip.Data);

			soundClip.Buffers = new AudioBuffer[Tracks.Length];

			for (var buffer = 0; buffer < soundClip.Buffers.Length; buffer++)
			{
				soundClip.Buffers[buffer] = new AudioBuffer(soundClip.Pointer);

				if (soundClip.RepeatCount != 0)
				{
					soundClip.Buffers[buffer].LoopCount = soundClip.RepeatCount == -1 ? 255 : soundClip.RepeatCount;
					soundClip.Buffers[buffer].LoopBegin = soundClip.RepeatStart;
					soundClip.Buffers[buffer].LoopLength = soundClip.RepeatEnd - soundClip.RepeatStart;
				}
			}
		}

		internal class SoundClip
		{
			internal byte[] Data;
			internal int RepeatCount;
			internal int RepeatStart;
			internal int RepeatEnd;

			internal AudioBuffer[] Buffers;
			internal DataPointer Pointer;
		}
	}
}
