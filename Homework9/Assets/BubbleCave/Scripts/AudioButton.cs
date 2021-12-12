using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;
    public bool efx;
    public GameObject spriteObject;

	void Start()
	{
        //change audio buttons sprite
        spriteObject.GetComponent<Image>().sprite = efx ? AudioManager.Instance.IsEfxMute() ? offSprite : onSprite : AudioManager.Instance.IsMusicMute() ? offSprite : onSprite;

    }

    //click on music button
    public void MusicButtonClicked()
    {
        spriteObject.GetComponent<Image>().sprite = AudioManager.Instance.IsMusicMute() ? onSprite : offSprite;
        AudioManager.Instance.MuteMusic();
    }

    //click ob on effects button
    public void EfxButtonClicked()
    {
        spriteObject.GetComponent<Image>().sprite = AudioManager.Instance.IsEfxMute() ? onSprite : offSprite;
        AudioManager.Instance.MuteEfx();
    }
}
