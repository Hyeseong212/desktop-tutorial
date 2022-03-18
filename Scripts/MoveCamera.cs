using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform target;
    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -100f);
    }
}
