using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(BoxCollider2D))]
public class DirectionalForceField : ForceField
{

    private BoxCollider2D boxCollider2D;
    private Vector2 directionVector;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        directionVector = (Vector2)(transform.localRotation * Vector3.up);
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3)directionVector * 2);
    }

    protected override void ApplyForce(Rigidbody2D rigidbody)
    {
        // Find proportion for collision that you have traversed
        Vector2 deltaPosition = (Vector2)(rigidbody.transform.position - transform.position);
        float forceMultiplier = Vector3.Dot(deltaPosition, directionVector) / transform.localScale.y;
        if (forceMultiplier < 0.0f) forceMultiplier = 0.0f;
        forceMultiplier = 2.0f - forceMultiplier;

        rigidbody.AddForce(directionVector * force * forceMultiplier);
    }
}
