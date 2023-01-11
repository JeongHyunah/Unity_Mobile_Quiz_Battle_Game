using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slidingTile : MonoBehaviour
{
    public Vector2 targetPosition;
    private Vector2 correctPosition;
    private SpriteRenderer sprite;
    public int number;
    public bool inRightPlace;

    public Vector2 getCorrectPosition()
    {
        return correctPosition;
    }

    void Awake()
    {
        targetPosition = transform.position;
        correctPosition = transform.position;
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(a: transform.position, b: targetPosition, t: 1);
        if (targetPosition == correctPosition)
        {
            sprite.color = Color.green;
            inRightPlace = true;
        }
        else
        {
            sprite.color = Color.white;
            inRightPlace = false;
        }
    }
}
