using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObj : MonoBehaviour {

    public event System.Action<GameObject> RetrunPool;
    Rigidbody rb = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnDisable()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null) rb.velocity = Vector3.zero;
        if (RetrunPool != null) RetrunPool(gameObject);
    }
}
