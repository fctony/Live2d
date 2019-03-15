using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;

/// <summary>
/// Boss脚本
/// </summary>
public class Live2dSampleModel : MonoBehaviour {

    public TextAsset modelFile;
    public Texture2D texture;
    public TextAsset idleMotionFile;
    public GameObject missCui;

    private Live2DModelUnity live2DModel;
    private Matrix4x4 live2DCanvasPos;

    private Live2DMotion live2DMotionIdle;
    private MotionQueueManager motionQueueManager;

    private EyeBlinkMotion eyeBlinkMotion;

    public float moveSpeed;

    private Vector3 initPos;


    private int hitCount;

    //判断当前boss是否被打败
    public bool isDefeat;



	// Use this for initialization
	void Start () {
        Live2D.init();
        live2DModel = Live2DModelUnity.loadModel(modelFile.bytes);
        live2DModel.setTexture(0, texture);
        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvasPos = Matrix4x4.Ortho(0,modelWidth,modelWidth,0,-50.0f,50.0f);

        live2DMotionIdle = Live2DMotion.loadMotion(idleMotionFile.bytes);

        live2DMotionIdle.setLoop(true);
        motionQueueManager = new MotionQueueManager();
        eyeBlinkMotion = new EyeBlinkMotion();
        motionQueueManager.startMotion(live2DMotionIdle);
        initPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);
        motionQueueManager.updateParam(live2DModel);
        eyeBlinkMotion.setParam(live2DModel);

        live2DModel.update();
        if (GameManager.Instance.gameOver)
        {
            return;
        }
        //判断当前Boss是否追赶上翠花
        if ((missCui.transform.position.x-transform.position.x)<3)
        {
            GameManager.Instance.gameOver = true;
        }

        if (isDefeat)
        {
            transform.position = Vector3.Lerp(transform.position,initPos,0.2f);
        }
        else
        {
            transform.Translate(Vector3.right*moveSpeed*Time.deltaTime);
        }
	}

    private void OnRenderObject()
    {
        live2DModel.draw();
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.gameOver)
        {
            return;
        }
        if (hitCount>=20)
        {
            isDefeat = true;
            GameManager.Instance.DefeatBadBoy();
        }
        else
        {
            hitCount++;
        }
    }
}
