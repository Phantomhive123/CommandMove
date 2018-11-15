using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheAnimationCurve : MonoBehaviour {

    private bool isGoingRight = true;
    private Vector3 originPosition;
    //用于计时
    private float timer;

    /*两条动画曲线*/
    public AnimationCurve climbAnimCurve;
    public AnimationCurve jumpAnimCurve;

    [SerializeField]
    private bool jumpFlag = false;
    [SerializeField]
    private bool climbFlag = false;
    [SerializeField]
    private float jumpAnimLength;
    [SerializeField]
    private float climbAnimLength;
	
	// Update is called once per frame
	void Update () {

        if (climbFlag == true)
        {
            timer += Time.deltaTime;
            Climb();
        }
        if (jumpFlag == true)
        {
            timer += Time.deltaTime;
            Jump();
        }
    }

    /// <summary>
    /// 播放攀爬动画的入口
    /// </summary>
    /// <param name="direction"></param>
    public void ClimbAnim(bool direction)
    {
        isGoingRight = direction;
        climbFlag = true;
        originPosition = transform.position;
    }

    /// <summary>
    /// 播放跳跃动画的入口
    /// </summary>
    /// <param name="direction"></param>
    public void JumpAnim(bool direction)
    {
        isGoingRight = direction;
        jumpFlag = true;
        originPosition = transform.position;
    }

    /// <summary>
    /// 攀爬动作完成的主体方法
    /// </summary>
    private void Climb()
    {
        if(isGoingRight)
            transform.position = new Vector3(1.1f * timer / climbAnimLength, climbAnimCurve.Evaluate(1.1f * timer / climbAnimLength), 0) + originPosition;
        else
            transform.position = new Vector3(-1.1f * timer / climbAnimLength, climbAnimCurve.Evaluate(1.1f * timer / climbAnimLength), 0) + originPosition;
        if (timer > climbAnimLength)
        {
            timer -= climbAnimLength;
            climbFlag = false;
            //回调移动脚本的方法，告知动作执行完毕
            GetComponent<CommandMove>().EndAnim(originPosition);
        }
    }

    /// <summary>
    /// 跳跃动作完成的主体方法
    /// </summary>
    private void Jump()
    {
        if (isGoingRight)
            transform.position = new Vector3(1.1f * timer / jumpAnimLength, jumpAnimCurve.Evaluate(1.1f * timer / jumpAnimLength), 0) + originPosition;
        else
            transform.position = new Vector3(-1.1f * timer / jumpAnimLength, jumpAnimCurve.Evaluate(1.1f * timer / jumpAnimLength), 0) + originPosition;
        if (timer > jumpAnimLength)
        {
            timer -= jumpAnimLength;
            jumpFlag = false;
            //回调移动脚本的方法，告知动作执行完毕
            GetComponent<CommandMove>().EndAnim(originPosition);
        }
    }
}
