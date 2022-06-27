using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class RotateForceField : ForceField
{
    protected override void ApplyForce(Rigidbody2D rigidbody)
    {
        return;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        var playerController = other.GetComponent<PlayerController>();
        if (playerController == null || !(other.isTrigger ^ alwaysOn)) return;

        playerController.SetRotation(transform.position);

        //rigidbody.gravityScale = 0.0f;
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        var playerController = other.GetComponent<PlayerController>();
        if (playerController == null || !(other.isTrigger ^ alwaysOn)) return;

        playerController.RemoveRoation();
    }
}
