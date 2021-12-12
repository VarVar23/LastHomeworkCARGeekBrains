using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainManager : MonoBehaviour {

    public static ChainManager Instance { get; set; }

    [Space(10)]
    public GameObject[] parts;
    float distanceBetweenParts = 4.6f;

    [Space (10)]
    public float dragSensity = .4f;

    public GameObject firstPart; //stores most left chain part
    public GameObject lastPart; //stores most right chain part

    [Space(10)]
    public int direction;

    Vector3 mouse;
    Vector3 mouseOffset;

    // Use this for initialization
    void Start () {
        firstPart = parts[0];
        lastPart = parts[parts.Length - 1];
        direction = 1;
	}

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

    // Update is called once per frame
    void Update () {
        if (GameManager.Instance.gameState == GameState.PLAYING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouse = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                mouseOffset = (Input.mousePosition - mouse);

                UpdateChain((mouseOffset.x) * dragSensity * direction);

                mouse = Input.mousePosition;
            }
        }
    }

    //update chain parts positions
    void UpdateChain(float xDistance)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].transform.position = new Vector3(xDistance + parts[i].transform.position.x, firstPart.transform.position.y, 0);

        }

        SetFirstAndLast();

        if (firstPart.transform.position.x < -9.2f)
        {
            firstPart.transform.position = new Vector3(lastPart.transform.position.x + distanceBetweenParts, firstPart.transform.position.y, 0);
            lastPart = firstPart;

            SetFirstAndLast();
        }
        else if (lastPart.transform.position.x > 9.2f)
        {
            lastPart.transform.position = new Vector3(firstPart.transform.position.x - distanceBetweenParts, firstPart.transform.position.y, 0);
            firstPart = lastPart;

            SetFirstAndLast();
        }
    }

    //set first and last part of chain
    void SetFirstAndLast()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].transform.position.x <= firstPart.transform.position.x)
                firstPart = parts[i];
            else if (parts[i].transform.position.x >= lastPart.transform.position.x)
                lastPart = parts[i];
        }
    }

    //change chain moving direction
    public void ChangeRotation()
    {
        direction *= -1;

    }
}
