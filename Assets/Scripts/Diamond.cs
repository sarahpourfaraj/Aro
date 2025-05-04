using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour {
    [SerializeField] private GameObject collectedEffect;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Instantiate(collectedEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            // Add score/win logic here
        }
    }
}
