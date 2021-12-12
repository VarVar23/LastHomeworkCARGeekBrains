using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; set; }

    [Space (10)]
    public GameObject rotateObject;
    public GameObject center;
    public Transform circle;
    public GameState gameState;

    [Space(10)]
    public GameObject nextBall;

    [Space(10)]
    [Header("Settings")]
    [Space(10)]
    [Range(1,3)]
    public float shootDelay = 2f;
    [Range (.1f,1f)]
    public float dragSensity = .4f;

    [Space(10)]
    [Header("Spawn Probabilities")]

    [Space(10)]
    public SpecialProbability[] specialProbabilities;

    [Space(10)]
    [Range(0f, 100f)]
    public float shieldProbability;

    [Space(10)]
    public int numberOfSameBalls = 0;

    [Space(10)]
    public NextBall nextBallIndicator;

    [Space(10)]
    public bool readyToShoot;

    [Space(10)]
    public List<GameObject> allBalls;

    Vector3 mouse;
    Vector3 mouseOffset;

    int ballNumber;
    int numberOfChecked = 0;
    int rotationDirection = 1;

    float sumAllProbabilities;
    float random;

    GameObject currentBall;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if (gameState == GameState.PLAYING)
        {
            if (Input.GetMouseButtonDown(0)) //get mouse position on mouse down
            {
                mouse = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0)) //rotate center object depends on drag length and drag sensitivity
            {
                mouseOffset = (Input.mousePosition - mouse);

                rotateObject.transform.Rotate(new Vector3(0, 0, -(mouseOffset.x) * dragSensity) * rotationDirection);

                mouse = Input.mousePosition;
            }

            //if true prepare next ball, spawn ball and shoot it
            if (readyToShoot)
            {
                readyToShoot = false;
                StartCoroutine(SpawnBall(0, false));
                StartCoroutine(ShootCurrentBall(shootDelay));
                StartCoroutine(HideNextBall(0));
                StartCoroutine(ShowNext(shootDelay - .3f));
            }
        } else if (gameState == GameState.READYTOSTART)
        {
            if (Input.GetMouseButtonDown(0)) //start game on first click or touch
            {
                gameState = GameState.PLAYING;
                StartCoroutine(ShootCurrentBall(.3f));
                StartCoroutine(ShowNext(.3f));
            }
        }

    }

    //prepare for new game
    IEnumerator PrepareGame()
    {
        ScoreManager.Instance.ResetScore();

        Physics2D.gravity = new Vector2(0, 0);
        SumAllProbabilities();
        ballNumber = 0;
        readyToShoot = false;
        gameState = GameState.PREPAREGAME;

        StartCoroutine(SpawnBall(0, true));

        yield return new WaitForSeconds(.3f);
        gameState = GameState.READYTOSTART;
    }

    //shoot current ball after delay
    IEnumerator ShootCurrentBall(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentBall != null)
            currentBall.GetComponent<Ball>().Shoot();
    }

    //start game
    public void StartGame()
    {
        dragSensity = PlayerPrefs.GetFloat("Sensitivity", .4f);

        StartCoroutine(PrepareGame());
    }

    //clear all balls from scene
    public void EmptyBalls()
    {
        for (int i = 0; i < allBalls.Count; i++)
        {
            allBalls[i].GetComponent<Ball>().DestroyBall();
        }

        allBalls.Clear();
    }


    //sum all probabilities for special balls
    void SumAllProbabilities()
    {
        sumAllProbabilities = 0;

        for (int i = 0; i < specialProbabilities.Length; i++)
        {
            sumAllProbabilities += specialProbabilities[i].probability;
        }
    }

    //reset checks on all balls
    void ResetBallsCheck()
    {
        for (int i = 0; i < allBalls.Count; i++)
        {
            allBalls[i].GetComponent<Ball>().ballChecked = false;
        }
    }

    //spawn ball
    IEnumerator SpawnBall(float delay, bool first)
    {
        Debug.Log("Spawn Ball");
        yield return new WaitForSeconds(delay);

        if (gameState != GameState.GAMEOVER)
        {

            currentBall = null;
            currentBall = Instantiate(nextBall);
            currentBall.name = "Ball" + ballNumber;

            if (!first)
            {
                currentBall.GetComponent<Ball>().SetBall(nextBallIndicator.ballType, nextBallIndicator.colorId, nextBallIndicator.shieldEnbld, center.transform, circle);
            }
            else
            {
                currentBall.GetComponent<Ball>().SetBall(BallType.NORMAL, Random.Range(0, currentBall.GetComponent<Ball>().colorSprites.Length -1), nextBallIndicator.shieldEnbld, center.transform, circle);
            }

            ballNumber++;
            AddBall(currentBall);
        }

    }

    //show next ball after delay
    IEnumerator ShowNext(float delay)
    {
        yield return new WaitForSeconds(delay);
        //create random ball

        if (sumAllProbabilities < 0.01 || gameState == GameState.PREPAREGAME) //first spawned ball is always normal ball
        {
            //change ball object to normal and change sprite to random;
            if (Random.Range(0, 100) > shieldProbability || gameState == GameState.PREPAREGAME)
            {
                nextBallIndicator.SetNextBall(BallType.NORMAL, false);
            }
            else
            {
                nextBallIndicator.SetNextBall(BallType.NORMAL, true);
            }

        }
        else
        {
            random = Random.Range(0.01f, 100f);
            //Debug.Log("Random value: " + random + ".");

            if (random <= sumAllProbabilities) //if random value is smaller than summary of all special ball posibilities than create special ball
            {
                for (int i = 0; i < specialProbabilities.Length; i++)
                {
                    if (specialProbabilities[i].probability > 0 && random > 0.0f && random < specialProbabilities[i].probability)
                    {
                        nextBallIndicator.SetNextBall(specialProbabilities[i].ballType, false);
                    }

                    random -= specialProbabilities[i].probability;
                }

            }
            else //create normal ball
            {
                if (Random.Range(1, 100) > shieldProbability) //if random is higher than shield probability value then set next ball to normal ball without shield
                {
                    nextBallIndicator.SetNextBall(BallType.NORMAL, false);
                }
                else //else create normal ball with shield on
                {
                    nextBallIndicator.SetNextBall(BallType.NORMAL, true);
                }
            }
        }
    }

    //hide sprite of next ball
    IEnumerator HideNextBall(float delay)
    {
        yield return new WaitForSeconds(delay);
        nextBallIndicator.PlayHideAnimation();
    }

    //change game state after delay
    IEnumerator ChangeState(GameState state, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameState = state;
    }

    //add new ball to all balls list
    public void AddBall(GameObject ball)
    {
        allBalls.Add(ball);

        //remove missing objects from all balls list
        allBalls.RemoveAll(GameObject => GameObject == null);
    }

    //check all balls on scene
    public void CheckBalls(Ball ball, int color)
    {
        if (gameState == GameState.PREGAMEOVER)
            return;

        //destroy same balls
        Debug.Log("Checking balls...");

        Ball tmp;
        //reset balls
        ResetBalls();

        //reset same balls counter
        numberOfSameBalls = 1;
        ball.ballChecked = true;
        //Debug.Log("Same balls before check: " + numberOfSameBalls);

        //check same neighbours for last ball
        CheckSameNeighbours(ball, color);
        //Debug.Log("Same balls after check: " + numberOfSameBalls);

        //delete same balls
        for (int i = allBalls.Count - 1; i >= 0; i--)
        {
            if (allBalls[i] != null)
            {
                tmp = allBalls[i].GetComponent<Ball>();

                if (!tmp.center && tmp.ballChecked && numberOfSameBalls > 2)
                {
                    if (!tmp.shield)
                    {
                        tmp.DestroyBall();
                        allBalls.RemoveAt(i);
                    }
                    else
                    {
                        tmp.RemoveShield();
                    }
                }
            }
        }


        if (numberOfSameBalls > 2)
            AudioManager.Instance.PlayEffects(AudioManager.Instance.great);

        //reset ball checks, remove missing objects from balls neighbours list
        ResetBalls();

        //destroy balls without connection to center
        numberOfChecked = 0;

        if (numberOfSameBalls <= 2)
            ball.ballChecked = true;

        StartCoroutine(DestroyNotConnected(center.GetComponent<Ball>(), true, 0));
    }

    //destroy not connected balls after delay
    IEnumerator DestroyNotConnected(Ball ball, bool main, float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyNotConnectedRecursion(ball, true);
    }

    //recursion for destroying not connected ball
    void DestroyNotConnectedRecursion(Ball ball, bool main)
    {
        Ball temp;

        for (int i = 0; i < ball.neighbourBalls.Count; i++)
        {
            if (ball.neighbourBalls[i] != null)
            {
                temp = ball.neighbourBalls[i].GetComponent<Ball>();

                if (!temp.ballChecked)
                {
                    numberOfChecked++;
                    temp.ballChecked = true;
                    DestroyNotConnectedRecursion(temp, false);
                }
            }
        }

        if (main)
        {
            //Debug.Log("Number of checked; " + numberOfChecked);
            StartCoroutine(DestroyBalls(.3f));
        }
    }

    //destroy checked balls
    IEnumerator DestroyBalls(float delay)
    {
        yield return new WaitForSeconds(delay);

        Ball temp;
        int tmp = 0;

        for (int i = 0; i < allBalls.Count; i++)
        {
            if (allBalls[i] != null)
            {
                temp = allBalls[i].GetComponent<Ball>();
                if (!temp.ballChecked)
                {
                    temp.DestroyBall();
                    tmp++;
                }
            }
        }

        if (tmp > 0)
            AudioManager.Instance.PlayEffects(AudioManager.Instance.great);

        allBalls.RemoveAll(GameObject => GameObject == null);

        ResetBalls();

        readyToShoot = true;
    }

    //check for same color balls
    public void CheckSameNeighbours(Ball ball, int color)
    {
        Ball temp;

        for (int i = 0; i < ball.neighbourBalls.Count; i++)
        {
            if (ball.neighbourBalls[i] != null)
            {
                temp = ball.neighbourBalls[i].GetComponent<Ball>();

                if ( !temp.ballChecked && temp.color == color)
                {
                    numberOfSameBalls++;
                    temp.ballChecked = true;
                    CheckSameNeighbours(temp, color);
                }
            }
        }

    }

    //reset checked balls
    void ResetBalls()
    {
        for (int i = 0; i < allBalls.Count; i++)
        {
            if (allBalls[i] != null)
            {
                allBalls[i].GetComponent<Ball>().ballChecked = false;
            }
        }
    }

    //hide next ball after time
    public void HideNext(float delay)
    {
        StartCoroutine(HideNextBall(delay));
    }

    //when game is over
    public void GameOver()
    {
        if (gameState == GameState.PREGAMEOVER)
            return;

        StartCoroutine(HideNextBall(0));

        gameState = GameState.PREGAMEOVER;

        Physics2D.gravity = new Vector2(0, -9.81f);

        for (int i = 0; i < allBalls.Count; i++)
        {
            if (allBalls[i].GetComponent<Rigidbody2D>() != null)
                allBalls[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }

        StopAllCoroutines();

        AudioManager.Instance.PlayEffects(AudioManager.Instance.gameOver);

        Invoke("ShowGameOverUI", 1f);
    }

    //show gameover gui
    void ShowGameOverUI()
    {
        UIManager.Instance.ShowGameOverMenu();
    }

    //called when star ball hits any ball
    public void DestroyAllBalls()
    {
        for (int i = 0; i < allBalls.Count; i++)
        {
            if (allBalls[i].GetComponent<Ball>().ballType != BallType.STONE)
                allBalls[i].GetComponent<Ball>().DestroyWithStar();
        }

        StartCoroutine(DestroyNotConnected(center.GetComponent<Ball>(), true, 1.01f));
    }

    //called when lightning ball hits
    public void DestroySameBalls(int colorId)
    {
        Ball temp;
        for (int i = 0; i < allBalls.Count; i++)
        {
            temp = allBalls[i].GetComponent<Ball>();
            if (temp.color == colorId)   
                temp.DestroyWithLightning();
        }

        StartCoroutine(DestroyNotConnected(center.GetComponent<Ball>(), true, 1.01f));
    }

    //called when fire ball hits
    public void DestroyBallsFire(Vector2 position, float maxDistance)
    {
        Ball temp;
        for (int i = 0; i < allBalls.Count; i++)
        {
            temp = allBalls[i].GetComponent<Ball>();

            if (temp != null)
                if (Mathf.Abs(Vector2.Distance(position,temp.gameObject.transform.position)) < maxDistance)
                {
                    if (temp.ballType == BallType.STONE)
                        {}
                    else if (temp.iceBall)
                        temp.BallChange();
                    else
                        temp.DestroyWithFire();
                }

        }

        StartCoroutine(DestroyNotConnected(center.GetComponent<Ball>(), true, 1.01f));
    }

    //called when change rotation ball hits (change rotation of center)
    public void ChangeRotation()
    {
        rotationDirection *= -1;
        ChainManager.Instance.ChangeRotation();
    }

    //set drag sensitivity
    public void SetDragSensitivity(Slider sl)
    {
        dragSensity = sl.value;
    }

}
