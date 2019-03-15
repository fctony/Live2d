using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
using live2d.framework;

public class Live2dModel : MonoBehaviour {

    //模型
    public TextAsset modelFile;
    public Texture2D[] textures;
    private Live2DModelUnity live2DModel;
    private Matrix4x4 live2DCanvasPos;
    //动作
    public int motionIndex;
    public TextAsset[] motionFiles;
    //private Live2DMotion live2DMontionIdle;
    //动作对象
    //1,2.Idel  3.左摆头 4.右摆头微笑 5.前弯腰较生气 6.不开心闭眼 7,8.不开心睁眼 
    //9.放电 10 蒙蔽卖萌 11.哭 12.掐腰生气 13.恶心不舒服 14.摇来摇去
    private Live2DMotion[] motions;
    //优先级
    //L2DMotionManager继承自MotionQueueManager
    //优先级的设置标准：
    //1.动作未进行的状态，优先级为0。
    //2.待机动作发生时，优先级为1。
    //3.其他动作进行时，优先级为2。
    //4.无视优先级，强制发生的动作，优先级为3。                               
    private L2DMotionManager l2DMotionManager;
    //动作管理
    private MotionQueueManager motionQueueManager;
    private MotionQueueManager motionQueueManagerA;

    //自动眨眼
    private EyeBlinkMotion eyeBlinkMotion;

    //鼠标拖拽引起的动作变化
    //管理拖拽坐标
    private L2DTargetPoint drag;


    //物理运算的设定
    private PhysicsHair physicsHairSideLeft;
    private PhysicsHair physicsHairSideRight;
    private PhysicsHair physicsHairBackLeft;
    private PhysicsHair physicsHairBackRight;

    //表情
    public TextAsset[] expressionFiles;
    public L2DExpressionMotion[] expressions;
    private MotionQueueManager expresionMotionQueueManager;

    public float a;

    // Use this for initialization
    void Start () {

        //初始化
        Live2D.init();

        //释放
        //Live2D.dispose();

        //读取模型
        //Live2DModelUnity.loadModel(Application.dataPath+ "/Resources/Epsilon/runtime/Epsilon.moc");
        //TextAsset mocFile = Resources.Load<TextAsset>("Epsilon/runtime/Epsilon.moc");

        live2DModel=Live2DModelUnity.loadModel(modelFile.bytes);

        //与贴图建立关联
        //Texture2D texture2D1 = Resources.Load<Texture2D>("Epsilon/runtime/Epsilon.1024/texture_00");
        //Texture2D texture2D2 = Resources.Load<Texture2D>("Epsilon/runtime/Epsilon.1024/texture_01");
        //Texture2D texture2D3 = Resources.Load<Texture2D>("Epsilon/runtime/Epsilon.1024/texture_02");
        //live2DModel.setTexture(0,texture2D1);
        //live2DModel.setTexture(1, texture2D2);
        //live2DModel.setTexture(2, texture2D3);
        for (int i = 0; i < textures.Length; i++)
        {
            live2DModel.setTexture(i, textures[i]);
        }

        //指定显示位置与尺寸（使用正交矩阵与相关API显示图像，再由游戏物体的位置和摄像机的size调整图像到合适的位置）
        float modelWidth = live2DModel.getCanvasWidth();

        live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50, 50);

        //播放动作
        //实例化动作对象
        //live2DMontionIdle = Live2DMotion.loadMotion(Application.dataPath+"");
        //TextAsset mtnFile = Resources.Load<TextAsset>("");
        //live2DMontionIdle= Live2DMotion.loadMotion(mtnFile.bytes);
        motions = new Live2DMotion[motionFiles.Length];
        for (int i = 0; i < motions.Length; i++)
        {
            motions[i] = Live2DMotion.loadMotion(motionFiles[i].bytes);
        }
        //设置某一个动画的一些属性
        //重复播放不淡入。
        motions[0].setLoopFadeIn(false);
        //设置淡入淡出时间，参数单位为毫秒
        motions[0].setFadeOut(1000);
        motions[0].setFadeIn(1000);
        //动画是否循环播放
        //motions[0].setLoop(true);

        //motionQueueManager = new MotionQueueManager();
        //motionQueueManager.startMotion(motions[0]);

        ////播放多个动作
        //motions[5].setLoop(true);

        //motionQueueManagerA = new MotionQueueManager();
        //motionQueueManagerA.startMotion(motions[5]);

        //动作的优先级使用
        l2DMotionManager = new L2DMotionManager();

        //眨眼
        eyeBlinkMotion = new EyeBlinkMotion();

        //鼠标拖拽
        drag = new L2DTargetPoint();

        
        #region 左右两侧头发的摇摆
        //左测旁边的头发
        physicsHairSideLeft = new PhysicsHair();
        //套用物理运算 
        physicsHairSideLeft.setup(0.2f, // 长度 ： 单位是公尺　影响摇摆周期(快慢)
                        0.5f, // 空气阻力 ： 可设定0～1的值、预设值是0.5　影响摇摆衰減的速度
                        0.14f); // 质量 ： 单位是kg　
        //设置输入参数
        //设置哪一个部分变动时进行哪一种物理运算
        PhysicsHair.Src srcXLeft = PhysicsHair.Src.SRC_TO_X;//横向摇摆

        //第三个参数，"PARAM_ANGLE_X"变动时头发受到0.005倍的影响度的输入参数
        physicsHairSideLeft.addSrcParam(srcXLeft, "PARAM_ANGLE_X",0.005f,1);

        //设置输出表现
        PhysicsHair.Target target = PhysicsHair.Target.TARGET_FROM_ANGLE;//表现形式
         
        physicsHairSideLeft.addTargetParam(target, "PARAM_HAIR_SIDE_L",0.005f,1);


        //右侧旁边的头发
        physicsHairSideRight = new PhysicsHair();
        //套用物理运算 
        physicsHairSideRight.setup(0.2f, // 长度 ： 单位是公尺　影响摇摆周期(快慢)
                        0.5f, // 空气阻力 ： 可设定0～1的值、预设值是0.5　影响摇摆衰減的速度
                        0.14f); // 质量 ： 单位是kg　
        //设置输入参数
        //设置哪一个部分变动时进行哪一种物理运算
        PhysicsHair.Src srcXRight = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        //PhysicsHair.Src srcXRight = PhysicsHair.Src.SRC_TO_Y;

        //第三个参数，"PARAM_ANGLE_X"变动时头发受到0.005倍的影响度的输入参数
        physicsHairSideRight.addSrcParam(srcXRight, "PARAM_ANGLE_X", 0.005f, 1);

        //设置输出表现
        PhysicsHair.Target targetRight = PhysicsHair.Target.TARGET_FROM_ANGLE;//表现形式

        physicsHairSideRight.addTargetParam(targetRight, "PARAM_HAIR_SIDE_R",0.005f,1);

        #endregion

        #region 左右后边头发的摇摆
        //左边
        physicsHairBackLeft = new PhysicsHair();
        physicsHairBackLeft.setup(0.24f, 0.5f, 0.18f);

        PhysicsHair.Src srcXBackLeft = PhysicsHair.Src.SRC_TO_X;
        PhysicsHair.Src srcZBackLeft = PhysicsHair.Src.SRC_TO_G_ANGLE;

        physicsHairBackLeft.addSrcParam(srcXBackLeft, "PARAM_ANGLE_X",0.005f,1);
        physicsHairBackLeft.addSrcParam(srcZBackLeft, "PARAM_ANGLE_Z",0.8f,1);

        PhysicsHair.Target targetBackLeft = PhysicsHair.Target.TARGET_FROM_ANGLE;

        physicsHairBackLeft.addTargetParam(targetBackLeft, "PARAM_HAIR_BACK_L", 0.005f, 1);

        //右边
        physicsHairBackRight = new PhysicsHair();
        physicsHairBackRight.setup(0.24f, 0.5f, 0.18f);

        PhysicsHair.Src srcXBackRight = PhysicsHair.Src.SRC_TO_X;
        PhysicsHair.Src srcZBackRight = PhysicsHair.Src.SRC_TO_G_ANGLE;

        physicsHairBackRight.addSrcParam(srcXBackRight, "PARAM_ANGLE_X", 0.005f, 1);
        physicsHairBackRight.addSrcParam(srcZBackRight, "PARAM_ANGLE_Z", 0.8f, 1);

        PhysicsHair.Target targetBackRight = PhysicsHair.Target.TARGET_FROM_ANGLE;

        physicsHairBackRight.addTargetParam(targetBackRight, "PARAM_HAIR_BACK_R", 0.005f, 1);

        #endregion

        //表情
        expresionMotionQueueManager = new MotionQueueManager();
        expressions = new L2DExpressionMotion[expressionFiles.Length];
        for (int i = 0; i < expressions.Length; i++)
        {
            expressions[i] = L2DExpressionMotion.loadJson(expressionFiles[i].bytes);
        }
    }

    // Update is called once per frame
    void Update () {

        live2DModel.setMatrix(transform.localToWorldMatrix*live2DCanvasPos);

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    motionIndex++;
        //    if (motionIndex>=motions.Length)
        //    {
        //        motionIndex = 0;
        //    }
        //    motionQueueManager.startMotion(motions[motionIndex]);
        //}

        //motionQueueManager.updateParam(live2DModel);
        //motionQueueManagerA.updateParam(live2DModel);

        if (Input.GetKeyDown(KeyCode.M))
        {
            motionIndex++;
            if (motionIndex >= expressions.Length)
            {
                motionIndex = 0;
            }
            expresionMotionQueueManager.startMotion(expressions[motionIndex]);
        }

        expresionMotionQueueManager.updateParam(live2DModel);


        //判断待机动作
        //if (l2DMotionManager.isFinished())
        //{
        //    StartMotion(0,1);
        //}
        //else if (Input.GetKeyDown(KeyCode.M))
        //{
        //    StartMotion(14,2);
        //}
        //l2DMotionManager.updateParam(live2DModel);

        //设置参数
        //live2DModel.setParamFloat("PARAM_ANGLE_X",1);

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    live2DModel.addToParamFloat("PARAM_ANGLE_X", a);
        //}

        //live2DModel.multParamFloat("PARAM_ANGLE_X", a);
        ////也可以通过获取索引去设置参数
        //int paramAngleX;
        //paramAngleX = live2DModel.getParamIndex("PARAM_ANGLE_X");
        //live2DModel.setParamFloat(paramAngleX,30);

        ////参数的保存与回复
        //live2DModel.setParamFloat("PARAM_ANGLE_X", 30);
        ////保存与回复的参数是整个模型的所有参数，并不只是之前同方法里设置的某几个参数
        //live2DModel.saveParam();
        //live2DModel.loadParam();

        //设定模型某一部分的不透明度。
        //live2DModel.setPartsOpacity("PARTS_01_FACE_001", 0);

        //模型跟随鼠标转向与看向
        //得到的Live2d鼠标检测点的比例值是-1到1（对应一个live2d拖拽
        //管理坐标系，或者叫做影响度。）
        //然后我们通过这个值去设置我们的参数，比如旋转30度*当前得到的值
        //就会按照这个值所带来的影响度去影响我们的模型动作
        //从而到达看向某一个点的位置
        Vector3 pos = Input.mousePosition;//屏幕坐标
        if (Input.GetMouseButton(0))
        {
            drag.Set(pos.x/Screen.width*2-1,pos.y/Screen.height*2-1);
        }

        else if (Input.GetMouseButtonUp(0))
        {
            drag.Set(0, 0);
        }

        //参数及时更新，考虑加速度等自然因素，计算坐标，进行逐帧更新。
        drag.update();

        //模型转向
        if (drag.getX()!=0)
        {
            live2DModel.setParamFloat("PARAM_ANGLE_X",30*drag.getX());
            live2DModel.setParamFloat("PARAM_ANGLE_Y", 30 * drag.getY());
            live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", 10 * drag.getX());
            live2DModel.setParamFloat("PARAM_EYE_BALL_X",-drag.getX());
            live2DModel.setParamFloat("PARAM_EYE_BALL_Y",-drag.getY());
        }

        //眨眼
        eyeBlinkMotion.setParam(live2DModel);

        long time = UtSystem.getUserTimeMSec();//执行时间

        physicsHairSideLeft.update(live2DModel,time);
        physicsHairSideRight.update(live2DModel,time);
        physicsHairBackLeft.update(live2DModel, time);
        physicsHairBackRight.update(live2DModel,time);

        live2DModel.update();
    }

    private void OnRenderObject()
    {
        live2DModel.draw();
    }

    private void StartMotion(int motionIndex,int priority)
    {
        if (l2DMotionManager.getCurrentPriority()>= priority)
        {
            return; 
        }
        l2DMotionManager.startMotion(motions[motionIndex]);
    }
}
