using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GUIResult : MonoBehaviour
{
    private int currentSecond = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Clock",1,1);
    }

    void onGUI()
    {
        GUI.Box(new Rect(10,10,(Screen.width / 2),Screen.height - 20),"RESULTS");

        if(currentSecond > 1){
            GUI.Label(new Rect(20,50,(Screen.width / 2),Screen.height - 20),"Player Total Hits: " + LevelManager.playerTotalHits);
        }
            
        if(currentSecond > 2){
            GUI.Label(new Rect(20,70,(Screen.width / 2),Screen.height - 20),"Player Successful Hits: " + LevelManager.playerSuccessfulHits);
        }
        
        if(currentSecond > 3){
            GUI.Label(new Rect(20,90,(Screen.width / 2),Screen.height - 20),"Player Effectivity: " + LevelManager.playerEffectivity + "%");
        }
        
        if(currentSecond > 5){
            GUI.Label(new Rect(20,140,(Screen.width / 2),Screen.height - 20),"PC AI Total Hits: " + LevelManager.enemyTotalHits);
        }
            
        if(currentSecond > 6){
            GUI.Label(new Rect(20,160,(Screen.width / 2),Screen.height - 20),"PC AI Successful Hits: " + LevelManager.enemySuccessfulHits);
        }
        
        if(currentSecond > 7){
            GUI.Label(new Rect(20,180,(Screen.width / 2),Screen.height - 20),"PC AI Effectivity: " + LevelManager.enemyEffectivity + "%");
        }
        
        if(currentSecond > 9){
            if(LevelManager.isKnockOut){
                GUI.Label(new Rect(20,220,(Screen.width / 2),Screen.height - 20),"WINNER: " + LevelManager.winner + " by Knockout!");
            }else{
                GUI.Label(new Rect(20,220,(Screen.width / 2),Screen.height - 20),"WINNER: " + LevelManager.winner + " by Points!");
            }
            
        }
        
        GUI.enabled = false;
        
        if(currentSecond > 10){
            GUI.enabled = true;
        }
        
        if(GUI.Button(new Rect(20,Screen.height - 80,(Screen.width / 2) - 30,60),"Back to Main Menu")){
            Application.LoadLevel("intro");
        }
    }

    void Clock()
    {
        currentSecond++;
    }
}
