using System.Collections;
using UnityEngine;

public class VanishAfterSeconds : MonoBehaviour
{
    [SerializeField] private float delay = .1f;

    void Start()
    {
        StartCoroutine(Vanish());
    }

    IEnumerator Vanish()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
