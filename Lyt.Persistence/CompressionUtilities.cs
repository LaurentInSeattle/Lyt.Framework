namespace Lyt.Persistence;

public static class CompressionUtilities
{
    /* String Compression Note:
        UTF-8 is the default and recommended encoding for JSON. 
        While the JSON specification (RFC 7159) allows for UTF-8, UTF-16, and UTF-32, UTF-8 is preferred 
        due to its space efficiency and broad interoperability across systems and applications. 
        Many implementations specifically expect and handle UTF-8 encoded JSON.
    */

    public static byte[] CompressString(string stringData)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(stringData);
        return CompressionUtilities.Compress(byteData);
    }

    public static string DecompressToString (byte[] byteData)
    {
        byte[] decompressedByteData = CompressionUtilities.Decompress(byteData);
        return Encoding.UTF8.GetString(decompressedByteData);
    }

    public static byte[] Compress(byte[] data)
    {
        byte[] compressArray ;
        try
        {
            using MemoryStream memoryStream = new();
            using (DeflateStream deflateStream = new(memoryStream, CompressionMode.Compress))
            {
                deflateStream.Write(data, 0, data.Length);
            }

            compressArray = memoryStream.ToArray();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            throw; 
        }

        return compressArray;
    }

    public static byte[] Decompress(byte[] data)
    {
        byte[] decompressedArray;
        try
        {
            using MemoryStream decompressedStream = new();
            using (MemoryStream compressStream = new(data))
            {
                using DeflateStream deflateStream = new(compressStream, CompressionMode.Decompress);
                deflateStream.CopyTo(decompressedStream);
            }

            decompressedArray = decompressedStream.ToArray();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            throw;
        }

        return decompressedArray;
    }
}