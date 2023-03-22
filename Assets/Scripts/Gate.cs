using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private bool _isGoal;
    [HideInInspector] 
    public bool IsGoal
    {
        set
        {
            _isGoal = value;
            if (value)
            {
                spriteRenderer.color = isGoalColor;
            }
            else
            {
                spriteRenderer.color = notGoalColor;
            }
        }

        get { return _isGoal; }
    }

    [SerializeField] private Color notGoalColor = Color.clear;

    private SpriteRenderer spriteRenderer; 
    private Color isGoalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isGoalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsGoal)
        {
            GameManager.Instance.OnCompleteCourse.Invoke();
        }
    }
}
