using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InitialScreenController : MonoBehaviour
{
    private UIDocument _document;

    public void RegisterActionToInitialButtons()
    {
        _document = FindObjectOfType<UIDocument>();
        Button classification = _document.rootVisualElement.Q<Button>("classification");
        Button history = _document.rootVisualElement.Q<Button>("history");
        Button sobre = _document.rootVisualElement.Q<Button>("about");

        classification.RegisterCallback<ClickEvent>(evt => GetComponent<OptionController>().ChangeScreen(Screens.States));
        history.RegisterCallback<ClickEvent>(evt => GetComponent<OptionController>().ChangeScreen(Screens.History));
    }
}
