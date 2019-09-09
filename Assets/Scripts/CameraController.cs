using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    float speed;

	// Use this for initialization
	void Start () {
        speed = 50f;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(Vector3.zero);
        transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
        //transform.Translate(Vector3.left * speed * Time.deltaTime);
	}
}
