using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效移动
/// </summary>
public class EffectMove : MonoBehaviour {

    public float moveSpeed;
    private float timeVal;
    private int randomYPos;

	// Use this for initialization
	void Start () {
        Destroy(gameObject,10);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(-transform.right*moveSpeed*Time.deltaTime);
        if (timeVal>=1)
        {
            timeVal = 0;
            randomYPos = Random.Range(-1, 2);
        }
        else
        {
            transform.Translate(transform.up * randomYPos * Time.deltaTime * moveSpeed/5);
            timeVal += Time.deltaTime;
        }
	}
}
