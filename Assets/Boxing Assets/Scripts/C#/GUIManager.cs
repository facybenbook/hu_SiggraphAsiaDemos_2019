using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GUIManager : MonoBehaviour
{

    public Texture2D barTexture;

    private LevelManager levelManagerScript;
    private PlayerStatus playerStatusScript;
    private AiScript enemyStatusScript;

    // Start is called before the first frame update
    void Start()
    {
        levelManagerScript = transform.GetComponent<LevelManager>();
        playerStatusScript = levelManagerScript.player.GetComponent<PlayerStatus>();
        enemyStatusScript = levelManagerScript.enemy.GetComponent<AiScript>();
    }

    // Update is called once per frame
    void onGUI()
    {
        GUI.depth = 1;

        var widthLifeBar = (Screen.width / 2) - 10;
        var widthStaminaBar = (Screen.width / 2) - 10;

        if(enemyStatusScript){
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(10, 10, widthLifeBar * enemyStatusScript.life / 100  , 15), barTexture);
            GUI.color = Color.white;
            
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(10, 30, widthStaminaBar * enemyStatusScript.stamina / 100  , 8), barTexture);
            GUI.color = Color.white;
        }
        
        if(playerStatusScript){
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(widthLifeBar + 20, 10, (widthLifeBar - 10) * playerStatusScript.life / 100  , 15), barTexture);
            GUI.color = Color.white;
            
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(widthStaminaBar + 20, 30, (widthStaminaBar - 10) * playerStatusScript.stamina / 100  , 8), barTexture);
            GUI.color = Color.white;
        }
    }
}
