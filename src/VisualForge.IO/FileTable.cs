namespace VisualForge.IO
{
    public class TableSystem
    {
        // TODO: Uncomment and fix this

        ///// <summary>
        ///// Creates a new Instance of the TableSystem.
        ///// </summary>
        ///// <param name="direc">The directory to save the file in.</param>
        ///// <param name="filename">The name of the file to have the TableSystem saved in it, in the directory.</param>
        ///// <param name="desc">The description used in the TableSystem Global Header.</param>
        //public TableSystem(string direc, string filename, string desc)
        //{
        //    if (!Directory.Exists(direc))
        //        Directory.CreateDirectory(direc);

        //    string file = direc + filename;

        //    if (System.IO.File.Exists(file))
        //        System.IO.File.Delete(file);

        //    _desc = desc;
        //    _file = System.IO.File.Create(file);
        //}
        ///// <summary>
        ///// Creates a new Instance of the TableSystem.
        ///// </summary>
        ///// <param name="input">Input stream to write/save the TableSystem to.</param>
        //public TableSystem(Stream input)
        //{
        //    _file = input;
        //}

        ///// <summary>
        ///// Table Format
        ///// </summary>
        //public class TableHeader
        //{
        //    public string Name { get; set; }
        //    public UInt32 EntryLength { get; set; }
        //    public UInt32 Count { get; set; }
        //    public UInt32 MaxCount { get; set; }
        //    public UInt32 HeaderSize = 0x42;
        //    public int TablePadding = 0x100;

        //    public IList<byte[]> Entries { get; set; }
        //}

        ///// <summary>
        ///// Intelligent List that contains all the headers
        ///// </summary>
        //private IList<TableHeader> _tables = new List<TableHeader>();
        ///// <summary>
        ///// Stream that contains the TableSystem
        ///// </summary>
        //private Stream _file;
        ///// <summary>
        ///// The description used in the Global File Header
        ///// </summary>
        //private string _desc;

        ///// <summary>
        ///// Intelligent List that contains all the headers
        ///// </summary>
        //public IList<TableHeader> Tables
        //{
        //    get { return _tables; }
        //}
        ///// <summary>
        ///// Stream that contains the TableSystem
        ///// </summary>
        //public Stream File
        //{
        //    get { return _file; }
        //}
        ///// <summary>
        ///// The description used in the Global File Header
        ///// </summary>
        //public string Desc
        //{
        //    get { return _desc; }
        //    set { _desc = value; }
        //}

        ///// <summary>
        ///// Create a new table, and add the entires.
        ///// </summary>
        ///// <param name="name">Table Name.</param>
        ///// <param name="entrylength">Length of each entry in the table.</param>
        ///// <param name="count">Number of entries in the table.</param>
        ///// <param name="maxcount">Max number of entries in the table.</param>
        ///// <param name="entries">IList of entries</param>
        //public void CreateTable(string name, UInt32 entrylength, UInt32 count, UInt32 maxcount, IList<byte[]> entries)
        //{
        //    _tables.Add(new TableHeader()
        //    {
        //        Name = name,
        //        EntryLength = entrylength,
        //        Count = count,
        //        MaxCount = maxcount,
        //        Entries = entries
        //    });
        //}
        ///// <summary>
        ///// Create a new table, and add the entires.
        ///// </summary>
        ///// <param name="name">Table Name.</param>
        ///// <param name="entrylength">Length of each entry in the table.</param>
        ///// <param name="count">Number of entries in the table.</param>
        ///// <param name="maxcount">Max number of entries in the table.</param>
        ///// <param name="entries">Single Entry</param>
        //public void CreateTable(string name, UInt32 entrylength, UInt32 count, UInt32 maxcount, byte[] entry)
        //{
        //    IList<byte[]> entryIList = new List<byte[]>()
        //    {
        //        entry
        //    };

        //    _tables.Add(new TableHeader()
        //    {
        //        Name = name,
        //        EntryLength = entrylength,
        //        Count = count,
        //        MaxCount = maxcount,
        //        Entries = entryIList
        //    });
        //}

        ///// <summary>
        ///// Read a File into the TableSystem.
        ///// </summary>
        //public void FileRead()
        //{
        //    _tables.Clear();

        //    Reader reader = new Reader(_file);

        //    // Verify Header
        //    reader.Seek(0, SeekOrigin.Begin);
        //    if (reader.ReadByte() != 0xDE || reader.ReadByte() != 0xAD ||
        //        reader.ReadByte() != 0xBE || reader.ReadByte() != 0xEF)
        //        throw new Exception("Invalid Package\n\nThe Table is corrupt, the magic doesn't match. fool.");

        //    // Read Description
        //    reader.Seek(0x20, SeekOrigin.Begin);
        //    _desc = reader.ReadUTF16();

        //    // Read Allocation Tables
        //    reader.Seek(0x250 + 0x36, SeekOrigin.Begin);
        //    IList<Int64> allocationEntries = new List<Int64>();
        //    UInt32 allocationEntryCount = reader.ReadUInt32();
        //    reader.Seek(0x250 + 0x42, SeekOrigin.Begin);
        //    for (int i = 0; i < allocationEntryCount; i++)
        //        allocationEntries.Add(reader.ReadInt64());

        //    // Read all Tables
        //    foreach (Int64 tableAllocation in allocationEntries)
        //    {
        //        // Seek to table Start
        //        reader.Seek(tableAllocation, SeekOrigin.Begin);

        //        // Construct new Table
        //        TableHeader header = new TableHeader();
        //        header.Entries = new List<byte[]>();

        //        // Read Table Data
        //        header.Name = reader.ReadUTF16();
        //        reader.Seek(tableAllocation + 0x32, SeekOrigin.Begin);

        //        header.EntryLength = reader.ReadUInt32();
        //        header.Count = reader.ReadUInt32();
        //        header.MaxCount = reader.ReadUInt32();
        //        header.HeaderSize = reader.ReadUInt32();

        //        // Start of Table Entries
        //        Int64 tableEntriesStart = tableAllocation + header.HeaderSize;

        //        // Loop Though Entries
        //        for (int i = 0; i < header.Count; i++)
        //        {
        //            // Seek to Table Entry Start
        //            reader.Seek(tableEntriesStart + (header.EntryLength * i), SeekOrigin.Begin);

        //            // Read the entry
        //            byte[] entry = new byte[header.EntryLength];
        //            reader.ReadBlock(entry, 0, entry.Length);

        //            // Add the byte data to the table
        //            header.Entries.Add(entry);
        //        }

        //        // Add Table to IList
        //        _tables.Add(header);
        //    }
        //}
        ///// <summary>
        ///// Write the Tables to the File.
        ///// </summary>
        //public void FileWrite()
        //{
        //    // Calculate Length
        //    long strLength = 0x400;
        //    foreach (TableHeader table in _tables)
        //        strLength += (table.Count * table.EntryLength) + table.TablePadding + table.HeaderSize;

        //    // Create Stream w/ File Length
        //    _file.SetLength(strLength);
        //    Writer writer = new Writer(_file);

        //    // Construct Header
        //    #region header
        //    // Write Magic
        //    writer.Seek(0, SeekOrigin.Begin);
        //    writer.WriteBlock(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF });

        //    // Write Build Data
        //    writer.Seek(0x20, SeekOrigin.Begin);
        //    writer.WriteUTF16(_desc);
        //    #endregion

        //    // Write Tables
        //    #region tableIO
        //    writer.Seek(0x400, SeekOrigin.Begin);
        //    long tableStart = 0x400;
        //    IList<long> TableStarts = new List<long>();

        //    foreach (TableHeader table in _tables)
        //    {
        //        TableStarts.Add(tableStart);
        //        writer.Seek(tableStart, SeekOrigin.Begin);

        //        // Write Name
        //        writer.WriteUTF16(table.Name);
        //        writer.Seek(tableStart + 0x32, SeekOrigin.Begin);

        //        // Write EntryLength
        //        writer.WriteUInt32(table.EntryLength);

        //        // Write EntryCount
        //        writer.WriteUInt32(table.Count);

        //        // Write MaxCount
        //        writer.WriteUInt32(table.MaxCount);

        //        // Write HeaderSize
        //        writer.WriteUInt32(table.HeaderSize);

        //        foreach (byte[] entry in table.Entries)
        //        {
        //            // Write Table Entry
        //            writer.WriteBlock(entry);
        //        }

        //        tableStart = writer.BaseStream.Position + table.TablePadding;
        //    }
        //    #endregion

        //    // Construct Allocation Table
        //    #region allocation_table
        //    writer.Seek(0x250, SeekOrigin.Begin);
        //    tableStart = 0x250;
        //    // Write Table Header
        //    {
        //        // Write Name
        //        writer.WriteUTF16("TableAllocation");
        //        writer.Seek(tableStart + 0x32, SeekOrigin.Begin);

        //        // Write EntryLength
        //        writer.WriteUInt32(0x04);

        //        // Write EntryCount
        //        writer.WriteUInt32((UInt32)TableStarts.Count);

        //        // Write MaxCount
        //        writer.WriteUInt32(UInt32.MaxValue);

        //        // Write HeaderSize
        //        writer.WriteUInt32(0x42);
        //    }
        //    // Write Entries
        //    writer.SeekTo(0x250 + 0x42);
        //    foreach (long tbs in TableStarts)
        //        writer.WriteInt64(tbs);
        //    #endregion
        //}
    }
} 
