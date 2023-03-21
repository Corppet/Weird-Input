using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [HideInInspector] 
    public bool IsGoal
    {
        set
        {
            IsGoal = value;
            if (value)
            {
                spriteRenderer.color = isGoalColor;
            }
            else
            {
                spriteRenderer.color = notGoalColor;
            }
        }

        get { return IsGoal; }
    }

    [SerializeField] private Color notGoalColor = Color.clear;

    private SpriteRenderer spriteRenderer; 
    private Color isGoalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsGoal)
        {
            GameManager.Instance.OnCompleteCourse.Invoke();
        }
    }
}
