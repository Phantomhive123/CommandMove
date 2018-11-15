using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 实现指令式主角寻路移动，现在最大的问题是对主角位置摆放和场景物体要求比较高
/// 1、主角位置不能够过高或者过低，脚本中没有引入重力，过高主角会悬空，过低会导致主角在地面碰撞体中触发跳跃动作
/// 2、场景中碰撞体高度也有限制
/// 解决方法：通过场景参数规格化，全部确定固定的值之后，调整该脚本参数。现阶段障碍物高度以及主角摆放位置参考示例场景。
/// </summary>
public class CommandMove : MonoBehaviour {

    [SerializeField]
    Camera mainCamera;

    //存储碰撞体信息，测试阶段有个别无用参数，后期针对功能修改
    struct ColliderInfo
    {
        public Vector2 center;
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        public Vector2 leftCenter;
        public Vector2 rightCenter;

        public float height;
        public float width;
    }

    #region fields and properties
    //待调的辅助参数
    [SerializeField][Range(0.01f,2f)]
    private float skinWidth;
    public float speed;
    [SerializeField][Range(3, 8)]  
    private int numOfHorizontalRays;
    //[SerializeField][Range(3, 8)]
    //private int numOfVerticalRays;

    //全局变量
    private Vector3 clickPoint;
    private Vector3 aim;
    private RaycastHit2D raycastHit2D;
    private bool isGoingRight;
    private bool canMoveHorizontally;
    private bool needMoveHorizontally;
    //private bool canMoveVertically;  
    //private bool needMoveVertically;
    private bool needJump = false;
    private bool canClimb = false;
    private bool isPlayingAnimation = false;
    public bool IsPlayingAnimation
    {
        get { return isPlayingAnimation; }
        set { isPlayingAnimation = value;return; }
    }
    #endregion

    #region components
    private Animator anim;
    private BoxCollider2D playerCollider;
    //private LayerMask playerLayer;
    private ColliderInfo colliderInfo;
    private TheAnimationCurve theAnimationCurve;
    #endregion

    //外部物体，debug使用
    #region external objects
    public GameObject AimSymbol;
    public GameObject ClickSymbol;
    public GameObject RayHitSymbol;
    #endregion

    #region Mono
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
        theAnimationCurve = GetComponent<TheAnimationCurve>();
        //playerLayer = gameObject.layer;

        //更新碰撞体信息，并且获取碰撞体高度和宽度
        UpdateColliderInfo();
        colliderInfo.height = 2*playerCollider.size.y;
        colliderInfo.width = 2*playerCollider.size.x;

        isGoingRight = true;
        aim = transform.position;
        AimSymbol.transform.position = aim;
	}
	
	// Update is called once per frame
	void Update () {

        UpdateColliderInfo();

        //正在播放动画，update不调用
        if (IsPlayingAnimation) return;

        //获取指令
        if(Input.GetMouseButtonDown(0))
        {
            GetClickPosition();
            RayToGround();
        }

        //执行移动策略
        MoveStrategy(Time.deltaTime * speed);
    }

    #endregion

    /// <summary>
    /// 每帧更新碰撞体位置信息
    /// </summary>
    private void UpdateColliderInfo()
    {
        var modifiedBounds = playerCollider.bounds;
        modifiedBounds.Expand(1f * skinWidth);
        colliderInfo.center = modifiedBounds.center;
        colliderInfo.topLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y - 2f * skinWidth);
        colliderInfo.topRight = new Vector2(modifiedBounds.max.x, modifiedBounds.max.y - 2f * skinWidth);
        colliderInfo.bottomLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.min.y + 2f * skinWidth);
        colliderInfo.bottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y + 2f * skinWidth);
        colliderInfo.leftCenter = new Vector2(modifiedBounds.min.x, colliderInfo.center.y);
        colliderInfo.rightCenter = new Vector2(modifiedBounds.max.x, colliderInfo.center.y);
    }

    #region get the aim position
    /// <summary>
    /// 从目标位置朝下发射射线，获取目标位置并且改变角色状态（动画、朝向）
    /// </summary>
    private void RayToGround()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(clickPoint, Vector3.down, 10f);

        if(raycastHit && raycastHit.collider.gameObject.tag == "Ground")
        {
            aim = new Vector3(clickPoint.x, raycastHit.point.y + colliderInfo.height/2, 0f);
            AimSymbol.transform.position = aim;
            RayHitSymbol.transform.position = raycastHit.point;    
        }
        else
        {
            aim = new Vector3(clickPoint.x, transform.position.y, 0f);
        }

        if (aim.x >= transform.position.x)
        {
            isGoingRight = true;
            transform.localScale = new Vector3(2, 2, 2);
        }
        else
        {
            isGoingRight = false;
            transform.localScale = new Vector3(-2, 2, 2);
        }
        if (Mathf.Abs(aim.x - transform.position.x) >= 0.05)
            anim.Play(Animator.StringToHash("Run"));
    }

    /// <summary>
    /// 获取点击坐标
    /// </summary>
    private void GetClickPosition()
    {
        clickPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        clickPoint.z = 0;
        canMoveHorizontally = true;
        needMoveHorizontally = true;
        ClickSymbol.transform.position = clickPoint;
    }
    #endregion

    #region get move strategy
    /// <summary>
    /// 移动策略，涉及到移动方法的执行顺序
    /// </summary>
    /// <param name="deltaMovement"></param>
    private void MoveStrategy(float deltaMovement)
    {
        CheckHorizontally(deltaMovement);

        //如果能够水平移动且需要水平移动，检查前方是否有深沟
        if (canMoveHorizontally && needMoveHorizontally)
            CheckDownward(deltaMovement);
        //如果不能水平移动，检测前方障碍物是否能翻过
        else if(!canMoveHorizontally && needMoveHorizontally)
            CheckUpward(deltaMovement);

        //需要竖直移动，优先竖直移动
        if (needJump)
            Jump();   
        else if(canClimb)
            Climb();
        //水平移动，不需要则不移动并播放idle动画
        else if (needMoveHorizontally && canMoveHorizontally)
            MoveHorizontally(deltaMovement);
        else
            anim.Play(Animator.StringToHash("Idle"));
    }

    /// <summary>
    /// 水平方向信息检测
    /// </summary>
    /// <param name="deltaMovement"></param>
    private void CheckHorizontally(float deltaMovement)
    {
        canMoveHorizontally = true;
        //距离目标距离近则不需要移动
        if (Mathf.Abs(aim.x - transform.position.x) < 0.05f)
            needMoveHorizontally = false;

        //发射数条水平方向射线用来检测信息
        for(int i=0;i<numOfHorizontalRays;i++)
        {
            Vector2 origin;
            if (isGoingRight)
            {
                origin = new Vector2(colliderInfo.bottomRight.x, colliderInfo.bottomRight.y + colliderInfo.height * i / (numOfHorizontalRays - 1));
                raycastHit2D = Physics2D.Raycast(origin, Vector2.right, deltaMovement/*, playerLayer*/);
                Debug.DrawRay(origin, Vector2.right, Color.red, deltaMovement);
            }
            else
            {
                origin = new Vector2(colliderInfo.bottomLeft.x, colliderInfo.bottomLeft.y + colliderInfo.height * i / (numOfHorizontalRays - 1));
                raycastHit2D = Physics2D.Raycast(origin, Vector2.left, deltaMovement/*, playerLayer*/);
                Debug.DrawRay(origin, Vector2.left, Color.red, deltaMovement);
            }

            //检测到前方有地面，则不能移动
            if (raycastHit2D && raycastHit2D.collider.gameObject.tag == "Ground")
            {
                canMoveHorizontally = false;
            }     
        }        
    }

    private void CheckDownward(float deltaMovement)
    {
        Vector2 origin;
        if (isGoingRight)
            origin = new Vector2(colliderInfo.rightCenter.x + deltaMovement, colliderInfo.rightCenter.y);
        else
            origin = new Vector2(colliderInfo.leftCenter.x - deltaMovement, colliderInfo.leftCenter.y);

        //参数待调**************************************************************************
        RaycastHit2D raycastHit = Physics2D.Raycast(origin, Vector2.down, 2f * colliderInfo.height);
        //参数待调**************************************************************************
        if(raycastHit && Mathf.Abs(colliderInfo.bottomRight.y - raycastHit.point.y) > 0.2 * colliderInfo.height)
        {
            needJump = true;
        }
        else if(!raycastHit)
        {
            //摇头动画TO DO
            canMoveHorizontally = false;
        }
    }

    private void CheckUpward(float deltaMovement)
    {
        Vector2 origin;
        RaycastHit2D raycastHit;
        if (isGoingRight)
        {
            origin = new Vector2(colliderInfo.topRight.x, colliderInfo.topRight.y + 0.5f * colliderInfo.height);
            raycastHit = Physics2D.Raycast(origin, Vector2.right, deltaMovement * 1.5f);
        }
        else
        {
            origin = new Vector2(colliderInfo.topLeft.x, colliderInfo.topLeft.y + 0.5f * colliderInfo.height);
            raycastHit = Physics2D.Raycast(origin, Vector2.left, deltaMovement * 1.5f);
        }
        
        if(!raycastHit)
        {
            canClimb = true;
        }
    }
    #endregion

    #region move function
    private void MoveHorizontally(float deltaMovement)
    {
        if (!isGoingRight)
            deltaMovement *= -1;
        transform.Translate(new Vector3(deltaMovement, 0f, 0f));
    }

    /// <summary>
    /// 处理向下跳的事件
    /// </summary>
    private void Jump()
    {
        needJump = false;
        isPlayingAnimation = true;
        anim.Play(Animator.StringToHash("NewJump"));
        theAnimationCurve.JumpAnim(isGoingRight);
    }

    /// <summary>
    /// 处理攀爬事件
    /// </summary>
    private void Climb()
    {
        canClimb = false;
        isPlayingAnimation = true;
        anim.Play(Animator.StringToHash("NewJump"));
        theAnimationCurve.ClimbAnim(isGoingRight);
    }
    
    /// <summary>
    /// 事件处理结束需要回调此方法，从而告知此脚本
    /// </summary>
    public void EndAnim(Vector3 origin)
    {
        IsPlayingAnimation = false;
        if ((origin.x - aim.x) * (aim.x - transform.position.x) > 0)
            needMoveHorizontally = false;

        if (Mathf.Abs(aim.x - transform.position.x) < 0.1)
            anim.Play(Animator.StringToHash("Idle"));
        else
            anim.Play(Animator.StringToHash("Run"));
    }
    #endregion
}