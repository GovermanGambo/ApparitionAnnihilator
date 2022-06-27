using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class CircularForceField : ForceField
{
    protected override void ApplyForce(Rigidbody2D rigidbody)
    {
        var directionVector = (rigidbody.transform.position - transform.position).normalized;
        rigidbody.AddForce(directionVector * force);
    }
}
