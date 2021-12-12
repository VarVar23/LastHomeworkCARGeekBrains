using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance { get; set; }

    [Header("Main Menu")]
    public GameObject mainMenu;
    public Button buttonHome;
    public Button buttonSettings;
    public Button buttonHowToPlay;
    public GameObject buttonHomeIcon;
    public GameObject buttonSettingsIcon;
    public GameObject buttonHowToPlayIcon;
    public GameObject tutorialMenu;
    public GameObject settingsMenu;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public Button buttonHomeP;
    public Button buttonSettingsP;
    public Button buttonHowToPlayP;
    public GameObject buttonHomeIconP;
    public GameObject buttonSettingsIconP;
    public GameObject buttonHowToPlayIconP;
    public GameObject tutorialMenuP;
    public GameObject settingsMenuP;
    public GameObject gameOverMenuP;

    [Space(10)]
    public Animator pauseMenuAnimator;
    public Animator menuAnimator;
    public Animator shadowAnimator;
    public Animator handAnimator;

    public GameObject shadow;
    public GameObject top;

    public Text scoreLabel;
    public Text bestScoreLabel;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Start () {
        GameManager.Instance.gameState = GameState.MENU;
        StartCoroutine(ShowMenu(1,true));

        AudioManager.Instance.PlayMusic(AudioManager.Instance.bgMusic);

        buttonHome.interactable = false; 
        buttonSettings.interactable = true;
        buttonHowToPlay.interactable = true;
        buttonHomeIcon.SetActive(false);
        buttonSettingsIcon.SetActive(true);
        buttonHowToPlayIcon.SetActive(true);

        mainMenu.SetActive(true);
        tutorialMenu.SetActive(false);
        settingsMenu.SetActive(false);

        buttonHomeP.interactable = false;
        buttonSettingsP.interactable = true;
        buttonHowToPlayP.interactable = true;
        buttonHomeIconP.SetActive(false);
        buttonSettingsIconP.SetActive(true);
        buttonHowToPlayIconP.SetActive(true);

        pauseMenu.SetActive(true);
        tutorialMenuP.SetActive(false);
        settingsMenuP.SetActive(false);
        gameOverMenuP.SetActive(false);

        top.SetActive(false);
    }
    	
    //click on play button in main menu
    public void ButtonPlayClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        GameManager.Instance.StartGame();
        StartCoroutine(HideMenu(0));
        handAnimator.enabled = true;
    }

    //show main menu
    IEnumerator ShowMenu(float delay, bool gameStart)
    {
        StartCoroutine(ShadowChangeStatus(true, 0));
        yield return new WaitForSeconds(delay);

        menuAnimator.Play("MenuShow");

        if (!gameStart)
            shadowAnimator.Play("ShadowShow");

        top.SetActive(false);
        
    }

    //hide main menu
    IEnumerator HideMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        menuAnimator.Play("MenuHide");
        shadowAnimator.Play("ShadowHide");
        StartCoroutine(ShadowChangeStatus(false, .3f));

        top.SetActive(true);
    }

    IEnumerator ShadowChangeStatus(bool status, float delay)
    {
        yield return new WaitForSeconds(delay);

        shadow.SetActive(status);
    }

    //click on pause button
    public void PauseGame()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        if (GameManager.Instance.gameState == GameState.PAUSED)
        {
            StartCoroutine(ShadowChangeStatus(false, .3f));
            pauseMenuAnimator.Play("PauseMenuHide");
            Invoke("ContinueGame", .3f);
        }
        else
        {
            pauseMenu.SetActive(true);
            gameOverMenuP.SetActive(false);
            shadow.SetActive(true);
            shadowAnimator.Play("ShadowShow");
            pauseMenuAnimator.Play("PauseMenuShow");
            GameManager.Instance.gameState = GameState.PAUSED;
            top.SetActive(false);
        }
    }

    //click on pause button
    public void ShowGameOverMenu()
    {
        shadow.SetActive(true);
        shadowAnimator.Play("ShadowShow");
        pauseMenuAnimator.Play("PauseMenuShow");
        pauseMenu.SetActive(false);
        gameOverMenuP.SetActive(true);

        if (ScoreManager.Instance.CheckHighScore())
        {
            ScoreManager.Instance.SetHighScore();
            AudioManager.Instance.PlayEffects(AudioManager.Instance.bestScore);
        }

        scoreLabel.text = ScoreManager.Instance.CurrentScore() + "";
        bestScoreLabel.text = ScoreManager.Instance.BestScore() + "";

        GameManager.Instance.HideNext(0f);

        Invoke("EmptyBalls",.25f);
        Invoke("ChangeStatus",.35f);
    }

    //change status to game over
    void ChangeStatus()
    {
        GameManager.Instance.gameState = GameState.GAMEOVER;
    }

    //empty all balls on scene
    void EmptyBalls()
    {
        GameManager.Instance.EmptyBalls();
    }

    //click on new game button
    public void NewGameClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);

        if (GameManager.Instance.gameState == GameState.PREGAMEOVER)
            return;

        pauseMenuAnimator.Play("PauseMenuHide");
        shadowAnimator.Play("ShadowHide");


        GameManager.Instance.StartGame();
        top.SetActive(true);

        StartCoroutine(ShadowChangeStatus(false, .3f));
    }

    //click on button home
    public void HomeClicked()
    {
        StartCoroutine(ShadowChangeStatus(true, 0));
        pauseMenuAnimator.Play("PauseMenuHide");
        shadowAnimator.Play("ShadowHide");

        Invoke("EmptyBalls", 1f);

        if (GameManager.Instance.gameState == GameState.PAUSED)
            GameManager.Instance.HideNext(.5f);

        GameManager.Instance.gameState = GameState.MENU;
        StartCoroutine(ShowMenu(1.3f,false));

    }

    //click on continue button in pause menu
    void ContinueGame()
    {
        GameManager.Instance.gameState = GameState.PLAYING;
        top.SetActive(true);
    }

    //click on home button in pause menu
    public void ButtonHomeClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        buttonHome.interactable = false;
        buttonSettings.interactable = true;
        buttonHowToPlay.interactable = true;
        buttonHomeIcon.SetActive(false);
        buttonSettingsIcon.SetActive(true);
        buttonHowToPlayIcon.SetActive(true);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    //click on settings button in main menu
    public void ButtonSettingsClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        buttonHome.interactable = true;
        buttonSettings.interactable = false;
        buttonHowToPlay.interactable = true;
        buttonHomeIcon.SetActive(true);
        buttonSettingsIcon.SetActive(false);
        buttonHowToPlayIcon.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        tutorialMenu.SetActive(false);
    }

    //click on button how to play in main menu
    public void ButtonHowToplayClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        buttonHome.interactable = true;
        buttonSettings.interactable = true;
        buttonHowToPlay.interactable = false;
        buttonHomeIcon.SetActive(true);
        buttonSettingsIcon.SetActive(true);
        buttonHowToPlayIcon.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        tutorialMenu.SetActive(true);
    }

    //click on home button in pause menu
    public void ButtonHomePClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        buttonHomeP.interactable = false;
        buttonSettingsP.interactable = true;
        buttonHowToPlayP.interactable = true;
        buttonHomeIconP.SetActive(false);
        buttonSettingsIconP.SetActive(true);
        buttonHowToPlayIconP.SetActive(true);
        pauseMenu.SetActive(true);
        settingsMenuP.SetActive(false);
        tutorialMenuP.SetActive(false);

        if (GameManager.Instance.gameState == GameState.PREGAMEOVER)
        {
            pauseMenu.SetActive(false);
            gameOverMenuP.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(true);
            gameOverMenuP.SetActive(false);
        }

    }

    //click on settings button in pause menu
    public void ButtonSettingsPClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        buttonHomeP.interactable = true;
        buttonSettingsP.interactable = false;
        buttonHowToPlayP.interactable = true;
        buttonHomeIconP.SetActive(true);
        buttonSettingsIconP.SetActive(false);
        buttonHowToPlayIconP.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenuP.SetActive(true);
        tutorialMenuP.SetActive(false);
        gameOverMenuP.SetActive(false);
    }

    //click on how to play button in pause menu
    public void ButtonHowToplayPClicked()
    {
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        buttonHomeP.interactable = true;
        buttonSettingsP.interactable = true;
        buttonHowToPlayP.interactable = false;
        buttonHomeIconP.SetActive(true);
        buttonSettingsIconP.SetActive(true);
        buttonHowToPlayIconP.SetActive(false);
        pauseMenu.SetActive(false);
        settingsMenuP.SetActive(false);
        tutorialMenuP.SetActive(true);
        gameOverMenuP.SetActive(false);
    }
}
