using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour {
        private Collider2D diamondCollider;

    void Start()
    {
        diamondCollider = GetComponent<Collider2D>();
        
        if (diamondCollider == null)
        {
            diamondCollider = gameObject.AddComponent<BoxCollider2D>();
            diamondCollider.isTrigger = true;
        }
        else
        {
            diamondCollider.isTrigger = true;
        }
    }
}
