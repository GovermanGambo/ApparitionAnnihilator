using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GlowController : MonoBehaviour
{
    private PlayerController playerController;
    private Light2D light2D;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        light2D = GetComponent<Light2D>();

        playerController.OnToggleForceField += OnToggleForceField;
    }

    private void OnToggleForceField(object sender, EventArgs e)
    {
        if (playerController.IsForceFieldActive)
        {
            light2D.color = Color.red;
        }
        else
        {
            light2D.color = Color.white;
        }
    }
}
