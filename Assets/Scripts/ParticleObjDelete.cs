using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObjDelete : MonoBehaviour
{
    ParticleSystem particle;
    
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(DeleteCoroutine());
    }

    IEnumerator DeleteCoroutine()
    {
        yield return new WaitWhile(() => particle.IsAlive(true));
        Destroy(this.gameObject);
    }
}
