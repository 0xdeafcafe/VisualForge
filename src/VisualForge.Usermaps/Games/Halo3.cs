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

		private EndianStream _forgeStream;

		private Header _forgeHeader;
		private MapMetaData _forgeMapMetaData;
		private IList<ObjectChunk> _forgeObjects; 

		private const string _gameId = "4D5307E6";

		private void Initalize(Stream fileStream)
		{
			_forgeStream = new EndianStream(fileStream, Endian.BigEndian);
			if (!ValidateUsermap())
			{
				Close();
				throw new InvalidOperationException("Invalid Halo 3 Usermap!");
			}

			LoadHeader();
			LoadMapMetaData();
			LoadObjectChunks();
		}
		private bool ValidateUsermap()
		{
			_forgeStream.SeekTo(0x138);
			return (_forgeStream.ReadAscii(0x04) == "mapv");
		}
		public void Close()
		{
			_forgeStream.Close();
		}


		private void LoadHeader()
		{
			_forgeHeader = new Header();

			_forgeStream.SeekTo(0x42);
			_forgeHeader.CreationDate =					_forgeStream.ReadInt32();
			_forgeStream.SeekTo(0x48);
			_forgeHeader.CreationVarientName =			_forgeStream.ReadUTF16(0x1F);
			_forgeStream.SeekTo(0x68);
			_forgeHeader.CreationVarientDescription =	_forgeStream.ReadAscii(0x80);
			_forgeStream.SeekTo(0xE8);
			_forgeHeader.CreationVarientAuthor =		_forgeStream.ReadAscii(0x13);

			_forgeStream.SeekTo(0x114);
			_forgeHeader.ModificationDate =				_forgeStream.ReadInt32();
			_forgeStream.SeekTo(0x150);
			_forgeHeader.VarientName =					_forgeStream.ReadUTF16(0x1F);
			_forgeStream.SeekTo(0x170);
			_forgeHeader.VarientDescription =			_forgeStream.ReadAscii(0x80);
			_forgeStream.SeekTo(0x1F0);
			_forgeHeader.VarientAuthor =				_forgeStream.ReadAscii(0x13);

			_forgeStream.SeekTo(0x228);
			_forgeHeader.MapID =						_forgeStream.ReadInt32();

			_forgeStream.SeekTo(0x246);
			_forgeHeader.SpawnedObjectCount =			_forgeStream.ReadInt16();

			_forgeStream.SeekTo(0x24C);
			_forgeHeader.WorldBoundsX =					new Header.WorldBound
															{
																Min = _forgeStream.ReadFloat(),
																Max = _forgeStream.ReadFloat()
															};
			_forgeHeader.WorldBoundsY =					new Header.WorldBound
															{
																Min = _forgeStream.ReadFloat(),
																Max = _forgeStream.ReadFloat()
															};
			_forgeHeader.WorldBoundsZ =					new Header.WorldBound
															{
																Min = _forgeStream.ReadFloat(),
																Max = _forgeStream.ReadFloat()
															};

			_forgeStream.SeekTo(0x268);
			_forgeHeader.MaximiumBudget =				_forgeStream.ReadFloat();
			_forgeHeader.CurrentBudget =				_forgeStream.ReadFloat();
		}
		private void LoadMapMetaData()
		{
			var taglist = 
				VariousFunctions.GZip.Decompress(VariousFunctions.GetTaglistFile(_gameId, _forgeHeader.MapID));

			_forgeMapMetaData = JsonConvert.DeserializeObject<MapMetaData>(VariousFunctions.ByteArrayToString(taglist, VariousFunctions.EncodingType.ASCII));
		}
		private void LoadObjectChunks()
		{
			_forgeStream.SeekTo(0x279);
			_forgeObjects = new List<ObjectChunk>();
			for(var chunk = 0; chunk < 640; chunk++)
				_forgeObjects.Add(new ObjectChunk(_forgeStream));
		}
		private void LoadTags()
		{
			
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
			public MapMetaData.Tag Tag { get; set; }

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
	}
}
