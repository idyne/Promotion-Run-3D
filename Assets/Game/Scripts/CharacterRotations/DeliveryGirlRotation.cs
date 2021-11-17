using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryGirlRotation : MonoBehaviour, ICharacterRotation
{
    public void RotateCharacter(Vector3 velocity)
    {
        float angle = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.z,
            velocity.x < 0 ? 25 :
            (velocity.x > 0 ? -25 
            : 0),
            180 * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
    }

}
