namespace Lyt.AdobeSwatchExchange.Structures;

public class AseDocument
{
    public AseDocument()
    {
        this.Groups = [];
        this.Colors = [];
    }

    public ColorEntryCollection Colors { get; set; }

    public ColorGroupCollection Groups { get; set; }

    public void Load(string fileName)
    {
        using Stream stream = File.OpenRead(fileName);
        this.Load(stream);
    }

    public void Load(Stream stream)
    {
        ColorGroupCollection groups = [];
        ColorEntryCollection globalColors = [];
        Stack<ColorEntryCollection> colors = new ();

        // add the global collection to the bottom of the stack to handle color blocks outside of a group
        colors.Push(globalColors);
        this.ReadAndValidateVersion(stream);
        int blockCount = stream.ReadUInt32BigEndian();
        for (int i = 0; i < blockCount; i++)
        {
            this.ReadBlock(stream, groups, colors);
        }

        this.Groups = groups;
        this.Colors = globalColors;
    }

    public void Save(string fileName)
    {
        using Stream stream = File.Create(fileName);
        this.Save(stream);
    }

    public void Save(Stream stream)
    {
        this.WriteVersionHeader(stream);
        this.WriteBlocks(stream);
    }

    // Should be method in Block
    private int GetBlockLength(Block block)
    {
        // name data (2 bytes per character + null terminator, plus 2 bytes to describe that first number )
        int blockLength = 2 + (((block.Name ?? string.Empty).Length + 1) * 2);
        if (block.ExtraData != null)
        {
            blockLength += block.ExtraData.Length; // data we can't process but keep anyway
        }

        return blockLength;
    }

    // Should be method in ColorEntry 
    private int GetBlockLength(ColorEntry block)
    {
        int blockLength = this.GetBlockLength((Block)block);
        blockLength += 6; // 4 bytes for the color space and 2 bytes for the color type

        // TODO: Include support for other color spaces
        blockLength += 12; // length of RGB data (3 * 4 bytes)
        return blockLength;
    }

    private void ReadAndValidateVersion(Stream stream)
    {
        // get the signature (4 ascii characters)
        string signature = stream.ReadString(4);
        if (signature != "ASEF")
        {
            throw new InvalidDataException("Invalid signature in file.");
        }

        // read the version
        int majorVersion = stream.ReadUInt16BigEndian();
        int minorVersion = stream.ReadUInt16BigEndian();
        if (majorVersion != 1 && minorVersion != 0)
        {
            // Need to throw ??? 
            // throw new InvalidDataException("Invalid version information.");
        }
    }

    private void ReadBlock(Stream stream, ColorGroupCollection groups, Stack<ColorEntryCollection> colorStack)
    {
        BlockType blockType = (BlockType)stream.ReadUInt16BigEndian();
        int blockLength = stream.ReadUInt32BigEndian();

        // store the current position of the stream, so we can calculate the offset
        // from bytes read to the block length in order to skip the bits we didn't
        // read, support or know what they are
        int offset = (int)stream.Position;

        // process the actual block
        Block? block;
        switch (blockType)
        {
            case BlockType.Color:
                block = this.ReadColorBlock(stream, colorStack);
                break;
            case BlockType.GroupStart:
                block = this.ReadGroupBlock(stream, groups, colorStack);
                break;
            case BlockType.GroupEnd:
                block = null;
                colorStack.Pop();
                break;
            default:
                throw new InvalidDataException($"Unsupported block type '{blockType}'.");
        }

        // load in any custom data and attach it to the
        // current block (if available) as raw byte data
        int dataLength = blockLength - (int)(stream.Position - offset);
        if (dataLength > 0)
        {
            byte[] extraData;

            extraData = new byte[dataLength];
            stream.ReadExactly(extraData, 0, dataLength);
            if (block != null)
            {
                block.ExtraData = extraData;
            }
        }
    }

    private Block ReadColorBlock(Stream stream, Stack<ColorEntryCollection> colorStack)
    {
        ColorEntry block;
        string colorMode;
        byte r;
        byte g;
        byte b;
        ColorType colorType;
        string name;
        ColorEntryCollection colors;

        // get the name of the color
        // this is stored as a null terminated string
        // with the length of the byte data stored before
        // the string data in a 16bit int
        name = stream.ReadStringBigEndian();

        // get the mode of the color, which is stored
        // as four ASCII characters
        colorMode = stream.ReadString(4);

        // read the color data
        // how much data we need to read depends on the
        // color mode we previously read
        switch (colorMode)
        {
            case "RGB ":
                // RGB is comprised of three floating point values ranging from 0-1.0
                float value1;
                float value2;
                float value3;
                value1 = stream.ReadSingleBigEndian();
                value2 = stream.ReadSingleBigEndian();
                value3 = stream.ReadSingleBigEndian();
                r = (byte)Convert.ToInt32(value1 * 255);
                g = (byte)Convert.ToInt32(value2 * 255);
                b = (byte)Convert.ToInt32(value3 * 255);
                break;
            case "CMYK":
                // CMYK is comprised of four floating point values
                throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
            case "LAB ":
                // LAB is comprised of three floating point values
                throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
            case "Gray":
                // Grayscale is comprised of a single floating point value
                throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
            default:
                throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
        }

        // the final "official" piece of data is a color type
        colorType = (ColorType)stream.ReadUInt16BigEndian();

        block = new ColorEntry
        {
            R = r,
            G = g,
            B = b,
            Name = name,
            Type = colorType
        };

        colors = colorStack.Peek();
        colors.Add(block);

        return block;
    }

    private Block ReadGroupBlock(Stream stream, ColorGroupCollection groups, Stack<ColorEntryCollection> colorStack)
    {
        ColorGroup block;
        string name;

        // read the name of the group
        name = stream.ReadStringBigEndian();

        // create the group and add it to the results set
        block = new ColorGroup
        {
            Name = name
        };

        groups.Add(block);

        // add the group color collection to the stack, so when subsequent color blocks
        // are read, they will be added to the correct collection
        colorStack.Push(block.Colors);

        return block;
    }

    private void WriteBlock(Stream stream, ColorEntry block)
    {
        int blockLength;

        blockLength = this.GetBlockLength(block);

        stream.WriteBigEndian((ushort)BlockType.Color);
        stream.WriteBigEndian(blockLength);

        this.WriteNullTerminatedString(stream, block.Name);

        stream.Write("RGB ");

        stream.WriteBigEndian((float)(block.R / 255.0));
        stream.WriteBigEndian((float)(block.G / 255.0));
        stream.WriteBigEndian((float)(block.B / 255.0));

        stream.WriteBigEndian((ushort)block.Type);

        this.WriteExtraData(stream, block.ExtraData);
    }

    private void WriteBlock(Stream stream, ColorGroup block)
    {
        int blockLength;

        blockLength = this.GetBlockLength(block);

        // write the start group block
        stream.WriteBigEndian((ushort)BlockType.GroupStart);
        stream.WriteBigEndian(blockLength);
        this.WriteNullTerminatedString(stream, block.Name);
        this.WriteExtraData(stream, block.ExtraData);

        // write the colors in the group
        foreach (ColorEntry color in block.Colors)
        {
            this.WriteBlock(stream, color);
        }

        // and write the end group block
        stream.WriteBigEndian((ushort)BlockType.GroupEnd);
        stream.WriteBigEndian(0); // there isn't any data, but we still need to specify that
    }

    private void WriteBlocks(Stream stream)
    {
        int blockCount;

        blockCount = (this.Groups.Count * 2) + this.Colors.Count + this.Groups.Sum(group => group.Colors.Count);

        stream.WriteBigEndian(blockCount);

        // write the global colors first
        // not sure if global colors + groups is a supported combination however
        foreach (ColorEntry color in this.Colors)
        {
            this.WriteBlock(stream, color);
        }

        // now write the groups
        foreach (ColorGroup group in this.Groups)
        {
            this.WriteBlock(stream, group);
        }
    }

    private void WriteExtraData(Stream stream, byte[] data)
    {
        if (data != null && data.Length > 0)
        {
            stream.Write(data, 0, data.Length);
        }
    }

    private void WriteNullTerminatedString(Stream stream, string? value)
    {
        value ??= string.Empty;
        stream.WriteBigEndian(value + '\0');
    }

    private void WriteVersionHeader(Stream stream)
    {
        stream.Write("ASEF");
        stream.WriteBigEndian((ushort)1);
        stream.WriteBigEndian((ushort)0);
    }
}
