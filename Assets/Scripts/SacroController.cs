using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SacroController : MonoBehaviour
{
    private const string SacroDropdown = "sacro-dropdown";
    public RawImage Image;
    public GameObject LineParent;
    public GameObject SacroLinePrefab;

    private DropdownField _dropdown;
    private StateController stateCotrol;
    ClassificationWithSacro sacro;
    
    void Start()
    {
        ImageStateController imgStateController = FindObjectOfType<ImageStateController>();
        stateCotrol = FindObjectOfType<StateController>();
        _dropdown = FindObjectOfType<UIDocument>().rootVisualElement.Q<DropdownField>();

        imgStateController.SetStateImage(Image);
        imgStateController.UpdateImageOnChangeState();
        
        _dropdown.style.display = DisplayStyle.Flex;
        _dropdown.RegisterValueChangedCallback(OnChooseOption);

        sacro = FindObjectOfType<Classifications>()[0][0].classification as ClassificationWithSacro;
        sacro.SubLines.Add(new Line(new Vector3(5, 1080, 0), new Vector3(5, -1080, 0)));
    }

    public void HideDropdown()
    {
        _dropdown.style.display = DisplayStyle.None;
    }

    private void OnChooseOption(ChangeEvent<string> option)
    {
        if(_dropdown.choices.IndexOf(option.newValue) is int sac && sac != 0)
        {
            sacro.Sacro = (Sacro) sac; 
            stateCotrol.ShowFowardButton();
        }
        else
        {
            stateCotrol.HideFowardButton();
        }
    }
}
