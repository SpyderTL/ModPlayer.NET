using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ModPlayer
{
	internal class ModReader
	{
		internal static bool Read()
		{
			using (var stream = new MemoryStream(ModFile.Data))
			using (var reader = new BinaryReader(stream))
			{
				ModSong.Title = Encoding.ASCII.GetString(reader.ReadBytes(20)).Replace("\0", string.Empty);

				var sampleLengths = new int[31];
				ModSong.SampleVolumes = new int[31];
				ModSong.SampleRepeatStart = new int[31];
				ModSong.SampleRepeatLength = new int[31];

				for (var sample = 0; sample < 31; sample++)
				{
					var name = Encoding.ASCII.GetString(reader.ReadBytes(22)).Replace("\0", string.Empty);
					sampleLengths[sample] = (reader.ReadByte() << 9) | (reader.ReadByte() << 1);
					var fineTune = reader.ReadSByte();

					if (fineTune > 7)
						fineTune -= 16;

					ModSong.SampleVolumes[sample] = reader.ReadByte();
					ModSong.SampleRepeatStart[sample] = (reader.ReadByte() << 9) | (reader.ReadByte() << 1);
					ModSong.SampleRepeatLength[sample] = (reader.ReadByte() << 9) | (reader.ReadByte() << 1);
				}

				var patternSequenceCount = reader.ReadByte();
				var unused = reader.ReadByte();
				var patternSequence = reader.ReadBytes(128);

				ModSong.Sequence = Enumerable.Range(0, patternSequenceCount).Select(x => (int)patternSequence[x]).ToArray();

				var signature = reader.ReadChars(4);

				var patternCount = patternSequence.Max() + 1;

				ModSong.Patterns = new ModSong.Pattern[patternCount];

				for (var pattern = 0; pattern < patternCount; pattern++)
				{
					ModSong.Patterns[pattern].Divisions = new ModSong.Division[64];

					for (var division = 0; division < 64; division++)
					{
						ModSong.Patterns[pattern].Divisions[division].Channels = new ModSong.Channel[4];

						for (var channel = 0; channel < 4; channel++)
						{
							var value = reader.ReadUInt32();

							var sample = (value & 0xF0) | ((value >> 20) & 0x0F);
							var parameter = ((value & 0x0F) << 8) | ((value >> 8) & 0xFF);
							var effect = ((value >> 8) & 0xF00) | (value >> 24);

							ModSong.Patterns[pattern].Divisions[division].Channels[channel].Sample = sample;
							ModSong.Patterns[pattern].Divisions[division].Channels[channel].Parameter = parameter;
							ModSong.Patterns[pattern].Divisions[division].Channels[channel].Effect = effect;
						}
					}
				}

				ModSong.Samples = new sbyte[31][];

				for (var sample = 0; sample < 31; sample++)
				{
					var length = sampleLengths[sample];

					var samples = Enumerable.Range(0, length).Select(x => reader.ReadSByte()).ToArray();

					ModSong.Samples[sample] = samples;
				}
			}

			return true;
		}
	}
}