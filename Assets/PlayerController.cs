using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    void Awake(){
        if(PlayerController.instance != null){
            //Destroy(this);
        }
        PlayerController.instance = this;
    }

    public static PlayerController instance;

    public GameTile currentTile;

    public Vector2 pos;

    public int health = 3;
    public int arrows = 5;

    private bool actionInProgress;
    private string facingDirection = "Down";

    private Animator anim;

    public GameObject arrowPrefab;

    public void StartPlayersTurn(){
        actionInProgress = false;
    }

    public GameObject restartButton;

    void Start(){
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gameOver){
            return;
        }
        if(!GameManager.playersTurn){
            return;
        }
        if(Input.GetKey("w")){
            MovePlayer("Up");
        }
        else if(Input.GetKey("a")){
            MovePlayer("Left");
        }
        else if(Input.GetKey("s")){
            MovePlayer("Down");
        }
        else if(Input.GetKey("d")){
            MovePlayer("Right");
        }
        else if(Input.GetKey("space")){
            TryShootBow();
        }
    }

    public void AddArrow(){
        if(arrows < 100){
            arrows++;
        }
        GameManager.instance.UpdateArrowCount();
    }

    void TryShootBow(){
        if(actionInProgress){
            return;
        }
        actionInProgress = true;
        if(arrows <= 0){ actionInProgress = false; return;}

        arrows--;
        GameManager.instance.UpdateArrowCount();

        GameManager.playersTurn = false;
        anim.SetTrigger("Bow");

    }

    public void EnableRestartButton(){
        restartButton.SetActive(true);
    }

    void MovePlayer(string direction){
        if(actionInProgress){
            return;
        }
        actionInProgress = true;
        int nextX = currentTile.x;
        int nextY = currentTile.y;
        if(direction == "Up"){
            nextY++;
        }
        else if(direction == "Left"){
            nextX--;
        }
        else if(direction == "Down"){
            nextY--;
        }
        else if(direction == "Right"){
            nextX++;
        }

        string action = GameManager.instance.DetermineActionForDestinationTile(nextX,nextY,true);
        if(action == "move"){
            GameManager.playersTurn = false;
            PlayMoveAnimation(direction);
            StartCoroutine(MoveToTile(GameManager.instance.grid[nextX,nextY]));
        }
        else if(action == "attack"){
            GameManager.playersTurn = false;
            PlayMoveAnimation(direction);
            StartCoroutine(PlayAttackAnimation(GameManager.instance.grid[nextX,nextY]));
        }
        else if(action == "takeDamage"){
            actionInProgress = false;
        }
    }

    void PlayMoveAnimation(string direction){
        facingDirection = direction;
        if(direction == "Up"){
            anim.Play("WalkUp");
        }
        else if(direction == "Left"){
            anim.Play("WalkLeft");
        }
        else if(direction == "Down"){
            anim.Play("WalkDown");
        }
        else if(direction == "Right"){
            anim.Play("WalkRight");
        }
    }

    public bool DamagePlayer(){
        health--;
        GameManager.instance.UpdatePlayerHealth();
        if(health <= 0){
            GameManager.instance.GameOver();
            anim.SetTrigger("Death");
            return true;
        }
        return false;
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
        GameManager.instance.PlayerTurnOver();
    }

    public void SpawnArrow(){
        GameObject obj = Instantiate(arrowPrefab,transform.position,Quaternion.identity);
        GameTile desTile = GameManager.instance.GetArrowTileFromThisDirection(currentTile,facingDirection);
        Vector2 des = Vector2.zero;
        if(desTile == null){
            des = GameManager.instance.GetArrowTileFromThisDirectionOuterTile(currentTile,facingDirection);
        }
        else{
            des = desTile.transform.position;
        }

        obj.GetComponent<Arrow>().Shoot(facingDirection, desTile, des);
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

        destinationTile.occupyingObject.GetComponent<Enemy>().AttackedByPlayer();


        elapsedTime = 0f;
        while (elapsedTime < waitTime){
            transform.position = Vector2.Lerp(destination, source , (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        GameManager.instance.PlayerTurnOver();
    }

    void CheckDeath(){

    }

}
