using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRotation : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _ForwardCheck = new Vector3(0,0,2);
    private Vector3 _PortCheck = new Vector3(-1, 0, -2);
    private Vector3 _StarboardCheck = new Vector3(1, 0, -2);
    [SerializeField] WaterGetHeight waterHeight;
    [HideInInspector] public Vector3 bobDisplacement = Vector3.zero;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion normalCurrRot = transform.rotation.normalized;
        Vector3 foreward = transform.position + (normalCurrRot * _ForwardCheck);
        //Debug.DrawLine(transform.position, foreward, Color.red);

        Vector3 port = transform.position + (normalCurrRot * _PortCheck);
        //Debug.DrawLine(transform.position, port, Color.yellow);

        Vector3 starboard = transform.position + (normalCurrRot * _StarboardCheck);
        //Debug.DrawLine(transform.position, starboard, Color.blue);

        Vector3 stern = Vector3.Lerp(port, starboard, 0.5f);
        //Debug.DrawLine(transform.position, stern, Color.green);
        float fBHeightDiff = waterHeight.getWaterHeight(foreward.x, foreward.z) - waterHeight.getWaterHeight(stern.x, stern.z);
        stern.y = fBHeightDiff;
        foreward.y = 0;
        float fBHyp = Vector3.Distance(foreward, stern);
        float xAngle = Mathf.Acos(fBHeightDiff / fBHyp) * Mathf.Rad2Deg;
        xAngle -= 90;


        float pSHeightDiff = waterHeight.getWaterHeight(port.x, port.z) - waterHeight.getWaterHeight(starboard.x, starboard.z);
        starboard.y = pSHeightDiff;
        port.y = 0;
        float pSHyp = Vector3.Distance(foreward, stern);
        float zAngle = Mathf.Acos(pSHeightDiff / pSHyp) * Mathf.Rad2Deg;
        zAngle -= 90;



        //transform.localRotation
        bobDisplacement  = new Vector3((xAngle / 2.5f), 0, (zAngle / 1.5f));
    }
}
