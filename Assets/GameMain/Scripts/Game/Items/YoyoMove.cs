using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoMove : MonoBehaviour
{
    public float xLeftOffset = -2f;
    public float xRightOffset = 2f;

    private Vector3 leftPos = Vector3.zero;
    private Vector3 rightPos = Vector3.zero;
    public float moveSpeed = 2f;
    private bool isRight = false;

    private void Start( )
    {
        leftPos = transform.position + Vector3.right * xLeftOffset;
        rightPos = transform.position + Vector3.right * xRightOffset;
    }

    private void Update( )
    {
        Vector3 target = isRight ? leftPos : rightPos;
        Vector3 dir = target - transform.position;

        if ( dir.magnitude <= 1e-2 )
        {
            isRight = !isRight;
        }
        transform.Translate( dir.normalized * Time.deltaTime * moveSpeed );
    }
}
