using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSet : MonoBehaviour
{
    private Collectible[] collectibles;

    private void Awake()
    {
        collectibles = GetComponentsInChildren<Collectible>();
    }
    public void SetSplinePercent(float percent)
    {
        for (int i = 0; i < collectibles.Length; i++)
        {
            collectibles[i].SplinePositioner.SetPercent(percent);
        }
    }
}
