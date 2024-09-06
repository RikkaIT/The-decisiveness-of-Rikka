using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{

    public GameObject occupyingObject;
    public int x;
    public int y;

    public void SetSprite(Sprite sprite){
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SetObjectToTile(GameObject obj){
        occupyingObject = obj;
    }
    public void RemoveObjectFromTile(){
        occupyingObject = null;
    }
}
