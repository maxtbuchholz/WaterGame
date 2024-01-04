using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;

public class LoadWaterTiles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private float maxLoadRadius = 500.0f;
    private float inBetween = 100.0f;
    private LoadedTiles _instance;
    // Update is called once per frame
    private int updateRound = 0;
    void FixedUpdate()
    {
        updateRound++;
        updateRound = updateRound % 10;
        if (updateRound != 0)
            return;
        _instance = LoadedTiles.Instance;
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);  //current ship posistion
        Vector2 ner = pos / 100;         //nearestTile
        ner.x = (Mathf.Round(ner.x)) * 100;
        ner.y = (Mathf.Round(ner.y)) * 100;
        if(_instance.tileDeleter == null)
            _instance.tileDeleter = FindAnyObjectByType<TileDeleter>();
        _instance.ResetTileList();
        bool inRange = true;
        int currentRadiusIndex = 1;

        DoCoord(0,0, ner.x, ner.y);
        while (inRange)
        {
            for(int _x = currentRadiusIndex; _x >= -currentRadiusIndex; _x--)
            {
                inRange = inRange && DoCoord(_x, currentRadiusIndex, ner.x, ner.y);
                inRange = inRange && DoCoord(_x, -currentRadiusIndex, ner.x, ner.y);
            }
            for (int _y = currentRadiusIndex -1; _y > -currentRadiusIndex; _y--)
            {
                inRange = inRange && DoCoord(currentRadiusIndex, _y, ner.x, ner.y);
                inRange = inRange && DoCoord(-currentRadiusIndex, _y, ner.x, ner.y);
            }

            currentRadiusIndex++;
        }
        _instance.FinishTileLoad();
    }
    private bool DoCoord(float x, float y, float posX, float posY)
    {
        float accX = posX + (x * inBetween);
        float accY = posY + (y * inBetween);
        float d = Mathf.Sqrt(Mathf.Pow(posX - accX, 2) + Mathf.Pow(posY - accY, 2));
        if (d < maxLoadRadius)
        {
            _instance.AddToTileList(new Vector2(accX, accY));
        }
        else if ((x == 0) || (y == 0))
            return false;
        return true;
    }
}
