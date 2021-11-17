using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretaryRotation : MonoBehaviour, ICharacterRotation
{
    public void RotateCharacter(Vector3 velocity)
    {
        float angle = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.y, velocity.x > 0 ? Mathf.Clamp(velocity.normalized.x * 15, 10, 15) : (velocity.x < 0 ? Mathf.Clamp(velocity.normalized.x * 15, -10, -15) : 0), 60 * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * angle);
    }

}
