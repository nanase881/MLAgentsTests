using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour 
{
    public enum ControlType { Swipes, Taps }
    public ControlType controlType = ControlType.Swipes;
    public bool JumpOnTap = false;
    public bool Invert;

    public static bool Jump, Swap;

    private Vector2 touchStart;
    private Player player;
    private bool swipeUp, swipeDown;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            touchStart = Input.mousePosition;

        swipeUp = Input.GetMouseButtonUp(0) && touchStart.y < Input.mousePosition.y - 10;
        swipeDown = Input.GetMouseButtonUp(0) && touchStart.y > Input.mousePosition.y + 10;

        switch(controlType)
        {
            case ControlType.Swipes:
                if(JumpOnTap)
                    Jump = Input.GetMouseButtonUp(0) && Mathf.Abs(touchStart.y - Input.mousePosition.y) < 1;
                else
                    Jump = player.IsBot() ? swipeDown : swipeUp;

                Swap = player.IsBot() ? swipeUp : swipeDown;
            break;
            case ControlType.Taps:
                 Jump = Invert ? Input.GetMouseButtonDown(0) && Input.mousePosition.x < Screen.width / 2
                               : Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width / 2;

                 Swap = Invert ? Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width / 2
                               : Input.GetMouseButtonDown(0) && Input.mousePosition.x < Screen.width / 2;
            break;
        }
    }
}
