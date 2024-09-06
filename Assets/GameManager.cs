using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    void Awake(){
        if(GameManager.instance != null){
            //Destroy(this);
        }
        GameManager.instance = this;
        gameOver = false;
        playersTurn = false;
    }
    public static bool gameOver;

    public static GameManager instance;
    public Camera cam;
    public GameObject gameTile;
    public GameObject enemy;


    public List<Sprite> grassVariationTiles = new List<Sprite>();
    public List<Sprite> stones = new List<Sprite>();

    public GameTile[,] grid;

    public int boardSizeX = 6;
    public int boardSizeY = 6;

    public int turn = 0;

    public static bool playersTurn;

    int enemyTurnIndex = 0;

    private List<Enemy> enemyList = new List<Enemy>();

    private float timeToDecideStart = 3f;
    private float timeToDecide = 5f;


    public Image timerImage;
    public TMP_Text scoreText;
    public TMP_Text arrowCount;
    public int score = 0;

    public Image playerHealth1;
    public Image playerHealth2;
    public Image playerHealth3;

    public Sprite playerEmptyHealth;


    // Start is called before the first frame update
    void Start()
    {
        grid = new GameTile[boardSizeX+1,boardSizeY+1];
        for(int x = -1; x <= boardSizeX+1; x++){
            for(int y = -1; y <= boardSizeY+1; y++){
                if(x == -1 || x == boardSizeX+1 || y == -1 || y == boardSizeY+1){
                    GameObject obj = Instantiate(gameTile,new Vector2(-boardSizeX + x,-boardSizeY + y), Quaternion.identity);
                    obj.GetComponent<GameTile>().SetSprite(stones[Random.Range(0,stones.Count-1)]);
                }
                else{
                    GameObject obj = Instantiate(gameTile,new Vector2(-boardSizeX + x,-boardSizeY + y), Quaternion.identity);
                    grid[x,y] = obj.GetComponent<GameTile>();
                    grid[x,y].x = x;
                    grid[x,y].y = y;
                    if(Random.Range(0,5) == 0){
                        obj.GetComponent<GameTile>().SetSprite(grassVariationTiles[Random.Range(0,grassVariationTiles.Count-1)]);
                    }
                }
            }
        }

        SpawnPlayerAndSetCameraToCenter();
        SpawnEnemy();
        PlayerTurnStart();
    }

    public void UpdateArrowCount(){
        arrowCount.text = PlayerController.instance.arrows.ToString();
    }

    public void GameOver(){
        gameOver = true;
        playersTurn = false;
    }

    public IEnumerator DecisionTimer(){
        float currentAmount = 1f;
        float lerp = 0f;
        while(playersTurn && currentAmount > 0f && !gameOver){
            float startValue = 1f;
            float endValue = 0f; 
            lerp += Time.deltaTime / timeToDecide;
            currentAmount = Mathf.Lerp (startValue, endValue, lerp);
            timerImage.fillAmount = currentAmount;
            yield return null;
        }
        if(currentAmount == 0f){
            if(PlayerController.instance.DamagePlayer()){
                GameOver();
            }
            else{
                PlayerTurnOver();
            }
        }
        
    }

    public void UpdatePlayerHealth(){
        if(PlayerController.instance.health == 3){

        }
        else if(PlayerController.instance.health == 2){
            playerHealth3.sprite = playerEmptyHealth;
        }
        else if(PlayerController.instance.health == 1){
            playerHealth3.sprite = playerEmptyHealth;
            playerHealth2.sprite = playerEmptyHealth;
        }
        else{
            playerHealth3.sprite = playerEmptyHealth;
            playerHealth2.sprite = playerEmptyHealth;
            playerHealth1.sprite = playerEmptyHealth;
        }
    }

    public void IncrementScore(){
        score++;
        scoreText.text = score.ToString();
    }

    public void KilledEnemy(Enemy enemy){
        if(enemyList.Contains(enemy)){
            enemyList.Remove(enemy);
        }
    }
    void StartEnemyTurn(){
        if(enemyList.Count > 0){
            enemyList[enemyTurnIndex].PlayRound();
        }
        else{
            PlayerTurnStart();
        }
    }

    public GameTile GetArrowTileFromThisDirection(GameTile source, string direction){

        if(direction == "Up"){
            for(int y = source.y; y <= boardSizeY; y++){
                if(grid[source.x,y].occupyingObject != null && grid[source.x,y].occupyingObject.GetComponent<Enemy>() != null){
                    return grid[source.x,y];
                }
            }
            
        }
        else if(direction == "Left"){
            for(int x = source.x; x >= 0; x--){
                if(grid[x,source.y].occupyingObject != null && grid[x,source.y].occupyingObject.GetComponent<Enemy>() != null){
                    return grid[x,source.y];
                }
            }
        }
        else if(direction == "Down"){
            for(int y = source.y; y >= 0; y--){
                if(grid[source.x,y].occupyingObject != null && grid[source.x,y].occupyingObject.GetComponent<Enemy>() != null){
                    return grid[source.x,y];
                }
            }
        }
        else if(direction == "Right"){
            for(int x = source.x; x <= boardSizeX; x++){
                if(grid[x,source.y].occupyingObject != null && grid[x,source.y].occupyingObject.GetComponent<Enemy>() != null){
                    return grid[x,source.y];
                }
            }
        }
        return null;
    }
    public Vector2 GetArrowTileFromThisDirectionOuterTile(GameTile source, string direction){

        if(direction == "Up"){
            return ((Vector2)(grid[source.x,boardSizeY].transform.position) + new Vector2(0,1));
        }
        else if(direction == "Left"){
            return ((Vector2)(grid[0,source.y].transform.position) + new Vector2(-1,0));
        }
        else if(direction == "Down"){
            return ((Vector2)(grid[source.x,0].transform.position) + new Vector2(0,-1));
        }
        else if(direction == "Right"){
            return ((Vector2)(grid[boardSizeX,source.y].transform.position) + new Vector2(1,0));
        }
        return Vector2.zero;
    }

    public void EnemyTurnOver(){
        if(gameOver){return;}
        enemyTurnIndex++;
        if(enemyList.Count >= enemyTurnIndex+1){
            enemyList[enemyTurnIndex].PlayRound();
        }
        else{
            PlayerTurnStart();
        }
    }

    public void RestartGame(){
        SceneManager.LoadScene("SampleScene");
    }
    public GameTile GetGridSlot(int x, int y){
        if(x < 0 || y < 0){
            return null;
        }
        if(x > boardSizeX || y > boardSizeY){
            return null;
        }
        return grid[x,y];
    }


    public void PlayerTurnOver(){
        if(gameOver){ return;}
        playersTurn = false;
        turn++;
        enemyTurnIndex = 0;
        if(turn % 3 == 0){
            SpawnEnemy();
        }
        StartEnemyTurn();
    }

    public void PlayerTurnStart(){
        if(gameOver){ return;}
        playersTurn = true;
        timeToDecide = Mathf.Clamp(timeToDecideStart-(turn * 0.5f),1f,5f);
        StartCoroutine(DecisionTimer());
        PlayerController.instance.StartPlayersTurn();
    }

    public string DetermineActionForDestinationTile(int x, int y, bool player){
        if(x < 0 || y < 0){
            return "takeDamage";
        }
        if(x > boardSizeX || y > boardSizeY){
            return "takeDamage";
        }

        GameTile destinationTile = grid[x,y];
        if(destinationTile.occupyingObject == null){
            return "move";
        }
        else{
            return "attack";
        }
    }

    void SpawnEnemy(){
        Vector2 playerPos = new Vector2(PlayerController.instance.currentTile.x,PlayerController.instance.currentTile.y);
        int i = 0;
        while(i < 100){
            Vector2 enemyPos = new Vector2(Random.Range(0,boardSizeX),Random.Range(0,boardSizeY));
            if(Vector2.Distance(enemyPos,playerPos) > 1.8f){
                if(grid[(int)enemyPos.x,(int)enemyPos.y].occupyingObject == null){
                    GameTile targetGrid = grid[(int)enemyPos.x,(int)enemyPos.y];
                    GameObject obj = Instantiate(enemy,targetGrid.transform.position,Quaternion.identity);
                    targetGrid.SetObjectToTile(obj);
                    obj.GetComponent<Enemy>().currentTile = targetGrid;
                    enemyList.Add(obj.GetComponent<Enemy>());
                    break;
                }
            }
            i++;
        }
    }

    void SpawnPlayerAndSetCameraToCenter(){
        if(!gameOver){
            cam.transform.position = grid[(int)boardSizeX/2,(int)boardSizeY/2].gameObject.transform.position;
            cam.transform.position = new Vector3(cam.transform.position.x,cam.transform.position.y,-10);
            PlayerController.instance.transform.position = grid[(int)boardSizeX/2,(int)boardSizeY/2].gameObject.transform.position;
            PlayerController.instance.currentTile = grid[(int)boardSizeX/2,(int)boardSizeY/2];
        }
    }


}
