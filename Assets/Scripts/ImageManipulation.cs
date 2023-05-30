using System.Collections.Generic;
using MyUILibrary;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageManipulation : MonoBehaviour
{
    public readonly static Vector3 DefaultPositionImage = new Vector3(0, -69.9f, 10f);
    public readonly static Vector3 DefaultScaleImage = Vector3.one;
    public static System.Action OnEditImageActive;
    public static System.Action<float> OnZoomImage;
    public float DistanciaMatriz;
    public float Zoom;
    public UnityEngine.UI.RawImage imagem;
    public Vector3 OneTouchPosition;
    public bool MovingImage;
    public bool Zooming;
    public static bool DisableImageManipulation {get; set;} = true;
    private SlideToggle _editModeToggle;
    private List<ClassificationData> _classifications;
    private StateController _stateController;

    private void OnEnable()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
        _stateController = FindObjectOfType<StateController>();
        _editModeToggle = FindObjectOfType<UIDocument>().rootVisualElement.Q<SlideToggle>();
        if(_editModeToggle is not null)
        {
            _editModeToggle.RegisterCallback<ChangeEvent<bool>>(EditImageAction);
            _editModeToggle.RegisterCallback<FocusInEvent>(evt => {
                VisualElementInteraction.IsVisualElementFocus = true;
            });
            _editModeToggle.RegisterCallback<FocusOutEvent>(evt => {
                VisualElementInteraction.IsVisualElementFocus = false;
            });
        }
        
        DrawLinesController.OnDrawModeActive += () => DisableImageManipulation = true;

        StateController.OnBeforeUpdateState += UpdatePositionAndScaleImageState;
    }

    private void OnDisable()
    {
        if(_editModeToggle is not null)
        {
            _editModeToggle.UnregisterCallback<ChangeEvent<bool>>(EditImageAction);
            _editModeToggle.UnregisterCallback<FocusInEvent>(evt => {
                VisualElementInteraction.IsVisualElementFocus = true;
            });
            _editModeToggle.UnregisterCallback<FocusOutEvent>(evt => {
                VisualElementInteraction.IsVisualElementFocus = false;
            });
        }
        StateController.OnBeforeUpdateState -= UpdatePositionAndScaleImageState;
        DrawLinesController.OnDrawModeActive -= () => DisableImageManipulation = true;
        DisableImageManipulation = true;
    }

    public void SetImage(UnityEngine.UI.RawImage img)
    {
        imagem = img;
    }
    
    public void EnableManipulation()
    {
        DisableImageManipulation = false;
        VisualElementInteraction.IsVisualElementFocus = false;
    }

    private void EditImageAction(ChangeEvent<bool> evt)
    {
        if(evt.newValue)
        {
            OnEditImageActive?.Invoke();
            DisableImageManipulation = false;
        }
    }

    private void Update()
    {
        if(DisableImageManipulation || VisualElementInteraction.IsVisualElementFocus) return;

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
            _classifications[(int)StateController.CurrentState].classification.ScaleImage = imagem.transform.localScale;
            OnZoomImage?.Invoke(imagem.transform.localScale.x);
        }

        if(imagem.transform.localScale.x < 0)
        {
            imagem.transform.localScale = _classifications[(int)StateController.CurrentState].classification.ScaleImage = Vector3.one/10;
        }


        MovingImage = false;
    }

    private void MoveManipulation()
    {
        var pos_touch_1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        pos_touch_1.z = 91;
        float distance = 0;

        
        if(Input.GetTouch(0).phase is TouchPhase.Began || Input.GetTouch(0).phase is TouchPhase.Ended)
        {
            OneTouchPosition = pos_touch_1;
            if(Input.GetTouch(0).tapCount == 2)
            {
                imagem.transform.position = pos_touch_1;
                _classifications[(int)StateController.CurrentState].classification.PositionImage = pos_touch_1;
            }
        }
            
        else
            distance = Vector3.Distance(OneTouchPosition, pos_touch_1);


        if(Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            var move = OneTouchPosition - pos_touch_1;
            imagem.transform.position -= move;
            OneTouchPosition = pos_touch_1;
            _classifications[(int)StateController.CurrentState].classification.PositionImage = imagem.transform.position;
        }
            
    }

    private void UpdatePositionAndScaleImageState()
    {
        _classifications[(int)StateController.CurrentState].classification.PositionImage = imagem.transform.position;
        _classifications[(int)StateController.CurrentState].classification.ScaleImage = imagem.transform.localScale;
        imagem.transform.localPosition = DefaultPositionImage;
        imagem.transform.localScale = DefaultScaleImage;
    }
}
