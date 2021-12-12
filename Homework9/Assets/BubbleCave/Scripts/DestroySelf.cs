using UnityEngine;

public class DestroySelf : MonoBehaviour {

    [Range(0, 10f)]
    public float destroyAfter;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, destroyAfter);
	}
	
}
