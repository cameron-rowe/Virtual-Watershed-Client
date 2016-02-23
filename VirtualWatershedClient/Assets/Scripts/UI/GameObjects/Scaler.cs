﻿using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour {

    GameObject currentController;
    public GameObject FirstPersonController;
    float dist_thresh = 150.0f;

    float OriginalHeight;
	// Use this for initialization
	void Start () {
        OriginalHeight = gameObject.transform.position.y;
        //FirstPersonController = GameObject.Find("ControlScripts");

	}
	
	// Update is called once per frame
	void Update () {
        ResizeObject();
	}

    void ResizeObject()
    {
        GameObject currentController;
        float distance;
        float yThresh, xzThresh;


        currentController = FirstPersonController;

        distance = (gameObject.transform.position - currentController.transform.position).magnitude;
        if (distance / dist_thresh < 3.14f)
        {
            yThresh = 3.14f;
        }
        else
        {
            yThresh = distance / dist_thresh;
        }

        if (distance / dist_thresh < 1.14)
        {
            xzThresh = 1.14f;
        }
        else
        {
            xzThresh = distance / dist_thresh;
        }

        //OriginalHeight
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        gameObject.transform.localScale = new Vector3(xzThresh, yThresh, xzThresh);
    }
}
