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
	internal static class SoundPlayer
	{
		internal static XAudio2 Audio = new XAudio2();
		internal static MasteringVoice Voice;

		internal static void Enable()
		{
			Audio.StartEngine();
			Voice = new MasteringVoice(Audio);
		}

		internal static void Disable()
		{
			Audio.StopEngine();
		}

		internal static void Play(SoundClip soundClip)
		{
			if (soundClip.Buffer == null)
				Load(soundClip);

			soundClip.Voice.Stop();
			soundClip.Voice.FlushSourceBuffers();
			soundClip.Voice.SubmitSourceBuffer(soundClip.Buffer, null);
			soundClip.Voice.Start();
		}

		internal static void Load(SoundClip soundClip)
		{
			soundClip.Pointer = new DataPointer(Utilities.AllocateMemory(soundClip.Data.Length), soundClip.Data.Length);
			soundClip.Pointer.CopyFrom(soundClip.Data);
			soundClip.Buffer = new AudioBuffer(soundClip.Pointer);

			soundClip.Format = new WaveFormat(soundClip.FramesPerSecond, soundClip.BytesPerChannel * 8, soundClip.ChannelsPerFrame);
			soundClip.Voice = new SourceVoice(Audio, soundClip.Format);
		}

		internal class SoundClip
		{
			internal int FramesPerSecond;
			internal int ChannelsPerFrame;
			internal int BytesPerChannel;
			internal byte[] Data;

			internal AudioBuffer Buffer;
			internal DataPointer Pointer;
			internal WaveFormat Format;
			internal SourceVoice Voice;
		}
	}
}
