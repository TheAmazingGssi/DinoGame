using System;
using UnityEngine;

public class SpencerHeadLocationTracking : MonoBehaviour
{
    [SerializeField] Transform headTransform;
    [SerializeField] GameObject NeckEdge;

    private void Update()
    {
        headTransform = NeckEdge.transform;
    }
}
