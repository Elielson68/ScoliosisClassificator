using System;
using UnityEngine;

public class PointController : MonoBehaviour
{
    public static bool IsMouseOnPoint;
    public static bool DisableMove { get; set; }
    private static bool IsRegistered;
    private RectTransform rectTransform;
    public bool IsSacralPoint;
    private DrawLinesController retaController;

    public static Action<LineRenderer> OnDragPoint;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        retaController = FindObjectOfType<DrawLinesController>();
    }

    private void Update()
    {
        if (IsSacralPoint is false)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = DisableMove is false;
            gameObject.GetComponent<BoxCollider2D>().enabled = DisableMove is false;
        }
        if (IsSacralPoint)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private void OnEnable()
    {
        if(IsRegistered is false)
        {
            ImageManipulation.OnEditImageActive += () => DisableMove = true;
            DrawLinesController.OnDrawModeActive += () => DisableMove = false;
            IsRegistered = true;
        }
        ImageManipulation.OnZoomImage += OnImageChangeScale;
        DisableMove = IsSacralPoint;
    }

    private void OnDisable()
    {
        ImageManipulation.OnZoomImage -= OnImageChangeScale;
        ImageManipulation.OnEditImageActive -= () => DisableMove = true;
        DrawLinesController.OnDrawModeActive -= () => DisableMove = false;
        IsRegistered = false;
    }

    private void OnMouseDown()
    {
        VisualElementInteraction.IsVisualElementFocus = true;
        if (DrawLinesController.IsLineCompleted || IsSacralPoint)
            IsMouseOnPoint = true;
    }

    private void OnMouseUp()
    {
        VisualElementInteraction.IsVisualElementFocus = false;
        if (DrawLinesController.IsLineCompleted || IsSacralPoint)
            IsMouseOnPoint = false;
    }

    private void OnMouseDrag()
    {
        if (DrawLinesController.IsLineCompleted || IsSacralPoint)
        {
            IsMouseOnPoint = true;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = rectTransform.position.z;

            if (IsSacralPoint)
            {
                pos.y = 0;
                rectTransform.SetPositionAndRotation(pos, Quaternion.identity);
            }
            else
                transform.position = pos;

            OnDragPoint?.Invoke(GetComponentInParent<LineRenderer>());
        }
    }

    private void OnImageChangeScale(float scaleFactor)
    {
        transform.localScale = (Vector3.one * (IsSacralPoint ? 4 : 2)) / scaleFactor;
    }
}
