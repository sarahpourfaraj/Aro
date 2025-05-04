using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public bool facingLeft = true;
    public float moveSpeed = 2f;
    public Transform Checkpoint;
    public float distance = 1f;
    public LayerMask layerMask;

    void Start()
    {

    }
    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
        RaycastHit2D hit = Physics2D.Raycast(Checkpoint.position, Vector2.down, distance, layerMask);

        if (hit == false && facingLeft)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            facingLeft = false; 
        }
        else if (hit == false && facingLeft == false){
            transform.eulerAngles = new Vector3(0, 0, 0);
            facingLeft = true; 
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (Checkpoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Checkpoint.position, Vector2.down * distance);
    }
}
