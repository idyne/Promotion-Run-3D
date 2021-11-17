using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [SerializeField] private Transform leftDoor, rightDoor;
    [SerializeField] private Transform[] confetties;
    private Queue<Transform> confettiQueue;
    private bool isDoorOpen = false;

    public Queue<Transform> ConfettiQueue { get => confettiQueue;}

    private void Awake()
    {
        confettiQueue = new Queue<Transform>(confetties);
    }

    public void OpenDoors()
    {
        if (!isDoorOpen)
        {
            isDoorOpen = true;
            leftDoor.LeanRotateY(leftDoor.rotation.eulerAngles.y - 70, 0.5f);
            rightDoor.LeanRotateY(rightDoor.rotation.eulerAngles.y + 70, 0.5f);
        }
    }
}
