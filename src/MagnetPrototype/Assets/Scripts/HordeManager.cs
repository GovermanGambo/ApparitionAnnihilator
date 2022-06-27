using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HordeManager : MonoBehaviour
{
    [SerializeField] private float delayInSeconds;
    [SerializeField] private int additionalClones;
    [SerializeField] private Transform hordeCloneTemplate;

    private int currentSize;
    
    private PlayerController playerController;
    private List<PlayerData> playerDatas;
    private List<Transform> clones;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        clones = new List<Transform>(30);
        hordeCloneTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        currentSize = Game.LevelCounter;
        for (int i = 0; i < currentSize; i++)
        {
            CreateClone();
        }
    }

    public void HideClones()
    {
        foreach (var clone in clones)
        {
            clone.gameObject.SetActive(false);
        }
    }
    
    public void ShowClones()
    {
        foreach (var clone in clones)
        {
            clone.gameObject.SetActive(true);
        }
    }

    private void CreateClone()
    {
        var clone = Instantiate(hordeCloneTemplate, transform);
        clone.gameObject.SetActive(true);
        clones.Add(clone);
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            return;
        }
        
        for (int i = 0; i < clones.Count; i++)
        {
            StartCoroutine(UpdateClone(playerController.PlayerData, i));
        }
    }

    public void KillAll()
    {
        enabled = false;
        StopAllCoroutines();
        for (int i = 0; i < clones.Count; i++)
        {
            StartCoroutine(DestroyClone(i));
        }
    }

    private IEnumerator DestroyClone(int index)
    {
        var clone = clones[index];
        yield return new WaitForSeconds(delayInSeconds * (index + 1));
        
        var spriteRenderer = clone.GetComponentInChildren<SpriteRenderer>();
        while (spriteRenderer.color.a > 0.0f)
        {
            var color = spriteRenderer.color;
            spriteRenderer.color = new Color(color.r, color.g, color.b, color.a - 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        
        Destroy(clone.gameObject);
    }

    private IEnumerator UpdateClone(PlayerData data, int index)
    {
        yield return new WaitForSeconds(delayInSeconds * (index + 1));
        var cloneTransform = clones[index].transform;
        cloneTransform.position = data.Position;
        cloneTransform.localScale = data.Scale;
    }
}

public readonly struct PlayerData
{
    public Vector3 Position { get; }
    public Vector3 Scale { get; }

    public PlayerData(Vector3 position, Vector3 scale)
    {
        Position = position;
        Scale = scale;
    }
}
