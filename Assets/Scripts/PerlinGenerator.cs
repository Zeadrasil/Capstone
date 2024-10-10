using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinGenerator
{
    //Taken from Wikipedia (https://en.wikipedia.org/wiki/Perlin_noise#Implementation) and then modified to work with C# instead of C
    private static Vector2 getGradient(int x, int y, uint seedA, uint seedB, uint seedC)
    {
        const int w = 8 * sizeof(uint);
        const int s = w / 2;
        uint a = (uint)x, b = (uint)y;
        a *= seedA; b ^= a << s | a >> w - s;
        b *= seedB; a ^= b << s | b >> w - s;
        a *= seedC;
        float random = a * (float)(3.14159265 / ~(~uint.MinValue >> 1));
        Vector2 v;
        v.x = Mathf.Cos(random); 
        v.y = Mathf.Sin(random);
        return v;
    }

    private static float dotGridGradient(int ix, int iy, float x, float y, uint seedA, uint seedB, uint seedC)
    {
        Vector2 gradient = getGradient(ix, iy, seedA, seedB, seedC);

        float dx = x - (float)ix;
        float dy = y - (float)iy;

        return (dx * gradient.x + dy * gradient.y);
    }

    public static float Noise(float x, float y, uint seedA = 3284157443, uint seedB = 1911520717, uint seedC = 2048419325)
    {
        int x0 = (int)Mathf.Floor(x);
        int x1 = x0 + 1;
        int y0 = (int)Mathf.Floor(y);
        int y1 = y0 + 1;

        float sx = x - (float)x0;
        float sy = y - (float)y0;

        float n0, n1, ix0, ix1, value;

        n0 = dotGridGradient(x0, y0, x, y, seedA, seedB, seedC);
        n1 = dotGridGradient(x1, y0, x, y, seedA, seedB, seedC);
        ix0 = Mathf.Lerp(n0, n1, sx);

        n0 = dotGridGradient(x0, y1, x, y, seedA, seedB, seedC);
        n1 = dotGridGradient(x1, y1, x, y, seedA, seedB, seedC);
        ix1 = Mathf.Lerp(n0, n1, sx);

        value = Mathf.Lerp(ix0, ix1, sy);
        return value * 0.5f + 0.5f;
    }
}
