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

		internal static void Play(SoundClip soundClip, int track, float speed = 1.0f)
		{
			if (soundClip.Buffer == null)
				Load(soundClip);

			Tracks[track].Stop();
			Tracks[track].FlushSourceBuffers();

			Tracks[track].SetFrequencyRatio(speed);
			Tracks[track].SubmitSourceBuffer(soundClip.Buffer, null);

			Tracks[track].Start();
		}

		internal static void Load(SoundClip soundClip)
		{
			soundClip.Pointer = new DataPointer(Utilities.AllocateMemory(soundClip.Data.Length), soundClip.Data.Length);

			if(soundClip.Data.Length != 0)
				soundClip.Pointer.CopyFrom(soundClip.Data);

			soundClip.Buffer = new AudioBuffer(soundClip.Pointer);

			if (soundClip.RepeatCount != 0)
			{
				soundClip.Buffer.LoopCount = soundClip.RepeatCount;
				soundClip.Buffer.LoopBegin = soundClip.RepeatStart;
				soundClip.Buffer.LoopLength = soundClip.RepeatEnd - soundClip.RepeatStart;
			}
		}

		internal class SoundClip
		{
			internal byte[] Data;
			internal int RepeatCount;
			internal int RepeatStart;
			internal int RepeatEnd;

			internal AudioBuffer Buffer;
			internal DataPointer Pointer;
		}
	}
}
