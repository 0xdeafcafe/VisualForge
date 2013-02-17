﻿/* Copyright 2012 Alex Reed
 * 
 * This file is part of Visual Forge.
 * 
 * VisualForge is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * VisualForge is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with VisualForge.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * End Note:
 * This class library was re-coded from PartyBlam (https://github.com/Xerax/PartyBlam), by Alex Reed.
 */


using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisualForge.Core.Helpers;
using VisualForge.IO;

namespace VisualForge.Usermaps.Games
{
	public class Halo3 : IUsermap
	{
		public Halo3(string filePath) { Initalize(new FileStream(filePath, FileMode.Open)); }
		public Halo3(byte[] file) { Initalize(new MemoryStream(file)); }
		public Halo3(Stream fileStream) { Initalize(fileStream); }

		private EndianStream _sandboxStream;

		private Header _sandboxeHeader;
		private MapMetaData _sandboxMapMetaData;
		private IList<ObjectChunk> _sandboxObjects;
		private IList<TagEntry> _sandboxTagEntries; 

		private const string _gameId = "4D5307E6";

		private void Initalize(Stream fileStream)
		{
			_sandboxStream = new EndianStream(fileStream, Endian.BigEndian);
			if (!ValidateUsermap())
			{
				Close();
				throw new InvalidOperationException("Invalid Halo 3 Usermap!");
			}

			LoadHeader();
			LoadMapMetaData();
			LoadObjectChunks();
			LoadTagEntries();
			BindTagEntryData();
		}
		private bool ValidateUsermap()
		{
			_sandboxStream.SeekTo(0x138);
			return (_sandboxStream.ReadAscii(0x04) == "mapv");
		}
		public void Close()
		{
			_sandboxStream.Close();
		}


		private void LoadHeader()
		{
			_sandboxeHeader = new Header();

			_sandboxStream.SeekTo(0x42);
			_sandboxeHeader.CreationDate =					_sandboxStream.ReadInt32();
			_sandboxStream.SeekTo(0x48);
			_sandboxeHeader.CreationVarientName =			_sandboxStream.ReadUTF16(0x1F);
			_sandboxStream.SeekTo(0x68);
			_sandboxeHeader.CreationVarientDescription =	_sandboxStream.ReadAscii(0x80);
			_sandboxStream.SeekTo(0xE8);
			_sandboxeHeader.CreationVarientAuthor =		_sandboxStream.ReadAscii(0x13);

			_sandboxStream.SeekTo(0x114);
			_sandboxeHeader.ModificationDate =				_sandboxStream.ReadInt32();
			_sandboxStream.SeekTo(0x150);
			_sandboxeHeader.VarientName =					_sandboxStream.ReadUTF16(0x1F);
			_sandboxStream.SeekTo(0x170);
			_sandboxeHeader.VarientDescription =			_sandboxStream.ReadAscii(0x80);
			_sandboxStream.SeekTo(0x1F0);
			_sandboxeHeader.VarientAuthor =				_sandboxStream.ReadAscii(0x13);

			_sandboxStream.SeekTo(0x228);
			_sandboxeHeader.MapID =						_sandboxStream.ReadInt32();

			_sandboxStream.SeekTo(0x246);
			_sandboxeHeader.SpawnedObjectCount =			_sandboxStream.ReadInt16();

			_sandboxStream.SeekTo(0x24C);
			_sandboxeHeader.WorldBoundsX =					new Header.WorldBound
															{
																Min = _sandboxStream.ReadFloat(),
																Max = _sandboxStream.ReadFloat()
															};
			_sandboxeHeader.WorldBoundsY =					new Header.WorldBound
															{
																Min = _sandboxStream.ReadFloat(),
																Max = _sandboxStream.ReadFloat()
															};
			_sandboxeHeader.WorldBoundsZ =					new Header.WorldBound
															{
																Min = _sandboxStream.ReadFloat(),
																Max = _sandboxStream.ReadFloat()
															};

			_sandboxStream.SeekTo(0x268);
			_sandboxeHeader.MaximiumBudget =				_sandboxStream.ReadFloat();
			_sandboxeHeader.CurrentBudget =				_sandboxStream.ReadFloat();
		}
		private void LoadMapMetaData()
		{
			var taglist = 
				VariousFunctions.GZip.Decompress(VariousFunctions.GetTaglistFile(_gameId, _sandboxeHeader.MapID));

			_sandboxMapMetaData = JsonConvert.DeserializeObject<MapMetaData>(VariousFunctions.ByteArrayToString(taglist, VariousFunctions.EncodingType.ASCII));
		}
		private void LoadObjectChunks()
		{
			_sandboxStream.SeekTo(0x279);
			_sandboxObjects = new List<ObjectChunk>();
			for(var chunk = 0; chunk < 640; chunk++)
				_sandboxObjects.Add(new ObjectChunk(_sandboxStream));
		}
		private void LoadTagEntries()
		{
			_sandboxTagEntries = new List<TagEntry>();
			_sandboxStream.SeekTo(0xD494);
			for(var entry = 0; entry < 0x100; entry++)
			{
				var tagEntry = new TagEntry(_sandboxStream, _sandboxMapMetaData);
				if (tagEntry.Tag != null)
					tagEntry.Tag.TagIndex = entry;
				_sandboxTagEntries.Add(tagEntry);
			}
		}
		private void BindTagEntryData()
		{
			foreach(var tagEntry in _sandboxTagEntries)
				if (tagEntry.CountOnMap > 0)
					foreach(var placedObject in _sandboxObjects.Where(placedObject => placedObject.TagIndex == tagEntry.Tag.TagIndex))
						tagEntry.PlacedObjects.Add(placedObject);

			for (var i = 0; i < 0x100; i++)
				for (var j = 0; j < 640; j++)
					if (_sandboxObjects[j].TagIndex == i)
						_sandboxObjects[j].TagEntry = _sandboxTagEntries[i];
		}

		// Classes
		public class Header
		{
			public Int32 CreationDate { get; set; }
			public string CreationVarientName { get; set; }
			public string CreationVarientDescription { get; set; }
			public string CreationVarientAuthor { get; set; }

			public Int32 ModificationDate { get; set; }
			public string VarientName { get; set; }
			public string VarientDescription { get; set; }
			public string VarientAuthor { get; set; }

			public Int32 MapID { get; set; }
			public Int16 SpawnedObjectCount { get; set; }

			public WorldBound WorldBoundsX { get; set; }
			public WorldBound WorldBoundsY { get; set; }
			public WorldBound WorldBoundsZ { get; set; }

			public class WorldBound
			{
				public float Min { get; set; }
				public float Max { get; set; }
			}

			public float MaximiumBudget { get; set; }
			public float CurrentBudget { get; set; }
		}
		public class MapMetaData
		{
			public string MapName { get; set; }
			public Int32 MapID { get; set; }
			public List<Tag> Tags { get; set; }

			/// <summary>
			/// Gets a tag based on it's Datum Index.
			/// </summary>
			/// <param name="datumIndex">The datum index of the tag you're searching for.</param>
			public Tag GetTag(Int32 datumIndex)
			{
				return Tags.FirstOrDefault(Tag => Tag.DatumIndex == datumIndex);
			}

			public class Tag
			{
				public string TagClass { get; set; }
				public string TagPath { get; set; }
				public int TagIndex { get; set; }
				public Int32 DatumIndex { get; set; }
			}
		}
		public class ObjectChunk
		{
			public ObjectChunk(EndianStream stream)
			{
				Offset = stream.Position;
				Load(stream);
			}

			public long Offset { get; set; }
			public int TagIndex { get; set; }
			public Coordinates SpawnCoordinates { get; set; }
			public byte Team { get; set; }
			public byte RespawnTime { get; set; }
			public TagEntry TagEntry { get; set; }

			public void Load(EndianStream stream)
			{
				stream.SeekTo(Offset + 0x0C);
				TagIndex = stream.ReadInt32();
				SpawnCoordinates = new Coordinates
					                   {
						                   X = stream.ReadFloat(),
										   Y = stream.ReadFloat(),
										   Z = stream.ReadFloat(),
										   Yaw = stream.ReadFloat(),
										   Pitch = stream.ReadFloat(),
										   Roll = stream.ReadFloat()
					                   };
				stream.SeekTo(stream.Position + 0x17);
				Team = stream.ReadByte();
				stream.SeekTo(stream.Position + 0x01);
				RespawnTime = stream.ReadByte();
				stream.SeekTo(stream.Position + 0x12);
			}
			public void Update(EndianStream stream)
			{
				stream.SeekTo(Offset + 0x0C);
				stream.WriteInt32(TagIndex);

				stream.WriteFloat(SpawnCoordinates.X);
				stream.WriteFloat(SpawnCoordinates.Y);
				stream.WriteFloat(SpawnCoordinates.Z);
				stream.WriteFloat(SpawnCoordinates.Yaw);
				stream.WriteFloat(SpawnCoordinates.Pitch);
				stream.WriteFloat(SpawnCoordinates.Roll);

				stream.SeekTo(stream.Position + 0x17);
				stream.WriteByte(Team);

				stream.SeekTo(stream.Position + 0x01);
				stream.WriteByte(RespawnTime);

				stream.SeekTo(stream.Position + 0x12);
			}

			public class Coordinates
			{
				public float X { get; set; }
				public float Y { get; set; }
				public float Z { get; set; }
				public float Roll { get; set; }
				public float Pitch { get; set; }
				public float Yaw { get; set; }
			}
		}
		public class TagEntry
		{
			public TagEntry(EndianStream stream, MapMetaData mapMetaData)
			{
				Offset = stream.Position;
				Load(stream, mapMetaData);
			}

			public List<ObjectChunk> PlacedObjects { get; set; }
			public long Offset { get; set; }
			public Int32 Ident { get; set; }
			public MapMetaData.Tag Tag { get; set; }
			public byte RunTimeMinimium { get; set; }
			public byte RunTimeMaximium { get; set; }
			public byte CountOnMap { get; set; }
			public byte DesignTimeMaximium { get; set; }
			public float Cost { get; set; }

			public void Load(EndianStream stream, MapMetaData mapMetaData)
			{
				stream.SeekTo(Offset);
				Ident = stream.ReadInt32();
				Tag = mapMetaData.GetTag(Ident);
				RunTimeMinimium = stream.ReadByte();
				RunTimeMaximium = stream.ReadByte();
				CountOnMap = stream.ReadByte();
				DesignTimeMaximium = stream.ReadByte();
				Cost = stream.ReadFloat();
			}
			public void Update(EndianStream stream)
			{
				stream.SeekTo(Offset);
				stream.WriteInt32(Ident);
				stream.WriteByte(RunTimeMinimium);
				stream.WriteByte(RunTimeMaximium);
				stream.WriteByte(CountOnMap);
				stream.WriteByte(DesignTimeMaximium);
				stream.WriteFloat(Cost);
			}
		}
	}
}
