using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsController : MonoBehaviour
{
    [SerializeField] Slider zoomSensSlider;
    [SerializeField] Slider xPanSensSlider;
    [SerializeField] Slider yPanSensSlider;
    private static PlayerPrefsController instance;
    public static PlayerPrefsController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<PlayerPrefsController>();
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
    public void ZoomSensChanged()
    {
        if (Mathf.Abs(0.5f - zoomSensSlider.value) < 0.1f)
            zoomSensSlider.value = 0.5f;
        zoomSens = zoomSensSlider.value;
        PlayerPrefs.SetFloat("_zoomSens", zoomSens);
    }
    public void XPanSensChanged()
    {
        if (Mathf.Abs(0.5f - xPanSensSlider.value) < 0.1f)
            xPanSensSlider.value = 0.5f;
        xPanSens = xPanSensSlider.value;
        PlayerPrefs.SetFloat("_xPanSens", xPanSens);
    }
    public void YPanSensChanged()
    {
        if (Mathf.Abs(0.5f - yPanSensSlider.value) < 0.1f)
            yPanSensSlider.value = 0.5f;
        yPanSens = yPanSensSlider.value;
        PlayerPrefs.SetFloat("_yPanSens", yPanSens);
    }
    public void InitSliders()
    {
        if (!PlayerPrefs.HasKey("_zoomSens"))
            PlayerPrefs.SetFloat("_zoomSens", zoomSens);
        if (!PlayerPrefs.HasKey("_xPanSens"))
            PlayerPrefs.SetFloat("_xPanSens", xPanSens);
        if (!PlayerPrefs.HasKey("_yPanSens"))
            PlayerPrefs.SetFloat("_yPanSens", yPanSens);
        zoomSens = PlayerPrefs.GetFloat("_zoomSens");
        xPanSens = PlayerPrefs.GetFloat("_xPanSens");
        yPanSens = PlayerPrefs.GetFloat("_yPanSens");
        zoomSensSlider.value = zoomSens;
        xPanSensSlider.value = xPanSens;
        yPanSensSlider.value = yPanSens;
    }
    private float zoomSens = 0.5f;
    public float GetZoomSens()
    {
        return zoomSens;
    }
    private float xPanSens = 0.5f;
    public float GetXPanSens()
    {
        return xPanSens;
    }
    private float yPanSens = 0.5f;
    public float GetYPanSens()
    {
        return yPanSens;
    }
    private bool shouldShowTapFortHelp = false;
    private bool shouldShowSet = false;
    public bool HasShowTapFortHelp()
    {
        if (!shouldShowSet)
        {
            if (!PlayerPrefs.HasKey("_showed_tap_fort_help"))
            {
                PlayerPrefs.SetInt("_showed_tap_fort_help", 0);
                shouldShowTapFortHelp = false;
                shouldShowSet = true;
                return shouldShowTapFortHelp;
            }
            shouldShowTapFortHelp = PlayerPrefs.GetInt("_showed_tap_fort_help") == 1 ? true : false;
            shouldShowSet = true;
        }
        return shouldShowTapFortHelp;
    }
    public void SetTapFortHelp(bool set)
    {
        PlayerPrefs.SetInt("_showed_tap_fort_help", set ? 1 : 0);
        shouldShowTapFortHelp = set;
    }
}
