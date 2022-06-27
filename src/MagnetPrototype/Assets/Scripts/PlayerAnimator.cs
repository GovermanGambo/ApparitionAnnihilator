using System;
using System.Collections;
using UnityEngine;


    [RequireComponent(typeof(PlayerController), typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private float maxAnimationSpeed;
        [SerializeField] private bool playOpeningAnimation = true;
        
        private PlayerController playerController;
        private Animator animator;
        private static readonly int a_XSpeed = Animator.StringToHash("xSpeed");
        private static readonly int a_AnimSpeed = Animator.StringToHash("animSpeed");
        private static readonly int a_AnimationDirection = Animator.StringToHash("animationDirection");

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (playOpeningAnimation)
            {
                StartCoroutine(co_PlayOpeningAnimation());
            }
            
            playerController.OnPlayerDeath += OnPlayerDeath;
        }

        private IEnumerator co_PlayOpeningAnimation()
        {
            AudioManager.Instance.PlaySound("spawn");
            var hordeManager = FindObjectOfType<HordeManager>(true);
            if (hordeManager != null) hordeManager.HideClones();
            
            playerController.PlayerState = PlayerState.Blocked;
            
            var sprite = transform.Find("sprite").GetComponent<SpriteRenderer>();
            sprite.enabled = false;
            yield return new WaitForSeconds(1.0f);
            sprite.enabled = true;
            
            animator.Play("Player_Spawn");
            yield return new WaitForSeconds(1.0f);
            
            playerController.PlayerState = PlayerState.Normal;
            
            if (hordeManager != null) hordeManager.ShowClones();
        }

        private void OnPlayerDeath(object sender, EventArgs e)
        {
            animator.Play("Player_DieRight");
        }

        private void Update()
        {
            var animSpeed = playerController.HorizontalSpeed / 2;
            animSpeed = animSpeed < 0.0001f ? 1.0f : animSpeed;
            animator.SetFloat(a_AnimSpeed, animSpeed);
            animator.SetFloat(a_XSpeed, playerController.HorizontalSpeed);
        }
    }