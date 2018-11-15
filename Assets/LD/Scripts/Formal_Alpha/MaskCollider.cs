using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskCollider : MonoBehaviour {


    [SerializeField]
    List<BoxCollider2D> insideLandform;
    [SerializeField]
    List<BoxCollider2D> outsideLandform;

    Rectangle[] insideBox;
    Rectangle[] outsideBox;

    void Awake()
    {
        insideBox = new Rectangle[insideLandform.Count];
        outsideBox = new Rectangle[outsideLandform.Count];
        InitRectangle();
    }


    void InitRectangle()
    {
        for(int i = 0;i<insideBox.Length;i++)
        {
            Bounds colliderBounds = insideLandform[i].bounds;
            insideBox[i].minX = colliderBounds.min.x;
            insideBox[i].minY = colliderBounds.min.y;
            insideBox[i].maxX = colliderBounds.max.x;
            insideBox[i].maxY = colliderBounds.max.y;
        }
        for(int i= 0;i<outsideBox.Length;i++)
        {

        }
    }

    public void UpdateLandformCollider(Rectangle maskRectangle)
    {

    }

    void Update()
    {
#if UNITY_EDITOR
        for (int i = 0; i < insideBox.Length; i++)
        {
            Debug.DrawLine(new Vector2(insideBox[i].minX, insideBox[i].minY), new Vector2(insideBox[i].minX, insideBox[i].maxY), Color.red);
            Debug.DrawLine(new Vector2(insideBox[i].minX, insideBox[i].maxY), new Vector2(insideBox[i].maxX, insideBox[i].maxY), Color.red);
            Debug.DrawLine(new Vector2(insideBox[i].maxX, insideBox[i].maxY), new Vector2(insideBox[i].maxX, insideBox[i].minY), Color.red);
            Debug.DrawLine(new Vector2(insideBox[i].maxX, insideBox[i].minY), new Vector2(insideBox[i].minX, insideBox[i].minY), Color.red);
        }
#endif
    }
}
