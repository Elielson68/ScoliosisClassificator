using UnityEngine;
using UnityEngine.UIElements;

public class ImageManipulation : MonoBehaviour
{
    public static System.Action OnEditImageActive;
    public float DistanciaMatriz;
    public float Zoom;
    public UnityEngine.UI.RawImage imagem;
    public Vector3 OneTouchPosition;
    public bool MovingImage;
    public bool Zooming;
    public static bool DisableImageManipulation {get; set;} = true;
    private RadioButton _editModeButton;

    private void OnEnable()
    {
        _editModeButton = FindObjectOfType<UIDocument>().rootVisualElement.Q<RadioButton>("edit-mode");
        _editModeButton.RegisterCallback<ClickEvent>(EditImageAction);
        DrawLinesController.OnDrawModeActive += () => DisableImageManipulation = true;
    }

    private void OnDisable()
    {
        _editModeButton.UnregisterCallback<ClickEvent>(EditImageAction);
        DrawLinesController.OnDrawModeActive -= () => DisableImageManipulation = true;
        DisableImageManipulation = true;
    }

    public void ShowEditModeButton()
    {
        _editModeButton.style.display = DisplayStyle.Flex;
    }

    public void HideEditModeButton()
    {
        _editModeButton.style.display = DisplayStyle.None;
    }

    private void EditImageAction(ClickEvent evt)
    {
        OnEditImageActive?.Invoke();
        DisableImageManipulation = false;
    }

    private void Update()
    {
        if(DisableImageManipulation) return;

        if (Input.touchCount > 1)
        {
            ZoomManipulation();
        } 
        if(Input.touchCount == 1 && Zooming is false)
        {
            MoveManipulation();
        }
        if(Input.touchCount == 0)
        {
            MovingImage = false;
            Zooming = false;
        }  
    }

    private void ZoomManipulation()
    {
        Zooming = true;
        var pos_touch_1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        var pos_touch_2 = Camera.main.ScreenToWorldPoint(Input.GetTouch(1).position);

        if(Input.GetTouch(1).phase == TouchPhase.Began)
            DistanciaMatriz = Vector3.Distance(pos_touch_1, pos_touch_2);
        else if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved && imagem.transform.localScale.x > 0)
        {
            Zoom = Vector3.Distance(pos_touch_1, pos_touch_2) - DistanciaMatriz;
            DistanciaMatriz = Vector3.Distance(pos_touch_1, pos_touch_2);
            imagem.transform.localScale += ((Vector3.one * Zoom)/10);
        }

        if(imagem.transform.localScale.x < 0)
            imagem.transform.localScale = Vector3.one/10;

        MovingImage = false;
    }

    private void MoveManipulation()
    {
        var pos_touch_1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        pos_touch_1.z = 91;
        float distance = 0;

        if(Input.GetTouch(0).phase is TouchPhase.Began || Input.GetTouch(0).phase is TouchPhase.Ended)
            OneTouchPosition = pos_touch_1;
        else
            distance = Vector3.Distance(OneTouchPosition, pos_touch_1);

        if(distance > 1)
            MovingImage = true;

        if(MovingImage)
            imagem.transform.position = pos_touch_1;
    }
}
