using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    Camera camer;

    Transform playerTransform;
    Vector2 hitPos;
    Vector2 targetPos;
    bool getDestination;
    [SerializeField]
    int lerpSpeed;
    PlayerAction playerAction;

    [SerializeField]
    LineRenderer lineEditor;

    int moveIndex;
    void Awake()
    {
        playerAction = GetComponent<PlayerAction>();
        playerTransform = transform;
        getDestination = true;
    }
    void Start()
    {
        playerTransform.position = MapNavigation.Instance.GetPlayerInitPos(playerTransform.position);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            hitPos = camer.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(hitPos);
            getDestination = false;
            MapNavigation.Instance.SetMovePoint(hitPos,playerTransform.position);
            moveIndex = 1;
            int length = MapNavigation.Instance.MovePoint.Count;
            if ((MapNavigation.Instance.MovePoint[length -1].x - playerTransform.position.x)*playerTransform.localScale.x<0)
            {
                playerTransform.localScale = new Vector2(-playerTransform.localScale.x,playerTransform.localScale.y);
            }


            Vector3[] test = new Vector3[length];
            for (int i = 0; i < test.Length; i++)
            {
                Vector2 a = MapNavigation.Instance.MovePoint[i];
                test[i] = new Vector3(a.x, a.y, -1);
            }
            lineEditor.positionCount = test.Length;
            lineEditor.SetPositions(test);
            //targetPos = MapNavigation.Instance.GetRightTargetPos(hitPos, playerTransform.position);
            //Debug.Log(MapNavigation.Instance.targetStep.maxY);



        }
#if UNITY_EDITOR
        //Debug.DrawLine(targetPos + Vector2.up*0.5f, targetPos + Vector2.down * 0.5f, Color.red);
        //Debug.DrawLine(targetPos + Vector2.right * 0.5f, targetPos + Vector2.left * 0.5f, Color.red);

#endif
        if (!getDestination)
        {
            if(MapNavigation.Instance.MovePoint[moveIndex].y != MapNavigation.Instance.MovePoint[moveIndex-1].y)
            {
                playerAction.SetPlayerAnimation(PlayerState.Climb);
            }
            else
            {
                playerAction.SetPlayerAnimation(PlayerState.Run);
            } 
            playerTransform.position = Vector2.MoveTowards(playerTransform.position, MapNavigation.Instance.MovePoint[moveIndex], lerpSpeed * Time.deltaTime);
            if ((Vector2)playerTransform.position == MapNavigation.Instance.MovePoint[moveIndex])
            {
                moveIndex++;
                if(moveIndex == MapNavigation.Instance.MovePoint.Count)
                {
                    getDestination = true;
                }
                
            }
        }
        else
        {
            playerAction.SetPlayerAnimation(PlayerState.Idel);
        }


    }

    void UpdatePlayerAnimator()
    {

    }
}
