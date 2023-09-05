using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 5f;
    private Vector2 nextPosition;

    private void Awake()
    {
        nextPosition = transform.position;        
    }

    private void Update()
    {
        if (playerSpeed != 0f)
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, playerSpeed * Time.deltaTime);
    }

    public void SetNextPosition(Vector3 newPosition)
    {
        nextPosition = newPosition;
    }
}
