using UnityEngine;

public static class RNG
{
    static int value = 0;
    private static int Next()
    {
        value = value * 0x08088405 + 1;
        return value;
    }

    public static int Range(int a, int b)
    {
        return Mathf.Abs(a + Next() % (b - a));
    }

    public static void Init(int seed)
    {
        value = seed;
    }
}
