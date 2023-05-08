using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class OptionController : MonoBehaviour
{
    public ScreenAsset InitialScreen;
    public ScreenAsset StatesFlow;
    public ScreenAsset HistoryScreen;

    private VisualElement _root;
    private VisualElement _currentOpenScreen;

    void Start()
    {
        _root = FindObjectOfType<UIDocument>().rootVisualElement;
        ChangeScreen(Screens.Initial);
    }

    public void ChangeScreen(Screens screens)
    {
        switch(screens)
        {
            case Screens.Initial:
            InstantiateScreen(InitialScreen.Screen);
            InitialScreen.OnOpenScreen?.Invoke();
            break;

            case Screens.States:
            InstantiateScreen(StatesFlow.Screen);
            StatesFlow.OnOpenScreen?.Invoke();
            break;

            case Screens.History:
            InstantiateScreen(HistoryScreen.Screen);
            HistoryScreen.OnOpenScreen?.Invoke();
            break;
        }
    }

    private void InstantiateScreen(VisualTreeAsset screen)
    {
        _root.Clear();
        TemplateContainer temp = screen.Instantiate();
        _root.Add(temp);
        foreach(var item in screen.stylesheets)
        {
            temp.styleSheets.Add(item);
        }
        _currentOpenScreen = temp;
    }
}
