using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    static Vector2 seedOffset = new Vector2(10000000, 10000000);
    public static float GroundHeight(float x, float z, float lacunarity, float persistance, int octaves)
    {
        x += seedOffset.x;
        z += seedOffset.y;
        //float y = Mathf.PerlinNoise((x * 0.03f), (z * 0.03f)) * 30f;
        //y += Mathf.PerlinNoise((x * 0.005f), (z * 0.005f)) * 30f;
        //y += Mathf.PerlinNoise((x * 0.0005f), (z * 0.0005f)) * 60f;
        //y += Mathf.PerlinNoise((x * 0.09f), (z * 0.09f)) * 10f;
        //y += Mathf.PerlinNoise((x * 0.020f), (z * 0.020f)) * 10f;
        //y -= 15;
        //if (y < -10) y *= 2;
        float totalHeight = 0;
        float orginalFrequency = 0.03f;
        float originalAmplitude = 20;
        float y = 0;
        for(int o = 0; o < octaves; o++)
        {
            float frequency = Mathf.Pow(lacunarity, o);
            float amplitude = Mathf.Pow(persistance, o) * originalAmplitude;
            y += Mathf.PerlinNoise((x * orginalFrequency * frequency), (z * orginalFrequency * frequency)) * amplitude;
            totalHeight += amplitude;
        }
        y = y - (totalHeight / 2);
        y -= 3;
        if (y < 0) y -= 0.5f;
        //if ((y > 0) && (y < 2.5)) y = 2;
        return y;
    }
    public static float IslandEdgeFilter(float origHeight, int x, int z, int iSizeX, int iSizeZ)
    {
        int xM = iSizeX - x;
        int zM = iSizeZ - z;
        x = Mathf.Min(x, z, xM, zM);
        float maxDis = Mathf.Min(iSizeX, iSizeZ) / 20; 
        if (x == 0) return -100;
        if (x < maxDis) return (origHeight + (-5f * (maxDis - x))) / 2;
        else return origHeight + (0.5f * (x - maxDis));
        return origHeight;
    }
    public static float ConformToIslandShape(float height)
    {
        float lerpTop = 30;
        if ((height >= -1f) && (height < lerpTop))
        {
            //height = height / 3;
            float t = height / lerpTop;
            t = Mathf.Pow(t, 2f);
            height = Mathf.Lerp(-1f, lerpTop, t);
        }
        else if (height >= lerpTop)
        {
            float t = (height - lerpTop) / lerpTop;
            t = Mathf.Pow(t, 3f);
            height = lerpTop + Mathf.Lerp(-0.1f, lerpTop, t);
        }
        return height;
    }
    public static float IslandEdgeCircleFilter(float origHeight, int x, int z, int iSizeX, int iSizeZ)
    {
        float xOffset = z - (iSizeZ / 2);
        float zOffset = x - (iSizeX / 2);
        float XRad = (iSizeX - (iSizeX / 2f)) / 2f;
        float ZRad = (iSizeZ - (iSizeZ / 2f)) / 2f;
        float angle = Mathf.Atan2(xOffset, zOffset);

        float a = XRad;// * Mathf.Cos(angle);
        float b = ZRad;// * Mathf.Sin(angle);
        //float radii = Mathf.Pow(Mathf.Pow(xCos, 2f) + Mathf.Pow(zSin, 2f), 0.5f);
        float radii = (a * b) / Mathf.Pow((cosSqTh(angle) * Mathf.Pow(b / 1,2)) + (sinSqTh(angle) * Mathf.Pow(a / 1, 2)), 0.5f);

        float pointDist = Mathf.Pow(Mathf.Pow(xOffset, 2f) + Mathf.Pow(zOffset, 2f), 0.5f);

        if ((x == 0) || (z == 0) || (x == iSizeX) || (z == iSizeZ)) return -100;
        if (radii > pointDist) return origHeight + Mathf.Min((0.5f * (radii - pointDist)), 20);
        return (origHeight + (-2.5f * (pointDist - radii))) / 2;
        //if (pointDist > radii)
        //    return -100;
        //return origHeight;
    }
    private static float cosSqTh(float theta)
    {
        theta = Mathf.Cos(theta / 1);
        theta = Mathf.Pow(theta, 2);
        //theta += 1;
        //theta /= 2;
        return theta;
    }
    private static float sinSqTh(float theta)
    {
        theta = Mathf.Sin(theta / 1);
        theta = Mathf.Pow(theta, 2);
        //theta += 1;
        //theta /= 2;
        return theta;
    }
    public static Color c_sand = new Color(0.866f, 0.647f, 0.153f, 1); //221, 165, 80, 255
    public static Color c_fore = new Color(0, 0.529f, 0.188f, 1); //0, 135, 48, 255
    public static Color c_ligg = new Color(0.541f, 0.831f, 0.533f, 1); //138, 212, 136, 255
    public static Color c_moun = new Color(0.314f, 0.314f, 0.314f, 1); //80, 80, 80, 255
    public static Color c_peak = new Color(1, 1, 1, 1);
    public static Color GetIslandColorForHeight(float height1, float height2, float height3)
    {
        if ((height1 < 1f) || (height2 < 1f) || (height3 < 1f))
            return c_sand;
        if ((height1 < 6f) || (height2 < 6f) || (height3 < 6f))
            return c_fore;
        if ((height1 < 12f) || (height2 < 12f) || (height3 < 12f))
            return c_ligg;
        if ((height1 < 15f) || (height2 < 15f) || (height3 < 15f))
            return c_moun;
        return c_peak;
    }
    public static float FloorHeight(float x, float z)
    {
        x += seedOffset.x;
        z += seedOffset.y;
        float y = Mathf.PerlinNoise((x * 0.03f), (z * 0.03f)) * 30f;
        y += Mathf.PerlinNoise((x * 0.005f), (z * 0.005f)) * 30f;
        y += Mathf.PerlinNoise((x * 0.0005f), (z * 0.0005f)) * 60f;
        //y += Mathf.PerlinNoise((x * 0.09f), (z * 0.09f)) * 10f;
        //y += Mathf.PerlinNoise((x * 0.020f), (z * 0.020f)) * 10f;
        y -= 70;
        if (y > -1) y = -1;
        return y;
    }
}
