using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
    public Vector2 offsets;         //Follow position offsets;

    private Transform target;
    private Transform thisT;
    private Vector3 followPos;

	// Use this for initialization
	void Start () 
    {
        //Caching components;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        thisT = GetComponent<Transform>();
	}
	
	void LateUpdate () 
    {
        //Calculate desired follow position, depending on player's position and offsets;
        followPos = new Vector3(target.position.x + offsets.x, offsets.y, thisT.position.z);
        //Assign follow position;
        thisT.position = followPos;
	}
}
