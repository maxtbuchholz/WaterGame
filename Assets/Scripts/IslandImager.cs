using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class IslandImager : MonoBehaviour
{
    private static IslandImager instance;
    public static IslandImager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<IslandImager>();
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
    Camera imageCamera;
    [SerializeField] RenderTexture snapTexture;
    void Start()
    {
        imageCamera = GetComponent<Camera>();
        imageCamera.enabled = false;
    }
    public IEnumerator ImageOfIsland(Vector2 islandPos, int keyI, GameObject island)
    {
        //yield return new WaitForSeconds(0.5f);
        yield return null;
        island.layer = 16;
        //islandPos = Vector2.zero;
        transform.position = new Vector3(islandPos.x, 100, islandPos.y);
        //imageCamera.Render();
        //RenderTexture outputMap = new RenderTexture(2000, 2000, 32);
        //RenderTexture.active = outputMap;
        //Texture2D texture = new Texture2D(outputMap.width, outputMap.height, TextureFormat.ARGB32, false, false);
        //texture.ReadPixels(new Rect(0, 0, outputMap.width, outputMap.height), 0, 0);
        //texture.Apply();

        RenderTexture mRt = new RenderTexture(snapTexture.width, snapTexture.height, snapTexture.depth, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        mRt.antiAliasing = snapTexture.antiAliasing;

        var tex = new Texture2D(mRt.width, mRt.height, TextureFormat.ARGB32, false);
        imageCamera.targetTexture = mRt;
        imageCamera.Render();
        RenderTexture.active = mRt;

        tex.ReadPixels(new Rect(0, 0, mRt.width, mRt.height), 0, 0);
        tex.Apply();
        Color Temp;
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                Temp = tex.GetPixel(x, y);
                if ((Temp.r > 0.8f) &&
                    (Temp.g > 0.8f) &&
                    (Temp.b > 0.8f))
                    tex.SetPixel(x, y, Color.clear);
            }
        }
        byte[] bytes = tex.EncodeToPNG();
        if (!Directory.Exists(Application.persistentDataPath + "/IslandImages"))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(Application.persistentDataPath + "/IslandImages");
        }
        File.WriteAllBytes(Application.persistentDataPath + "/IslandImages/" + keyI.ToString() + ".png", bytes);
        Destroy(tex);
        island.layer = 11;
    }
}
