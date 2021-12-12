using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour {

    public Text tutorialText;
    public string[] texts;
    public Image tutorialImage;
    public Sprite[] sprites;

    public Button next;
    public Button previous;


    int index;

	// Use this for initialization
	void Start () {
        index = 0;
        previous.interactable = false;
        UpdateTutorial();
	}
	
    //click on next tutorial image button
    public void ButtonNextClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        if (index < (texts.Length - 1))
        {
            index++;
            UpdateTutorial();

            next.interactable &= index != texts.Length - 1;

            previous.interactable |= index > 0;
        }
    }

    //click on previous tutorial image button
    public void ButtonPreviousClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        if (index > 0)
        {
            index--;
            UpdateTutorial();

            previous.interactable &= index != 0;

            next.interactable |= index < texts.Length - 1;
        }
    }

    //update tutorial image and text
    void UpdateTutorial()
    {
        tutorialText.text = texts[index];
        tutorialImage.sprite = sprites[index];
    }
}
