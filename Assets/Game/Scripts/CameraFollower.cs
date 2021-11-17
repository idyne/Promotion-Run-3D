using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform Target = null;
    public float Speed = 15;

    private void LateUpdate()
    {
        Follow();
    }

    private void Follow()
    {
        transform.position = Target.position;
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.y = Target.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
