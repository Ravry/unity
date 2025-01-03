using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private Material material;
    [SerializeField] private float dissolveTime = .2f, delayTime = 1.0f;

    bool shouldDissolve = false;
    private float time = 0;

    float dissolve;
    Coroutine coroutine;

    void Update() {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (shouldDissolve)
                dissolve = Mathf.Lerp(1, 0, time / dissolveTime);
            else 
                dissolve = Mathf.Lerp(0, 1, time / dissolveTime);
            
            material.SetFloat("_DissolveAmount", dissolve);
        }

        if (dissolve == 1 && coroutine == null)
        {
            coroutine = StartCoroutine(Dissolved());
        }


        if (dissolve > 0)
            meshCollider.enabled = false;
        else {
            meshCollider.enabled = true;
        }

    }

    IEnumerator Dissolved() {
        yield return new WaitForSeconds(delayTime);
        Show();
        coroutine = null;
    }

    public void Dissolve() {
        shouldDissolve = true;
        time = dissolveTime;
    }

    public void Show() {
        shouldDissolve = false;
        time = dissolveTime;
    }
}
