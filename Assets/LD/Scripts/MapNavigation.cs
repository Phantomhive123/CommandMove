using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerBehaviour
{
    stop = 0,
    climb = 1,
    fall = 2
}


public class MapNavigation : MonoBehaviour{

    private static MapNavigation instance;
    public static MapNavigation Instance
    {
        get
        {
            return instance;
        }
    }
    //[SerializeField]
    //List<FlatBoxStep> step;

    //[SerializeField]
    //List<Chasm> chasm;

    [SerializeField]
    public float maxClimbHeight;
    [SerializeField]
    public float playerHalfWidth;
    [SerializeField]
    public float playerHalfHeight;
    [SerializeField]
    public float maxFallHeight;
    //LandformItem[] landform;

    //float[] stepX;
    // List<FlatBoxStep> relatedStep = new List<FlatBoxStep>();//点击的可能相关台阶
    //FlatBoxStep targetStep;
    //float[] itemX;

    //FlatBoxStep hitRelatedStep;
    //Chasm hitRelatedChasm;

    [HideInInspector]
    public List<Vector2> MovePoint;

    [SerializeField]
    List<LandformInterval> interval;

    [SerializeField]
    List<LandformInterval> maskInterval;



    void Awake()
    {
        instance = this;

        // SortItemX();
        SortInterval();

        //test
        //for (int i = 0; i < interval.Count; i++)
        //{
        //    Debug.Log("minX:" + interval[i].minX + " maxX:" + interval[i].maxX + " y:" + interval[i].y);
        //}

    }

    //public Vector2 GetRightTargetPos(Vector2 hitPos,Vector2 playerPos)
    //{
    //    targetStep = null;
    //    Vector2 rightPos = Vector2.up*3;
    //    for(int index = 0; index < steps.Count; index++)
    //    {
    //        if(hitPos.x >= steps[index].minX && hitPos.x <= steps[index].maxX)
    //        {
    //            relatedStep.Add(steps[index]);
    //        }
    //    }

    //    if(relatedStep.Count == 0)
    //    {
    //        rightPos = new Vector2(hitPos.x, playerPos.y);
    //    }
    //    else if(relatedStep.Count == 1)//这个其实可以省略，下面包含了这种情况
    //    {
    //        if (hitPos.y >= relatedStep[0].maxY)
    //        {
    //            rightPos = new Vector2(hitPos.x, relatedStep[0].maxY + 0.7f);//加主角的半身长
    //            targetStep = relatedStep[0];
    //        }            
    //        else
    //            rightPos = new Vector2(hitPos.x, playerPos.y);
    //    }
    //    else
    //    {
    //        int rightIndex = MathCalulate.GetExactlyLessIndex(relatedStep, hitPos.y);
    //        if (rightIndex == -1)
    //        {
    //            rightPos = new Vector2(hitPos.x, playerPos.y);
    //        }
    //        else
    //        {
    //            rightPos = new Vector2(hitPos.x, relatedStep[rightIndex].maxY + 0.7f);
    //            targetStep = relatedStep[rightIndex];
    //        }
    //    }
    //    relatedStep.Clear();
    //    return rightPos;
    //}


    //void SortItemX()
    //{
    //    landform = new LandformItem[step.Count + chasm.Count];
    //    int i = 0;
    //    for(; i < step.Count; i++)
    //    {
    //        landform[i] = step[i];
    //    }
    //    for(int j = 0; j < chasm.Count; j++)
    //    {
    //        landform[i + j] = chasm[j];
    //    }


    //    for (int times = 0; times < landform.Length - 1; times++)
    //    {
    //        for (int index = 0; index < landform.Length - times - 1; index++)
    //        {
    //            if (landform[index].minX > landform[index + 1].minX)
    //            {
    //                LandformItem max = landform[index];
    //                landform[index] = landform[index + 1];
    //                landform[index + 1] = max;
    //            }
    //        }
    //    }

    //    itemX = new float[landform.Length * 2];
    //    for (int index = 0; index < itemX.Length; index++)
    //    {
    //        itemX[index] = landform[index / 2].minX;
    //        itemX[++index] = landform[(index - 1) / 2].maxX;
    //    }
    //}

   
    //public void SetMovePoint(Vector2 hitPos, Vector2 playerPos)
    //{
    //    MovePoint.Clear();
    //    hitRelatedStep = null;
    //    hitRelatedChasm = null;

    //    MovePoint.Add(playerPos);
    //    int hitInterval = MathCalulate.GetInsertIndexOfMin(itemX, hitPos.x);
    //    int playerInterval = MathCalulate.GetInsertIndexOfMin(itemX, playerPos.x);
       
    //    if(playerInterval <= hitInterval)
    //    {
    //        while (playerInterval < hitInterval)
    //        {
    //            if (playerInterval % 2 == 0)
    //            {
    //                if (landform[playerInterval / 2].type == ItemType.highStep)//主角现在在高台阶
    //                {
    //                    hitRelatedStep = (FlatBoxStep)landform[hitInterval / 2];
    //                }
    //                else if (landform[playerInterval / 2].type == ItemType.lowChasm)//主角现在在低坑
    //                {
    //                    hitRelatedChasm = (Chasm)landform[hitInterval / 2];
    //                }

    //            }
    //            else //主角在平地
    //            {
    //                MovePoint.Add(new Vector2(landform[(playerInterval + 1) / 2].minX,playerPos.y));
    //                //判断平地右边的物体
    //                if(landform[(playerInterval+1)/2].type == ItemType.highStep)
    //                {
    //                    hitRelatedStep = (FlatBoxStep)landform[(playerInterval + 1) / 2];
    //                    if(hitRelatedStep.maxY < playerPos.y )//如果台阶低于主角
    //                    {
    //                        if(playerPos.y-hitRelatedStep.maxY > maxFallHeight)
    //                        {
    //                            return;
    //                        }

    //                    }
    //                    else//否则高于主角位置
    //                    {
    //                        if(hitRelatedStep.maxY - playerPos.y > maxClimbHeight)//不爬上去
    //                        {
    //                            return;
    //                        }
    //                    }
    //                }
    //                else if(landform[(playerInterval + 1) / 2].type == ItemType.lowChasm)
    //                {

    //                }
    //            }
    //            playerInterval++;
    //        }
    //        //主角来到了最终区间

    //        if (playerInterval % 2 == 0)
    //        {
    //            if (landform[playerInterval / 2].type == ItemType.highStep)//高台阶
    //            {
    //                hitRelatedStep = (FlatBoxStep)landform[hitInterval / 2];
    //                if(hitPos.y >= hitRelatedStep.maxY)//鼠标点击的是台阶上方
    //                {
    //                    float y = hitRelatedStep.maxY + playerHalfHeight;
    //                    MovePoint.Add(new Vector2(hitRelatedStep.minX, hitRelatedStep.maxY + playerHalfHeight));
    //                    MovePoint.Add(new Vector2(hitPos.x, y));
    //                    return;
    //                }
    //            }
    //            else if (landform[playerInterval / 2].type == ItemType.lowChasm)//低坑
    //            {
    //                hitRelatedChasm = (Chasm)landform[hitInterval / 2];
    //            }

    //        }
    //    }
    //    else
    //    {
    //        while (playerInterval > hitInterval)
    //        {
    //            playerInterval++;
    //        }
    //    }
        
       


        

    //}



    public Vector2 GetPlayerInitPos(Vector2 playerPos)
    {
        return new Vector2(playerPos.x, interval[GetLocateInterval(playerPos.x)].y + playerHalfHeight);
    }

    void SortInterval()
    {
        for (int times = 0; times < interval.Count - 1; times++)
        {
            for (int index = 0; index < interval.Count - times - 1; index++)
            {
                if (interval[index].minX > interval[index + 1].minX)
                {
                    LandformInterval max = interval[index];
                    interval[index] = interval[index + 1];
                    interval[index + 1] = max;
                }
            }
        }
    }

    int GetLocateInterval(float posX)
    {
        int index = -1;
        for(int i = 0;i < interval.Count;i++)
        {
            if(posX >= interval[i].minX && posX <= interval[i].maxX)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public void SetMovePoint(Vector2 hitPos,Vector2 playerPos)
    {
        //清空
        MovePoint.Clear();

        MovePoint.Add(GetPlayerInitPos(playerPos));//第一个元素添加主角初始位置

        int playerInterval = GetLocateInterval(playerPos.x);
        int hitInterval = GetLocateInterval(hitPos.x);
        if(playerInterval <= hitInterval)//往右走
        {
            while(playerInterval < hitInterval)
            {
                if (!TryGoNextInterval(playerInterval, playerInterval+1, playerPos.y))
                    return;
                playerInterval++;
            }        
        }
        else//往左走
        {
            while(playerInterval > hitInterval)
            {
                if (!TryGoNextInterval(playerInterval, playerInterval - 1, playerPos.y))
                    return;
                playerInterval--;
            }
        }
        //到了最终区间
        MovePoint.Add(new Vector2(hitPos.x, interval[hitInterval].y + playerHalfHeight));
    }

   bool TryGoNextInterval(int fromIndex, int toIndex,float playerPosY)
   {
        bool canAccess = false;

        float targetX;
        float returnX = 0;
        if (fromIndex < toIndex)
        {
            if(interval[fromIndex].y < interval[toIndex].y)
            {
                targetX = interval[toIndex].minX - playerHalfWidth;
                returnX = playerHalfWidth;
            }
            else
            {
                targetX = interval[toIndex].minX;
            }
           
        }
        else
        {
            if(interval[fromIndex].y < interval[toIndex].y)
            {
                targetX = interval[toIndex].maxX + playerHalfWidth;
                returnX = -playerHalfWidth;
            }
            else
            {
                targetX = interval[toIndex].maxX;
            }        
        }
        MovePoint.Add(new Vector2(targetX, interval[fromIndex].y+playerHalfHeight));//不管能不能过去都能到达此区间末尾位置

        targetX += returnX;
        float height = interval[toIndex].y - interval[fromIndex].y;

        if((height >= 0 && height <= maxClimbHeight)||
            (height < 0 && height >= -maxFallHeight))//能爬上去或者能跳下去
        {
            canAccess = true;    
            MovePoint.Add(new Vector2(targetX, interval[toIndex].y + playerHalfHeight));
        }

        return canAccess;

   }
}
