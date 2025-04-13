using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    [SerializeField] private LayerMask reportCollisionsLayer;

    public event EventHandler<Collision> OnCollisionEnterReporter;
    public event EventHandler<Collision> OnCollisionStayReporter;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsInAcceptedLayerMasks(collision.gameObject.layer))
        {
            OnCollisionEnterReporter?.Invoke(this, collision);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (IsInAcceptedLayerMasks(collision.gameObject.layer))
        {
            OnCollisionStayReporter?.Invoke(this, collision);
        }
    }

    private bool IsInAcceptedLayerMasks(int layer)
    {
        return (reportCollisionsLayer & (1 << layer)) != 0;
    }
}
