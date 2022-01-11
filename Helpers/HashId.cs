namespace zblesk.Helpers;

/// <summary>
/// A representation of byte[] with Hash operations implemented, so it can be used as a key in hash tables.
/// </summary>
public sealed class HashId
{
    public readonly byte[] Bytes;

    public HashId(byte[] bytes)
    {
        Bytes = bytes;
    }

    public HashId(string hexRepresentation)
    {
        int length = hexRepresentation.Length;
        Bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            Bytes[i / 2] = Convert.ToByte(hexRepresentation.Substring(i, 2), 16);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is HashId hashId)
        {
            if (Bytes.Length != hashId.Bytes.Length)
            {
                return false;
            }
            for (int i = 0; i < Bytes.Length; i++)
            {
                if (Bytes[i] != hashId.Bytes[i])
                {
                    return false;
                }
            }
            return true;
        }
        return ReferenceEquals(this, obj);
    }

    /// <remarks>
    /// Ooooh, https://stackoverflow.com/a/30353296/856077
    /// If you happen to know that the byte[] arrays you're using as the key were themselves cryptographic hashes,
    /// then you can utilize this assumption to your benefit, and simply return the first 4 bytes converted to an int.
    /// </remarks>
    public override int GetHashCode()
    {
        if (Bytes.Length >= 4)
        {
            return BitConverter.ToInt32(Bytes, 0);
        }
        throw new Exception("Hash length too short");
    }

    public override string ToString() 
        => BitConverter.ToString(Bytes).Replace("-", "");
}
