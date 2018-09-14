using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTrigger : MonoBehaviour {
    public GameObject canvas;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            canvas.SetActive(true);
        }
    }
}
