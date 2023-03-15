using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [HideInInspector] public bool IsGoal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsGoal)
        {
            Debug.Log("Goal reached");
            GameManager.Instance.OnCompleteCourse.Invoke();
        }
    }
}
