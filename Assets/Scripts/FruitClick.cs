using System.Collections;
using UnityEngine;

public class FruitClick : MonoBehaviour
{
    [SerializeField] private float eatDuration = 0.15f;
    [SerializeField] private float eatenScaleMultiplier = 0.2f;
    [SerializeField] private float spawnDuration = 0.2f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip eatSound;
    [SerializeField] private GameObject eatEffectPrefab;

    private GameManager gameManager;
    private FruitSpawner fruitSpawner;
    private Vector3 normalScale;
    private bool isEating;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        fruitSpawner = FindObjectOfType<FruitSpawner>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        normalScale = transform.localScale;
    }

    private void Start()
    {
        StartCoroutine(PlaySpawnEffect());
    }

    private void OnMouseDown()
    {
        if (isEating)
        {
            return;
        }

        if (gameManager != null && !gameManager.IsGameRunning())
        {
            return;
        }

        Debug.Log("Fruit clicked");

        // 🔊 ТЕСТОВЫЙ ЗВУК (через Play)
        if (audioSource != null)
        {
            audioSource.Play();
        }

        if (gameManager != null)
        {
            gameManager.AddScore();
        }

        StartCoroutine(PlayEatenEffect());
    }

    private IEnumerator PlaySpawnEffect()
    {
        Vector3 targetScale = normalScale;
        float randomScale = Random.Range(1.05f, 1.15f);
        Vector3 overshootScale = normalScale * randomScale;

        transform.localScale = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < spawnDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / spawnDuration);

            transform.localScale = Vector3.Lerp(Vector3.zero, overshootScale, progress);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < 0.08f)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / 0.08f);

            transform.localScale = Vector3.Lerp(overshootScale, targetScale, progress);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator PlayEatenEffect()
    {
        isEating = true;

        if (eatEffectPrefab != null)
        {
            GameObject effect = Instantiate(eatEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        Vector3 startScale = normalScale;
        Vector3 targetScale = normalScale * eatenScaleMultiplier;

        float elapsedTime = 0f;

        while (elapsedTime < eatDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / eatDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            yield return null;
        }

        transform.localScale = targetScale;

        if (fruitSpawner != null)
        {
            fruitSpawner.NotifyFruitEaten(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}