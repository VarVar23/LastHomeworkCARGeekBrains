using UnityEngine;

public class Obstacle : MonoBehaviour
{
    //trigger when ball hit collision cave
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Ball")
        {
            GameManager.Instance.GameOver();

        }
    }
}
