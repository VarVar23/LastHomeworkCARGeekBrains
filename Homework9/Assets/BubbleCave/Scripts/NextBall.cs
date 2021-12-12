using System.Collections;
using UnityEngine;

public class NextBall : MonoBehaviour {

    [Space(10)]
    public SpriteRenderer ballRenderer;
    public SpriteRenderer shieldRenderer;
    public Animator animator;
    public Animator animatorSpec;
    public GameObject hole;

    [Space(10)]
    public Sprite[] ballSprites;
    public Sprite[] colorSprites;

    public BallType ballType;
    public int colorId;
    public bool shieldEnbld;

	// Use this for initialization
	void Start () {
        animator.enabled = false;
	}

    //show next ball 
    public void SetNextBall(BallType type, bool shieldEnabled)
    {
        shieldRenderer.enabled = shieldEnabled;
        animatorSpec.enabled = false;
        animator.enabled = false;
        hole.SetActive(false);
        ballRenderer.enabled = true;
        shieldEnbld = shieldEnabled;

        //show normal ball sprite for next ball
        if (type == BallType.NORMAL)
        {
            ballType = BallType.NORMAL;

            colorId = Random.Range(0, colorSprites.Length);

            ballRenderer.sprite = colorSprites[colorId];

        }
        else //show special ball sprite for next ball
        {
            ballType = type;
            
            colorId = -2;

            switch (type)
            {
                case BallType.DESTROYALL:
                    ballRenderer.sprite = ballSprites[0];
                    StartCoroutine(PlayAnimation(.3f, "DestroyAll"));
                    break;

                case BallType.DESTROYSAME:
                    ballRenderer.sprite = ballSprites[1];
                    StartCoroutine(PlayAnimation(.3f, "DestroySame"));
                    break;

                case BallType.FIRE:
                    ballRenderer.sprite = ballSprites[2];
                    StartCoroutine(PlayAnimation(.3f, "FireBall"));
                    break;

                case BallType.HOLE:
                    hole.SetActive(true);
                    ballRenderer.enabled = false;
                    break;

                case BallType.ICE:
                    ballRenderer.sprite = ballSprites[3];
                    StartCoroutine(PlayAnimation(.3f, "IceBall"));
                    break;

                case BallType.RAINBOW:
                    ballRenderer.sprite = ballSprites[4];
                    StartCoroutine(PlayAnimation(.3f, "RainbowBall"));
                    break;

                case BallType.RANDOMCOLOR:
                    ballRenderer.sprite = ballSprites[5];
                    StartCoroutine(PlayAnimation(.3f, "RandomColor"));
                    break;

                case BallType.ROTATION:
                    ballRenderer.sprite = ballSprites[6];
                    StartCoroutine(PlayAnimation(.3f, "ChangeRotation"));
                    break;

                case BallType.STONE:
                    ballRenderer.sprite = ballSprites[7];
                    break;
            }
        }

        animator.enabled = true;
        animator.Play("BallSpawn");
    }

    //play animation for next ball
    IEnumerator PlayAnimation(float delay, string anim)
    {
        yield return new WaitForSeconds(delay);
        animatorSpec.enabled = true;
        animatorSpec.Play(anim);
    }

    //hide next ball sprite
    public void PlayHideAnimation()
    {
        animator.Play("BallDestroy");
    }
}
