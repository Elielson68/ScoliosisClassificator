using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SacroController : MonoBehaviour
{
    private const string SacroDropdown = "sacro-dropdown";
    private DropdownField _dropdown;
    private StateController _stateCotrol;
    private ClassificationWithSacro _sacro;
    private GameObject _sacroLine;
    
    public RawImage Image;
    public GameObject LineParent;
    public GameObject SacroLinePrefab;

    private void OnEnable()
    {
        ImageStateController imgStateController = FindObjectOfType<ImageStateController>();
        _stateCotrol = FindObjectOfType<StateController>();
        _dropdown = FindObjectOfType<UIDocument>().rootVisualElement.Q<DropdownField>();

        imgStateController.SetStateImage(Image);
        imgStateController.UpdateImageOnChangeState();
        
        _dropdown.style.display = DisplayStyle.Flex;
        _dropdown.RegisterValueChangedCallback(OnChooseOption);

        _sacro = FindObjectOfType<Classifications>()[0][0].classification as ClassificationWithSacro;
        _sacroLine = Instantiate(SacroLinePrefab, Vector3.zero, Quaternion.identity, LineParent.transform);
    }

    private void OnDisable()
    {
        float posX = _sacroLine.transform.GetChild(0).transform.position.x;
        _sacro.SubLines.Add(new Line(new Vector3(posX, 1080, 0), new Vector3(posX, -1080, 0)));
    }

    public void HideDropdown()
    {
        _dropdown.style.display = DisplayStyle.None;
        foreach(Transform child in LineParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnChooseOption(ChangeEvent<string> option)
    {
        if(_dropdown.choices.IndexOf(option.newValue) is int sac && sac != 0)
        {
            _sacro.Sacro = (Sacro) sac; 
            _stateCotrol.ShowFowardButton();
        }
        else
        {
            _stateCotrol.HideFowardButton();
        }
    }
}
