using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效孵化器
/// </summary>
public class EffectSpawn : MonoBehaviour {

    public GameObject[] effectGos;
    public Transform canvasTrans;

	// Use this for initialization
	void Start () {
        InvokeRepeating("CreateEffectGo",0,2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateEffectGo()
    {
        int randomIndex = Random.Range(0,2);
        transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(0,45)));
        GameObject effectGo = Instantiate(effectGos[randomIndex], transform.position, transform.rotation);
        effectGo.transform.SetParent(canvasTrans);
    }
}
