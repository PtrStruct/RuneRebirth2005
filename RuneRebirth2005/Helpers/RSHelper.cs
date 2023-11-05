namespace RuneRebirth2005.Helpers;

public static class RSHelper
{
    public static long ToLong(this string s)
    {
        var l = 0L;
        for (var i = 0; i < s.Length && i < 12; i++)
        {
            var c = s[i];
            l *= 37L;
            if (c >= 'A' && c <= 'Z')
                l += 1 + c - 65;
            else if (c >= 'a' && c <= 'z')
                l += 1 + c - 97;
            else if (c >= '0' && c <= '9')
                l += 27 + c - 48;
        }

        while (l % 37L == 0L && l != 0L)
            l /= 37L;
        return l;
    }
    
    public static int HexToInt(byte[] data, int offset, int len)
    {
        int temp = 0;
        int i = 1000;
        for (int cntr = 0; cntr < len; cntr++)
        {
            int num = (data[offset + cntr] & 0xFF) * i;
            temp += num;
            if (i > 1)
                i = i / 1000;
        }
        return temp;
    }
}