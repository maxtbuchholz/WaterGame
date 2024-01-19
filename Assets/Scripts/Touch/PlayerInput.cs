using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    private float vertical = 0;
    public void SetVertical(float vertical)
    {
        this.vertical = vertical;
    }
    private float horizontal = 0;
    public void SetHorizontal(float horizontal)
    {
        this.horizontal = horizontal;
    }
    public float GetVertical()
    {
        if((vertical == 0) && horizontal == 0)
            return Input.GetAxisRaw("Vertical");
        return vertical;
    }
    public float GetHorizontal()
    {
        if ((vertical == 0) && horizontal == 0)
            return Input.GetAxisRaw("Horizontal");
        return horizontal;
    }

}