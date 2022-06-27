using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public abstract class ForceField : MonoBehaviour
{
    [SerializeField] protected bool alwaysOn;
    //[SerializeField] protected bool negative;
    [SerializeField] protected float force;
    [SerializeField] protected Sprite alwaysOnSprite;

    private readonly List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>();
    private static readonly int IsOpen = Animator.StringToHash("isOpen");

    private void Start()
    {
        IsEnabled = true;

        Light2D light2D = GetComponentInChildren<Light2D>();
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        /*if (negative) 
        {
            force *= -1;
            var main = particleSystem.main;
            main.startRotation = new ParticleSystem.MinMaxCurve(Mathf.PI + main.startRotation.constant);;
        }*/

        if (alwaysOn)
        {
            light2D.color = new Color(0, 0, 150f/255f);
            particleSystem.textureSheetAnimation.SetSprite(0, alwaysOnSprite);
        }

        // Also scale light size on circle light
        if (light2D.lightType == Light2D.LightType.Point)
        {
            light2D.pointLightInnerRadius = transform.localScale.x * 2.5f;
            light2D.pointLightOuterRadius = transform.localScale.x * 5.0f;
        }
    }

    public bool IsEnabled
    {
        get => gameObject.activeSelf;
        set
        {

            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool(IsOpen, value);
            }

            gameObject.SetActive(value);
        }
    }

    private void FixedUpdate()
    {
        foreach (var rigidbody in rigidbodies)
        {
            var playerController = rigidbody.GetComponent<PlayerController>();
            if (playerController == null)
            {
                continue;
            }
        
            ApplyForce(rigidbody);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        var rigidbody = other.GetComponent<Rigidbody2D>();
        if (rigidbody == null || !(other.isTrigger ^ alwaysOn)) return;
        
        rigidbodies.Add(rigidbody);
        other.GetComponent<PlayerController>().isInForceField++;
    }
    
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        var rigidbody = other.GetComponent<Rigidbody2D>();
        if (rigidbody == null || !(other.isTrigger ^ alwaysOn)) return;
        
        rigidbodies.Remove(rigidbody);
        other.GetComponent<PlayerController>().isInForceField--;
    }

    protected abstract void ApplyForce(Rigidbody2D rigidbody);
}
