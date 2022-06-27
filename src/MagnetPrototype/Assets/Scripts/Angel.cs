using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class Angel : MonoBehaviour
{
    private Animator animator;
    private bool levelCompleted;

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        StartEndScene(other.transform);
    }

    private void StartEndScene(Transform player)
    {
        if (levelCompleted) return;

        levelCompleted = true;
        
        AudioManager.Instance.PlaySound("angelConsumed");
        
        SceneUtils.SwitchCamera("endingCamera");

        var hordeManager = FindObjectOfType<HordeManager>();
        hordeManager.KillAll();
        
        var position = transform.position;
        var animationState = player.position.x < position.x ? "Angel_Consumed" : "Angel_ConsumedLeft";

        StartCoroutine(co_MoveToPosition(player, position, () =>
        {
            animator.Play(animationState);
            player.gameObject.SetActive(false);
            
            StartCoroutine(WaitForAnimation(() =>
            {
                Game.EndScene(this);
            }));
        }));
    }
    
    public void PlayEndAnimation()
    {
        animator.Play("Player_Win");
    }

    private IEnumerator co_MoveToPosition(Transform player, Vector3 destination, Action callback)
    {
        player.position = destination;

        while (player.position != destination)
        {
            player.position = Vector3.MoveTowards(player.position, destination, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }

        callback();
    }

    private IEnumerator WaitForAnimation(Action callback)
    {
        yield return new WaitForSeconds(2.0f);
        callback();
    }
    
}
