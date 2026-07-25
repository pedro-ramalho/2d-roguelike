using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{    
    public void Launch(Vector3 startWorldPos, Vector3 endWorldPos, float duration, Action onArrive)
    {
        StartCoroutine(LaunchCoroutine(startWorldPos, endWorldPos, duration, onArrive));
    }

    public IEnumerator LaunchCoroutine(Vector3 startWorldPos, Vector3 endWorldPos, float duration, Action onArrive)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startWorldPos, endWorldPos, t);

            yield return null;    
        }

        transform.position = endWorldPos;
        onArrive?.Invoke();

        Destroy(gameObject);
    }
}
