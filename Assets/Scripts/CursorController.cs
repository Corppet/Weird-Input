using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [HideInInspector] public static CursorController Instance { get; private set; }

    [HideInInspector] public Rigidbody2D selectedRB;

    private Vector3 cursorPosition;
    private Vector3 offset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        selectedRB = null;
    }

    private void Update()
    {
        if (!GameManager.instance.isInPlay)
            return;

        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch(GameManager.instance.currentControls)
        {
            case ControlScheme.Normal:
                DragAndDrop();
                break;
            case ControlScheme.Switched:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch(GameManager.instance.currentControls)
        {
            case ControlScheme.Normal:
                MoveSelectedObject();
                break;
            case ControlScheme.Switched:
                break;
        }
    }

    private void MoveSelectedObject()
    {
        if (selectedRB)
        {
            selectedRB.MovePosition(cursorPosition + offset);
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
                selectedRB = targetObject.GetComponent<Rigidbody2D>();
                offset = selectedRB.transform.position - cursorPosition;
            }
        }
        // release the selected object
        else if (selectedRB && Input.GetMouseButtonUp(0))
        {
            selectedRB.velocity = Vector2.zero;
            selectedRB = null;
        }
    }
}
