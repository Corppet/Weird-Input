using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [HideInInspector] public bool isGoal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isGoal)
        {
            Debug.Log("Goal reached");
            GameManager.instance.onCompleteCourse.Invoke();
        }
    }
}
