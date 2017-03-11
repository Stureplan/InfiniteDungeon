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

    public static Quaternion Q90f(Vector3 axis)
    {
        Quaternion q = Quaternion.identity;
        int r = RNG.Range(0, 4);

        if      (r == 0) { q = Quaternion.AngleAxis(0.0f, axis); }
        else if (r == 1) { q = Quaternion.AngleAxis(90.0f, axis); }
        else if (r == 2) { q = Quaternion.AngleAxis(180.0f, axis); }
        else if (r == 3) { q = Quaternion.AngleAxis(270.0f, axis); }

        return q;
    }

    public static Quaternion Qf(Vector3 axis, int a)
    {
        Quaternion q = Quaternion.identity;
        int r = Random.Range(0, a);

        int rng = Random.Range(0, 2);
        if (rng == 0) { q = Quaternion.AngleAxis((float)r, axis); }
        if (rng == 1) { q = Quaternion.AngleAxis((float)r * -1, axis); }

        return q;
    }

    public static Vector3 VXZf(float min, float max)
    {
        Vector3 r;

        r.x = Random.Range(min, max);
        r.y = 0.0f;
        r.z = Random.Range(min, max);

        return r;
    }


    public static T RandomFromArray<T>(T[] arr)
    {
        int length = arr.Length;

        return arr[Range(0, length)];
    }
}
