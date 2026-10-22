using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScreen : MonoBehaviour
{
    public UIDocument m_uiDocument;
    private VisualElement m_DuckoSelector;
    private VisualElement m_RubySelector;
    private VisualElement m_SugarSelector;
    private void OnEnable()
    {
        m_uiDocument = GetComponent<UIDocument>();

        m_DuckoSelector = m_uiDocument.rootVisualElement.Q<VisualElement>("DuckoSelector");
        m_RubySelector = m_uiDocument.rootVisualElement.Q<VisualElement>("RubySelector");
        m_SugarSelector = m_uiDocument.rootVisualElement.Q<VisualElement>("SugarSelector");

        m_RubySelector.AddManipulator(new Clickable(() => { SceneManager.LoadScene(1); }));
        m_DuckoSelector.AddManipulator(new Clickable(() => { SceneManager.LoadScene(2); }));
        m_SugarSelector.AddManipulator(new Clickable(() => { SceneManager.LoadScene(3); }));

    }
}
