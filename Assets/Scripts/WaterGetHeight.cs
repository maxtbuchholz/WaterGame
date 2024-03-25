using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class WaterGetHeight : MonoBehaviour
{
    [SerializeField] Texture2D waterHeightTex;
    float waterTexScale = 1;                    //1 texture for every 10 units squared
    float actualTexSize;
    float waterMoveSpeed = 0.008f;
    float actualMoveAmount;
    float[,] heightTexValues;   //between 0 and 1
    private void Start()
    {
        actualTexSize = 100 / waterTexScale;
        actualMoveAmount = 100 * waterMoveSpeed;
        heightTexValues = new float[waterHeightTex.width, waterHeightTex.height];
        for(int x = 0; x < waterHeightTex.width; x++)
        {
            for(int y = 0; y < waterHeightTex.height; y++)
            {
                heightTexValues[x, y] = (float)(waterHeightTex.GetPixel((waterHeightTex.width - 1) - x, (waterHeightTex.height - 1) - y).r);
                //Debug.Log(heightTexValues[x, y]);
            }
        }
    }
    private static WaterGetHeight instance;

    public static WaterGetHeight Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<WaterGetHeight>();
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //void OnDrawGizmos()
    //{
    //    for (int x = -50; x < 50; x++)
    //    {
    //        for (int y = -50; y < 50; y++)
    //        {
    //            float h = getWaterHeight(x, y);
    //            Gizmos.color = new Color(h, h, h, 1);
    //            Gizmos.DrawSphere(new Vector3(x, 10, y), 1);
    //            //if (getWaterHeight(x, y) < 0.85f)
    //            //{
    //            //    Gizmos.color = Color.yellow;
    //            //    Gizmos.DrawSphere(new Vector3(x, 10, y), 1);
    //            //}
    //            //else
    //            //{
    //            //    Gizmos.color = Color.red;
    //            //    Gizmos.DrawSphere(new Vector3(x, 10, y), 1);
    //            //}
    //        }
    //    }
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(transform.position, 1);
    //}
    public float getWaterHeight(float x, float y)
    {
        if (heightTexValues == null) return 0;

        x -= (actualMoveAmount * Time.time);
        y -= (actualMoveAmount * Time.time);
        x += (actualTexSize / 2);
        y += (actualTexSize / 2);
        x %= actualTexSize;
        y %= actualTexSize;
        if (x < 0) x += actualTexSize;
        if (y < 0) y += actualTexSize;
        x /= actualTexSize;
        y /= actualTexSize;
        x *= waterHeightTex.width;
        y *= waterHeightTex.height;
        x %= waterHeightTex.width;
        y %= waterHeightTex.height;
        if (x < 0) x += waterHeightTex.width;
        if (y < 0) y += waterHeightTex.height;
        int xL = Mathf.FloorToInt(x);
        int xH = xL + 1;
        int yL = Mathf.FloorToInt(y);
        int yH = yL + 1;
        float xOff = x - xL;
        if ((xOff >= 1) || (xOff < 0))
            Debug.Log(xOff);
        float mXOff = 1 - xOff;
        float yOff = y - yL;
        if ((yOff >= 1) || (yOff < 0))
            Debug.Log(yOff);
        float mYOff = 1 - yOff;
        if (xH == waterHeightTex.width) xH = 0;
        if (yH == waterHeightTex.height) yH = 0;
        //if (xH == -1) xH = waterHeightTex.width;
        //if (yH == -1) yH = waterHeightTex.height;
        float hL = Mathf.Lerp(heightTexValues[xL, yL], heightTexValues[xH, yL], xOff);
        float hH = Mathf.Lerp(heightTexValues[xL, yH], heightTexValues[xH, yH], xOff);
        float h = Mathf.Lerp(hL, hH, yOff);

        //float h = (yOff * mXOff * heightTexValues[xL, yH]) + (mYOff * mXOff * heightTexValues[xL, yL]);
        //h += (yOff * xOff * heightTexValues[xH, yH]) + (mYOff * xOff * heightTexValues[xH, yL]);
        //    h /= 2;

        //h = (yOff * 0.5f * heightTexValues[xL, yH]) + (mYOff * 0.5f * heightTexValues[xL, yL]);
        //Debug.Log(h);
        //h = heightTexValues[xL, yL];
        //Debug.ClearDeveloperConsole();
        //Debug.Log(xL + "   " + yL);
        return h * 1;
    }
    //public static float GetWaterHeightAt(float x, float y)
    //{
    //    Texture2D trx;
    //    trx.getp
    //    x += Time.time * moveSpeed;
    //    y += Time.time * moveSpeed;
    //    //x += 0.5f;
    //    //y += 0.5f;
    //    //x *= waveWidth;
    //    //y *= waveWidth;
    //    //float n = GetAt(new Vector2(x, y), waveWidth);
    //    float n = Mathf.PerlinNoise(x, y) * waveHeight;
    //    return n * 2;
    //}
    //private static float2 unity_gradientNoise_dir(float2 p)
    //{
    //    p = p % 289;
    //    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    //    x = (34 * x + 1) * x % 289;
    //    x = ((x / 41) % 1) * 2 - 1;
    //    return (new Vector2(x - Mathf.Floor(x + 0.5f), Mathf.Abs(x) - 0.5f)).normalized;
    //}

    //float unity_gradientNoise(float2 p)
    //{
    //    float2 ip = new float2(Mathf.Floor(p.x), Mathf.Floor(p.y));
    //    float2 fp = frac(p);
    //    float d00 = dot(unity_gradientNoise_dir(ip), fp);
    //    float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    //    float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    //    float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    //    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    //    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
    //}

    //void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
    //{
    //    Out = unity_gradientNoise(UV * Scale) + 0.5;
    //}

    //private static Vector2 gradientNoisedir(Vector2 p)
    //{
    //    p = new Vector2(p.x % 289, p.y % 289);
    //    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    //    x = (34 * x + 1) * x % 289;
    //    x = ((x / 41) % 1) * 2 - 1;
    //    return (new Vector2(x - Mathf.Floor(x + 0.5f), Mathf.Abs(x) - 0.5f)).normalized;
    //}

    //private static float gradientNoise(Vector2 p)
    //{
    //    Vector2 ip = new Vector2(Mathf.Floor(p.x), Mathf.Floor(p.y));
    //    Vector2 fp = new Vector2(p.x % 1, p.y % 1);
    //    float d00 = Vector3.Dot(gradientNoisedir(ip), fp);
    //    float d01 = Vector3.Dot(gradientNoisedir(ip + new Vector2(0, 1)), fp - new Vector2(0, 1));
    //    float d10 = Vector3.Dot(gradientNoisedir(ip + new Vector2(1, 0)), fp - new Vector2(1, 0));
    //    float d11 = Vector3.Dot(gradientNoisedir(ip + new Vector2(1, 1)), fp - new Vector2(1, 1));
    //    fp = fp * fp * fp * (fp * (fp * 6 - new Vector2(15, 15)) + new Vector2(10, 10));
    //    return Mathf.Lerp(Mathf.Lerp(d00, d01, fp.y), Mathf.Lerp(d10, d11, fp.y), fp.x);
    //}

    //private static float GetAt(Vector2 UV, float Scale)
    //{
    //    //UV = new Vector2(transform.position.x, transform.position.z); //Irrelevant to the transfer, but relevant to observing it
    //    float Height = gradientNoise(UV * Scale) + 0.5f; //relevant to the transfer
    //    return Height;
    //    //transform.position = new Vector3(transform.position.x, Height * Amplitude - ((Height * Amplitude) / 2), transform.position.z); //Irrelevant to the transfer, but relevant to observing it
    //}
}
