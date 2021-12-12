using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LocalizationWindow : MonoBehaviour
{
    [SerializeField] private Button _enlishButton;
    [SerializeField] private Button _russianButton;

    void Start()
    {
        _enlishButton.onClick.AddListener(() => ChangeLanguage(0));
        _russianButton.onClick.AddListener(() => ChangeLanguage(1));

    }

    private void OnDestroy()
    {
        _enlishButton.onClick.RemoveAllListeners();
        _russianButton.onClick.RemoveAllListeners();
    }

    private void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
