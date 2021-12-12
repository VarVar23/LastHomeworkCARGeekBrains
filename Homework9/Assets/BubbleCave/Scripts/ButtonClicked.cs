using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClicked : MonoBehaviour {

    //click on button rate
	public void ButtonRateClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        //add code here
    }

    //click on button leaderboard
    public void ButtonLeaderboardClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        //add code here
    }

    //click on restore button
    public void ButtonRestoreClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        //add code here
    }

    //click on no ads button
    public void ButtonNoAdsClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        //add code here
    }
}
