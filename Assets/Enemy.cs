using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameTile currentTile;


    public void PlayRound(){
        if(GameManager.gameOver){
            return;
        }
        GameTile playerTile = PlayerController.instance.currentTile;
        int playerDiffX = Mathf.Abs(playerTile.x-currentTile.x);
        int playerDiffY = Mathf.Abs(playerTile.y-currentTile.y);
        if((playerDiffX == 1 && playerDiffY == 0) || (playerDiffX == 0 && playerDiffY == 1)){
            StartCoroutine(PlayAttackAnimation(PlayerController.instance.currentTile));
            return;
        }

        GameTile newTile = null;
        //Move
        if(playerDiffX >= playerDiffY){
            if(playerTile.x > currentTile.x){
                newTile = GameManager.instance.GetGridSlot(currentTile.x+1,currentTile.y);
                if(newTile != null){
                    if(newTile.occupyingObject == null){
                        StartCoroutine(MoveToTile(newTile));
                        return;
                    }
                }
            }
            else{
                newTile = GameManager.instance.GetGridSlot(currentTile.x-1,currentTile.y);
                if(newTile != null){
                    if(newTile.occupyingObject == null){
                        StartCoroutine(MoveToTile(newTile));
                        return;
                    }
                }
            }
        }
        if(playerTile.y > currentTile.y){
            newTile = GameManager.instance.GetGridSlot(currentTile.x,currentTile.y+1);
            if(newTile != null){
                if(newTile.occupyingObject == null){
                    StartCoroutine(MoveToTile(newTile));
                    return;
                }
            }
        }
        else{
            newTile = GameManager.instance.GetGridSlot(currentTile.x,currentTile.y-1);
            if(newTile != null){
                if(newTile.occupyingObject == null){
                    StartCoroutine(MoveToTile(newTile));
                    return;
                }
            }
        }


        TurnOver();
    }


    public void Kill(){
        Destroy(gameObject);
    }

    public void AttackedByPlayer(bool fromArrow = false){
        GetComponent<Animator>().enabled = true;
        currentTile.RemoveObjectFromTile();
        if(!fromArrow){
            PlayerController.instance.AddArrow();
        }
        GameManager.instance.KilledEnemy(this);
        GameManager.instance.IncrementScore();
    }

    IEnumerator MoveToTile(GameTile destinationTile){
        float elapsedTime = 0f;
        float waitTime = 0.18f;
        Vector2 source = transform.position;
        Vector2 destination = destinationTile.gameObject.transform.position;
        while (elapsedTime < waitTime){
            transform.position = Vector2.Lerp(source,destination, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentTile.RemoveObjectFromTile();
        currentTile = destinationTile;
        destinationTile.SetObjectToTile(gameObject);
        TurnOver();
    }

    private void TurnOver(){
        GameManager.instance.EnemyTurnOver();
    }


    IEnumerator PlayAttackAnimation(GameTile destinationTile){
        float elapsedTime = 0f;
        float waitTime = 0.08f;
        Vector2 source = transform.position;
        Vector2 destination = destinationTile.gameObject.transform.position;
        while (elapsedTime < waitTime){
            transform.position = Vector2.Lerp(source,destination , (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PlayerController.instance.DamagePlayer();

        elapsedTime = 0f;
        while (elapsedTime < waitTime){
            transform.position = Vector2.Lerp(destination, source , (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        TurnOver();
    }


}
