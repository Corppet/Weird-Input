using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager gm = GameManager.Instance;

        if (collision.CompareTag("Player") && !gm.IsCourseComplete)
        {
            gm.OnFailCourse.Invoke();
        }
    }
}
