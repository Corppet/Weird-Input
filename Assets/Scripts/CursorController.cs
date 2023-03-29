using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [HideInInspector] public static CursorController Instance { get; private set; }

    [HideInInspector] public Rigidbody2D SelectedRB;

    private Vector3 cursorPosition;
    private Vector3 offset;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SelectedRB = null;
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (!gm.IsInPlay || gm.IsCourseComplete)
            return;

        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (GameManager.Instance.CurrentControls)
        {
            case ControlScheme.Normal:
                DragAndDrop();
                break;
            case ControlScheme.Switched:
                SelectedRB = null;
                break;
        }
    }

    private void FixedUpdate()
    {
        GameManager gm = GameManager.Instance;

        if (!gm.IsInPlay || gm.IsCourseComplete)
            return;

        switch (GameManager.Instance.CurrentControls)
        {
            case ControlScheme.Normal:
                DragBall();
                break;
            case ControlScheme.Switched:
                break;
        }
    }

    private void DragBall()
    {
        if (SelectedRB)
        {
            SelectedRB.MovePosition(cursorPosition + offset);
        }
    }

    private void DragAndDrop()
    {
        // select and "pick up" an object with the cursor
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D targetObject = Physics2D.OverlapPoint(cursorPosition);
            if (targetObject && targetObject.CompareTag("Player"))
            {
                SelectedRB = targetObject.GetComponent<Rigidbody2D>();
                offset = SelectedRB.transform.position - cursorPosition;
            }
        }
        // release the selected object
        else if (SelectedRB && Input.GetMouseButtonUp(0))
        {
            SelectedRB.velocity = Vector2.zero;
            SelectedRB = null;
        }
    }
}
