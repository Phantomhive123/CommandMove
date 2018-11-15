using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour {
    [SerializeField]
    float halfWidth;
    [SerializeField]
    float halfHeight;

    [SerializeField]
    float attachSpeed;

    Rectangle maskContour;

    Vector3 attachPos;

    bool getAttached;

    bool hitted;

    Camera camer;

    [SerializeField]
    PlayerAction playerAction;
    Transform playerTransform;

    Transform maskTransform;

    Vector2 firstHitPos;

    void Awake()
    {
        maskTransform = transform;
        playerTransform = playerAction.transform;
        hitted = false;
        getAttached = true;
        camer = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        UpdateMaskContour();
    }

    public float GetMinX()
    {
        return maskTransform.position.x - halfWidth;
    }

    public float GetMaxX()
    {
        return maskTransform.position.x + halfWidth;
    }

    public float GetMinY()
    {
        return maskTransform.position.y - halfHeight;
    }

    public float GetMaxY()
    {
        return maskTransform.position.y + halfHeight;
    }

    public void UpdateMaskContour()
    {
        maskContour.minX = GetMinX();
        maskContour.minY = GetMinY();
        maskContour.maxX = GetMaxX();
        maskContour.maxY = GetMaxY();
    }

    bool IsInRectangle(Vector2 pos)
    {
        float x = pos.x;
        float y = pos.y;
        if(x >= GetMinX() && x <= GetMaxX() && y>= GetMinY() && y <= GetMaxY())
        {
            return true;
        }
        return false;
    }

    //bool IfIntersectionWithPlayer(Vector2 pos)
    //{
    //    float minX = pos.x - halfWidth;
    //    float maxX = pos.x + halfWidth;
    //    float minY = pos.y - halfHeight;
    //    float maxY = pos.y + halfWidth;
    //    bool left = minX <= playerTransform.position.x + MapNavigation.Instance.playerHalfWidth && minX >= playerTransform.position.x - MapNavigation.Instance.playerHalfWidth;
    //    bool right = maxX <= playerTransform.position.x + MapNavigation.Instance.playerHalfWidth && maxX >= playerTransform.position.x - MapNavigation.Instance.playerHalfWidth;
    //    bool bottom = minY <= playerTransform.position.y + MapNavigation.Instance.playerHalfHeight && minY >= playerTransform.position.y - MapNavigation.Instance.playerHalfHeight;
    //    bool top = maxY <= playerTransform.position.y + MapNavigation.Instance.playerHalfHeight && maxY >= playerTransform.position.y - MapNavigation.Instance.playerHalfHeight;
    //    if (left || right || bottom || top)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
   
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if(Input.GetMouseButtonDown(0))
            {
                
                firstHitPos = camer.ScreenToWorldPoint(Input.mousePosition);
                if (!IsInRectangle(firstHitPos) || playerAction.CurrentState != PlayerState.Idel)
                    return;
                hitted = true;    
            }
            Vector2 newHitPos = camer.ScreenToWorldPoint(Input.mousePosition);
            if (!IsInRectangle(newHitPos))
                return;

            Vector2 offoset = newHitPos - firstHitPos;
            Vector2 maskAboutToPos = (Vector2)maskTransform.position + offoset;
            //if(IfIntersectionWithPlayer(maskAboutToPos))
            //{
            //    //soffoset = Vector2.zero;
            //}

            maskTransform.Translate(offoset);
            firstHitPos = newHitPos;
        }

        if(Input.GetMouseButtonUp(0) && hitted)//松开鼠标
        {
            getAttached = false;
            hitted = false;
            Vector2 halfPos = MathCalulate.GetHalfVector2(maskTransform.position);
            attachPos = new Vector3(halfPos.x, halfPos.y, -1);
        }

        if(!getAttached)//吸附到指定位置
        {
            Vector3 pos = maskTransform.position;
            maskTransform.position = Vector3.MoveTowards(pos, attachPos, Time.deltaTime * attachSpeed);
            if(maskTransform.position == attachPos)
            {
                getAttached = true;
            }
        }




    }
}

