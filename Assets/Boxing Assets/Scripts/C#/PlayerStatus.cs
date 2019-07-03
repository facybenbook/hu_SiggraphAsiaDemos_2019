using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public GameObject enemy;
    public float life = 100f;
    public float stamina = 100f;
    int staminaRecuperationFactor = 10; // If you increment this, it will gain stamina faster.

    // Some private variables.
    private bool isDead = false;
    private bool canRegenerateStamina = true;

    // Variables to access to others scripts.
    private PlayerAnimation playerAnimationScript;
    private PlayerCombat playerCombatScript;
    private PlayerMovement playerMovementScript;
    private LevelManager levelManagerScript;

    private bool isCovered = false;
        // Start is called before the first frame update
    void Start()
    {
        // Set external scripts.
        playerAnimationScript = transform.GetComponent<PlayerAnimation>();
        playerCombatScript = transform.GetComponent<PlayerCombat>();
        playerMovementScript = transform.GetComponent<PlayerMovement>();
        levelManagerScript = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        
	if(enemy==null){
		Debug.LogWarning("WARNING: You must set one enemy (the PC character) for this script in the Inspector View!");
	    }
    }

    // Update is called once per frame
    void Update()
    {
        // if player is not dead, regenerate stamina.
        if(!isDead){
            RegenerateStamina();	
        }
    }

    // This function apply damage for this player.
    void Damage(float amount){
	
        float totalAmount;
        
        if(isCovered){
            totalAmount = amount / 5;
        }else{
            totalAmount = amount;
            // If the player was damaged, add one successful hit to the enemy in LevelManager.js.
            levelManagerScript.AddSuccessfulHit("enemy");
        }
            
        life -= totalAmount;
        
        if(life <= 0){
            life = 0;
            isDead = true;
            gameObject.SendMessage("Dead");
        }
    }  

    // this function reduce the stamina.
    // Is called externally from PlayerCombat.js.
    public IEnumerator LoseStamina(float cantidad){
	stamina -= cantidad;
	if(stamina < 0){
		stamina = 0;
		canRegenerateStamina = false;
		yield return new WaitForSeconds(3f);
		canRegenerateStamina = true;
	}
}

    // this function regenerate stamina every frame.
    // Is called in Update function.
    void RegenerateStamina(){
        if(canRegenerateStamina){
            stamina += Time.deltaTime * staminaRecuperationFactor;
            stamina = Mathf.Clamp(stamina,0,100);
        }
    }

    void Covered(){
        isCovered = true;
    }

    void Uncovered(){
        isCovered = false;
    }

    // Player Dead.
    // Informs to LevelManager.js that the player was Knockout.
    void Dead(){
        isDead = true;
        levelManagerScript.KO("player");
    }
}
