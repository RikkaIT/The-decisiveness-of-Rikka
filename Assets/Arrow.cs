using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private Vector2 destination;
    private Vector2 startPosition;
    private float speed = 12f;

    public void Shoot(string direction, GameTile tile, Vector2 des){
        startPosition = (Vector2)transform.position;
        if(direction == "Left"){
        }
        else if(direction == "Right"){
            gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z + 180);
        }
        else if(direction == "Up"){
            gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z + 270);
        }
        else if(direction == "Down"){
            gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z + 90);

        }
        destination = des;
        StartCoroutine(ShootArrow(tile));
    }

    private IEnumerator ShootArrow(GameTile tile){
        float fraction = 0f;
        while((Vector2)transform.position != destination){
            fraction += Time.deltaTime * speed;
            transform.position = Vector2.MoveTowards(startPosition, destination, fraction);
            //transform.position = Vector2.Lerp(startPosition,destination,fraction);
            yield return null;
        }
        if(tile != null){
            if(tile.occupyingObject.GetComponent<Enemy>() != null){
                tile.occupyingObject.GetComponent<Enemy>().AttackedByPlayer(true);
            }
        }
        else{
            GetComponent<Animator>().enabled = true;
        }
        GameManager.instance.PlayerTurnOver();
        Destroy(gameObject);
    }
}
