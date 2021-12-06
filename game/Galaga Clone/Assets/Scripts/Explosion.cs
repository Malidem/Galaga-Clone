using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [HideInInspector]
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Die());
    }

    void Update()
    {
        if (parent != null)
        {
            transform.position = parent.transform.position;
        }
    }

    public IEnumerator Die()
    {
        yield return new WaitForSeconds(0.6F);
        Destroy(gameObject);
    }
}
