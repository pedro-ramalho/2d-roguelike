using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public IEnumerator Launch(Vector3 startWorldPos, Vector3 endWorldPos, float duration)
    {
        float elapsed = 0;
        float t = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            t = elapsed / duration;

            transform.position = Vector3.Lerp(startWorldPos, endWorldPos, t);

            yield return null;    
        }

        transform.position = endWorldPos;

        Destroy(gameObject);
    }
}
