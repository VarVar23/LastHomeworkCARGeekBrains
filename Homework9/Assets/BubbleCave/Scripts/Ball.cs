using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
    [Space(10)]
    public BallType ballType;

    [Space(10)]
    public float ballSpeed = 10f;
    public float ballYPos = 5f;
    [Range(1.5f,3f)]
    public float fireDistance = 2f;
    [Range(4f, 8f)]
    public float holeSize = 6f;

    [Space(10)]
    public Sprite[] ballSprites;

    [Space(10)]
    public Sprite[] colorSprites;

    [Space(10)]
    public bool center;
    public bool shield;

    [Space(10)]
    [Header("Effects Prefabs")]
    public GameObject scorePrefab;
    public GameObject starPrefab;
    public GameObject lightningPrefab;
    public GameObject firePrefab;

    [Space(10)]
    public SpriteRenderer shieldRenderer;
    public SpriteRenderer ballRenderer;
    public Rigidbody2D rigidbody2d;
    public Collider2D coll3;
    public Collider2D coll2;
    public Animator animator;
    public Animator animatorSpec;
    public GameObject hole;

    [Space(10)]
    public List<GameObject> neighbourBalls;
    public bool ballChecked;
    public bool iceBall;

    bool moving;
    bool stopBall;
    public int color = -1;

    Vector3 targetDirection;
    Transform targetPosition;
    Transform circle;

    // Use this for initialization
    void Start () {

        if (!center)
        {
            rigidbody2d = gameObject.GetComponent<Rigidbody2D>();

            //correct in case, when camera position is equal (0,0) 
            transform.position = new Vector3(0, ballYPos, 0);

            targetDirection = (targetPosition.position - transform.position).normalized;

            neighbourBalls = new List<GameObject>();
        }
	}

    public void SetBall(BallType type, int colorId, bool shieldEnabled, Transform target, Transform parent)
    {
        targetPosition = target;
        circle = parent;

        if (type == BallType.NORMAL)
        {
            ballType = BallType.NORMAL;
    
            shield = shieldEnabled;
            shieldRenderer.enabled = shieldEnabled;
            animatorSpec.enabled = false;
            color = colorId;
            ballRenderer.sprite = colorSprites[color];
        }
        else
        {
            ballType = type;

            animatorSpec.enabled = false;
            color = -2;

            switch (type)
            {
                case BallType.DESTROYALL:
                    ballRenderer.sprite = ballSprites[1];
                    StartCoroutine(PlayAnimation(.3f, "DestroyAll"));
                    break;

                case BallType.DESTROYSAME:
                    ballRenderer.sprite = ballSprites[2];
                    StartCoroutine(PlayAnimation(.3f, "DestroySame"));
                    break;

                case BallType.FIRE:
                    ballRenderer.sprite = ballSprites[3];
                    StartCoroutine(PlayAnimation(.3f, "FireBall"));
                    break;

                case BallType.HOLE:
                    hole.SetActive(true);
                    ballRenderer.enabled = false;
                    break;

                case BallType.ICE:
                    ballRenderer.sprite = ballSprites[4];
                    StartCoroutine(PlayAnimation(.3f, "IceBall"));
                    break;

                case BallType.RAINBOW:
                    ballRenderer.sprite = ballSprites[5];
                    StartCoroutine(PlayAnimation(.3f, "RainbowBall"));
                    break;

                case BallType.RANDOMCOLOR:
                    ballRenderer.sprite = ballSprites[6];
                    StartCoroutine(PlayAnimation(.3f, "RandomColor"));
                    break;

                case BallType.ROTATION:
                    ballRenderer.sprite = ballSprites[7];
                    StartCoroutine(PlayAnimation(.3f, "ChangeRotation"));
                    break;

                case BallType.STONE:
                    ballRenderer.sprite = ballSprites[8];
                    break;
            }
        }
    }

  
	// Update is called once per frame
	void Update () {
        if (!center && stopBall && GameManager.Instance.gameState != GameState.PREGAMEOVER && !(ballType == BallType.HOLE))
            rigidbody2d.velocity = Vector2.zero;
    }

    IEnumerator PlayAnimation(float delay, string anim)
    {
        yield return new WaitForSeconds(delay);
        animatorSpec.enabled = true;
        Debug.Log(anim);
        animatorSpec.Play(anim);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Ball")
        {
            if (transform.position.y >= ballYPos)
            {
                moving = false;
                GameManager.Instance.GameOver();
                transform.position = new Vector3(Random.Range(-.01f, .01f), transform.position.y, 0);
            }

            if (!CheckNeighbourBall(coll.gameObject))
                AddBall(coll.gameObject);

            if (color == coll.gameObject.GetComponent<Ball>().color && color > -1)
            {
                //call ball checking only for moving ball (call with a little delay in case if player hit two same balls)
                if (moving)
                {
                    StartCoroutine(CheckBalls(.15f));
                }

            }
            else if (ballType == BallType.DESTROYALL)
            {
                if (moving)
                {
                    GameManager.Instance.DestroyAllBalls();
                }
            }
            else if (ballType == BallType.DESTROYSAME)
            {
                if (coll.gameObject.GetComponent<Ball>().center || coll.gameObject.GetComponent<Ball>().ballType == BallType.STONE)
                {
                    DestroyWithLightning();
                    StartCoroutine(ReadyForNextBall(.15f));

                }
                else if (moving)
                {
                    color = coll.GetComponent<Ball>().color;
                    GameManager.Instance.DestroySameBalls(coll.GetComponent<Ball>().color);
                }
            }
            else if (ballType == BallType.FIRE)
            {
                if (moving)
                {
                    if (coll.gameObject.GetComponent<Ball>().center)
                    {
                        DestroyWithFire();
                        StartCoroutine(CheckBalls(.25f));
                        moving = false;
                    }
                    else
                    {
                        color = ballSprites.Length - 1;
                        GameManager.Instance.DestroyBallsFire(transform.position, fireDistance);
                        AudioManager.Instance.PlayEffects(AudioManager.Instance.fire);
                    }
                }
            }
            else if (ballType == BallType.HOLE)
            {
                if (moving)
                {
                    coll2.enabled = false;
                    coll3.enabled = false;

                    rigidbody2d.bodyType = RigidbodyType2D.Static;

                    StartCoroutine(Scale(hole.transform, new Vector3(holeSize, 1, holeSize), .35f));
                    Invoke("Descale", .35f);
                    StartCoroutine(CheckBalls(1.3f));
                }

            }
            else if (ballType == BallType.ICE)
            {
                if (iceBall)
                {
                    BallChange();
                }

                Invoke("SetIceBall", .1f);

                if (moving)
                    StartCoroutine(ReadyForNextBall(.5f));
            }
            else if (ballType == BallType.RAINBOW)
            {
                if (moving)
                {
                    if (coll.gameObject.GetComponent<Ball>().center || coll.gameObject.GetComponent<Ball>().ballType == BallType.ICE || coll.gameObject.GetComponent<Ball>().ballType == BallType.STONE)
                        BallChange();
                    else 
                        SetBallColor(coll.gameObject.GetComponent<Ball>().color);

                    StartCoroutine(CheckBalls(.15f));
                }
            }
            else if (ballType == BallType.RANDOMCOLOR)
            {
                if (moving)
                {
                    BallChange();
                    StartCoroutine(CheckBalls(.15f));
                }
            }
            else if (ballType == BallType.ROTATION)
            {
                if (moving)
                {
                    GameManager.Instance.ChangeRotation();
                    StartCoroutine(ReadyForNextBall(.25f));
                    DestroyBall();
                }
            }
            else if (ballType == BallType.STONE) 
            {
                if (moving)
                    StartCoroutine(ReadyForNextBall(.5f));
            }
            else if (moving)
            {
                StartCoroutine(ReadyForNextBall(.5f));
            }

            if (!center)
            {
                transform.SetParent(circle);

                if (moving)
                    AudioManager.Instance.PlayEffects(AudioManager.Instance.bubbleStick);

                moving = false;
                Invoke("StopBall", .02f);
            }

        }
        else if (coll.gameObject.tag == "Hole" && !center)
        {
            center = true;
            coll3.enabled = false;
            Invoke("DestroyBall", .6f);
            AudioManager.Instance.PlayEffects(AudioManager.Instance.blackHole);
        }
    }

    void StopBall()
    {
        stopBall = true;

        rigidbody2d.mass = 350;

        if (rigidbody2d.bodyType != RigidbodyType2D.Static)
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;   
    }

    void SetIceBall()
    {
        iceBall = true;
    }

    IEnumerator CheckBalls(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.CheckBalls(gameObject.GetComponent<Ball>(),color);
    }

    //check if neighbour list contains ball
    public bool CheckNeighbourBall(GameObject ball)
    {
        return neighbourBalls.Contains(ball);
    }

    //add ball to neighbour list
    void AddBall(GameObject ball)
    {
        neighbourBalls.Add(ball);
    }

    //shoot ball
    public void Shoot()
    {
        moving = true;
        rigidbody2d.AddForce(targetDirection * ballSpeed, ForceMode2D.Impulse);
    }

    public void DestroyBall()
    {
        gameObject.GetComponent<Animator>().Play("BallDestroy");
        Invoke("RemoveObject", .3f);
        gameObject.transform.parent = null;
    }

	public void DestroyWithStar()
	{
        GameObject tmp = Instantiate(starPrefab);
        tmp.transform.position = gameObject.transform.position;
        gameObject.GetComponent<Animator>().Play("BallDestroy");

        Invoke("RemoveObject", .3f);

        gameObject.transform.parent = null;
	}

    public void DestroyWithLightning()
    {
        GameObject tmp = Instantiate(lightningPrefab);
        tmp.transform.position = gameObject.transform.position;
        gameObject.GetComponent<Animator>().Play("BallDestroy");

        Invoke("RemoveObject", .3f);

        gameObject.transform.parent = null;
    }

    public void DestroyWithFire()
    {
        GameObject tmp = Instantiate(firePrefab);
        tmp.transform.position = gameObject.transform.position;

        ballRenderer.sprite = colorSprites[colorSprites.Length - 1];

        Invoke("RemoveObject", .6f);
        Invoke("PlayBallDestroyAnimation",.3f);
        gameObject.transform.parent = null;
    }

	public void RemoveNeighbour(GameObject ball)
    {
        neighbourBalls.Remove(ball);
    }

    void RemoveObject()
    {
        if (GameManager.Instance.gameState == GameState.PLAYING)
        {
            GameObject tmp = Instantiate(scorePrefab);
            tmp.transform.position = gameObject.transform.position;

            ScoreManager.Instance.UpdateScore(1);
        }

        for (int i = 0; i < neighbourBalls.Count; i++)
        {
            if (neighbourBalls[i] != null)
                neighbourBalls[i].GetComponent<Ball>().RemoveNeighbour(gameObject);
        }

        Destroy(gameObject);
      
    }

    void PlayBallDestroyAnimation()
    {
        gameObject.GetComponent<Animator>().Play("BallDestroy");
    }

    void Descale()
    {
        StartCoroutine(Scale(hole.transform, new Vector3(.1f, 1, .1f), 1f));
        Invoke("RemoveObject",1f);
    }

    IEnumerator Scale(Transform objectToScale, Vector3 toScale, float duration)
    {

        float counter = 0;

        //Get the current scale of the object to be moved
        Vector3 startScaleSize = objectToScale.localScale;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
            yield return null;
        }

    }

    public void BallChange()
    {
        ballType = BallType.NORMAL;
        animatorSpec.enabled = false;
        color = Random.Range(0, colorSprites.Length - 4);
        ballRenderer.sprite = colorSprites[color];
    }

    void SetBallColor(int colorId)
    {
        ballType = BallType.NORMAL;
        animatorSpec.enabled = false;
        color = colorId;
        ballRenderer.sprite = colorSprites[colorId];
    }

    public void RemoveShield()
    {
        shield = false;
        shieldRenderer.enabled = false;
    }

    IEnumerator ReadyForNextBall(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.readyToShoot = true;
    }
}
