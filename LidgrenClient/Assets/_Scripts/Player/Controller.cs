using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            var clickedPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            StaticManager.Client.SendPosition(clickedPosition.x, clickedPosition.y);
        }
    }
}
