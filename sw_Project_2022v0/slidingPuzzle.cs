using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class slidingPuzzle : MonoBehaviour
{
    [SerializeField] private Transform emptyObject = null;
    private Camera camera;
    [SerializeField] private slidingTile[] tiles;
    private int emptyObjectIndex = 15;

    void Start()
    {
        camera = Camera.main;
        Shuffle();
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if(hit)
            {
                if (Vector2.Distance(a: emptyObject.position, b: hit.transform.position) < 121)
                {
                    try
                    {
                        Vector2 lastEmptySpacePosition = emptyObject.position;
                        slidingTile thistile = hit.transform.GetComponent<slidingTile>();
                        emptyObject.position = thistile.targetPosition;
                        thistile.targetPosition = lastEmptySpacePosition;
                        int tileIndex = findIndex(thistile);
                        tiles[emptyObjectIndex] = tiles[tileIndex];
                        tiles[tileIndex] = null;
                        emptyObjectIndex = tileIndex;
                    }
                    catch
                    {
                    }
                }  
            }
        }
        int correctTiles = 0;
        foreach(var a in tiles)
        {
            if(a != null)
            {
                if (a.inRightPlace)
                    correctTiles++;
            }
        }
        if(correctTiles == tiles.Length - 1)
        {
            SceneManager.LoadScene("7_txtQuiz");
        }
    }

    public void Shuffle()
    {
        for (int i = 0; i < 15; i++)
        {
            if (tiles[i] != null)
            {
                var lastPos = tiles[i].targetPosition;
                int randomIndex = Random.Range(0, 14);
                tiles[i].targetPosition = tiles[randomIndex].targetPosition;
                tiles[randomIndex].targetPosition = lastPos;
                var tile = tiles[i];
                tiles[i] = tiles[randomIndex];
                tiles[randomIndex] = tile;
            }
        }
    }

    public int findIndex(slidingTile ts)
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                if (tiles[i] == ts)
                {
                    return i;
                }
            }
        }
        return -1;
    }
}
