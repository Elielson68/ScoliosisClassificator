using UnityEngine;
using UnityEngine.UIElements;

public class OptionController : MonoBehaviour
{
    public ScreenAsset InitialScreen;
    public ScreenAsset StatesFlow;
    public ScreenAsset HistoryScreen;
    public ScreenAsset ReportScreen;

    private VisualElement _root;
    private VisualElement _currentOpenScreen;

    void Start()
    {
        _root = FindObjectOfType<UIDocument>().rootVisualElement;
        ChangeScreen(Screens.Initial);
        ClassificationFolder.ConfigureFoldersOnAndroid();
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

            case Screens.Report:
            InstantiateScreen(ReportScreen.Screen);
            ReportScreen.OnOpenScreen?.Invoke();
            break;
        }
    }
    public void ChangeScreen(int screens)
    {
        ChangeScreen((Screens) screens);
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
