
    using System;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

    public static class Game
    {
        private const int MaxLevels = 13;
        public static int LevelCounter { get; set; } = 0;

        public static void StartGame(MonoBehaviour sender)
        {
            FadeOut();
            sender.StartCoroutine(co_LoadScene(() =>
            {
                SceneManager.UnloadSceneAsync("MainMenu");
            }));
        }
        
        public static void EndScene(MonoBehaviour sender)
        {
            LevelCounter++;

            if (LevelCounter == MaxLevels)
            {
                EndGame(sender);
                return;
            }
            
            FadeOut();
            var endingText = GameObject.Find("endingText");
            var textMesh = endingText.GetComponent<TextMeshProUGUI>();
            textMesh.enabled = true;
            textMesh.text = GetEndingMessage();
            endingText.GetComponent<Animator>().enabled = true;
            
            sender.StartCoroutine(co_LoadScene(() =>
            {
                if (LevelCounter > 0)
                {
                    SceneManager.UnloadSceneAsync($"Level{LevelCounter - 1}");
                }
            }));
        }

        private static void EndGame(MonoBehaviour sender)
        {
            var angel = Object.FindObjectOfType<Angel>(true);
            angel.PlayEndAnimation();
            sender.StartCoroutine(co_Wait(5.0f, () =>
            {
                FadeOut();
                var endingText = GameObject.Find("endingText");
                var textMesh = endingText.GetComponent<TextMeshProUGUI>();
                textMesh.enabled = true;
                textMesh.text = "All apparitions (and loneliness) annihilated. Congratulations!";
                var animator = endingText.GetComponent<Animator>();
                animator.enabled = true;

                sender.StartCoroutine(co_Wait(2.0f, () =>
                {
                    animator.enabled = false;
                    sender.StartCoroutine(co_Wait(3.0f, Application.Quit));
                }));
            }));
        }

        private static IEnumerator co_Wait(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        public static void RestartLevel(MonoBehaviour sender)
        {
            FadeOut();
            var endingText = GameObject.Find("endingText");
            var textMesh = endingText.GetComponent<TextMeshProUGUI>();
            textMesh.text = GetDeathMessage();
            textMesh.enabled = true;
            endingText.GetComponent<Animator>().enabled = true;

            var scene = SceneManager.GetSceneByName($"Level{LevelCounter}");
            sender.StartCoroutine(co_LoadScene(() =>
            {
                if (scene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(scene.name);
                }
            }));
        }

        private static IEnumerator co_LoadScene(Action callback)
        {
            yield return new WaitForSeconds(2.5f);
            SceneManager.LoadScene($"Level{LevelCounter}", LoadSceneMode.Additive);
            
            callback?.Invoke();
        }

        public static void FadeIn()
        {
            var blackFade = GameObject.Find("blackFade")?.GetComponent<Animator>();
            if (blackFade == null) return;
            
            blackFade.Play("blackFade_In");
        }

        public static void FadeOut()
        {
            var blackFade = GameObject.Find("blackFade")?.GetComponent<Animator>();
            if (blackFade == null) return;
            
            blackFade.Play("blackFade_Out");
        }

        private static string GetDeathMessage()
        {
            string[] messages = new []
            {
                "You can't escape unity...",
                "There are dozens of us...",
                "My finger slipped..."
            };

            var rng = new System.Random();
            return messages[rng.Next(messages.Length)];
        }
        
        private static string GetEndingMessage()
        {
            string[] messages = new []
            {
                "Will you be my plus one?",
                "My parents can't wait to meet you...",
                "I love us...",
                "A life of unity and discounts awaits you...",
                "You have nice skin, give me your skin...",
                "Is that your real hair color?",
                "Our thoughts and phobias will become one...",
            };

            var rng = new System.Random();
            return messages[rng.Next(messages.Length)];
        }
    }
