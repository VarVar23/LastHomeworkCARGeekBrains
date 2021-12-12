using UnityEngine;
using UnityEngine.UI;

public class SliderSc : MonoBehaviour {

    public Slider slider;
    float value;

    void Start()
    {
        value = PlayerPrefs.GetFloat("Sensitivity", .4f);

        GameManager.Instance.dragSensity = value;
    }

    //set slider value
    void OnEnable()
    {
        value = GameManager.Instance.dragSensity;
        slider.value = value;
    }

    //save sensitivity value
    void OnDisable()
    {
        value = GameManager.Instance.dragSensity;
        slider.value = value;
        PlayerPrefs.SetFloat("Sensitivity", value);
    }
}
