using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiScript : MonoBehaviour
{

    public GameObject player;

    public enum enemyState {offensive, defensive, idle}

    public float life = 100;
    public float stamina = 100;

    public float movSpeed = 2.3f;
    public int rotSpeed = 5;

    public int staminaRecuperationFactor;

    private bool canRegenerateStamina = true;

    private enemyState currentState;
    private AiAnimation iaAnimationScript;

    public float damageJab;
    public float damageCross;
    public float damageUpperLeft;
    public float damageUpperRight;

    private bool canAttack = true;
    float attackRatio;
    private float timeForNextAttack;

    private bool canChangeState = true;
    float changeStateRatio;
    private float timeForNextState;

    private bool canChangeAction = true;
    float actionRatio;
    private float timeForNextAction;

    private bool hit = false;

    private string hitType;

    private bool isDead = false;
    private float damageCaused;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion rotInitial;
    private LevelManager levelManagerScript;

    public bool isCovered = false;
    // Audio Vars
    AudioSource audio;
    public AudioClip attackMissed;
    public AudioClip attackLeft;
    public AudioClip attackRight;
    // Start is called before the first frame update
    void Start()
    {
        audio = this.GetComponent<AudioSource>();
        if(!player){
		    Debug.LogWarning("WARNING: You must set one enemy (the player character) for this script in the Inspector View!");
	    }
        levelManagerScript = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        iaAnimationScript = transform.GetComponent<AiAnimation>();
        currentState = enemyState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead){
		RegenerateStamina();	
		if(player){
			// Aplicar AutoRotacion
			Quaternion newRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), rotSpeed * Time.deltaTime);
			newRotation.x = rotInitial.x;
			newRotation.z = rotInitial.z;
            transform.rotation = newRotation;
		}
		// Verificar si el estado necesita cambiarse.
		MakeDecisions();
		// Ejecutar los comandos del estado seleccionado.
		switch(currentState){	
			case enemyState.idle:
				break;
			case enemyState.offensive:			
				Offensive();
				break;
			case enemyState.defensive:
				Defensive();
				break;
		    }
	    }
    }

    void MakeDecisions(){
        if(timeForNextState > 0){
            timeForNextState -= Time.deltaTime;
            canChangeState = false;
        }
        if(timeForNextState < 0){
            timeForNextState = 0;
            canChangeState = true;
        }
        
        if(canChangeState){
            int randomState = Random.Range(1,4);
            
            if(randomState == 1){
                currentState = enemyState.idle;
                iaAnimationScript.walking = false;
                timeForNextState += changeStateRatio;
            }
            if(randomState == 2){
                currentState = enemyState.defensive;
                iaAnimationScript.walking = true;
                timeForNextState += changeStateRatio / 2;
            }
            if(randomState == 3){
                currentState = enemyState.offensive;
                iaAnimationScript.walking = true;
                timeForNextState += changeStateRatio;
            }
        }
        
        if(timeForNextAction > 0){
            timeForNextAction -= Time.deltaTime;
            canChangeAction = false;
        }
        if(timeForNextAction < 0){
            timeForNextAction = 0;
            canChangeAction = true;
        }
        
        if(canChangeAction){
            int randomAccion = Random.Range(1,11);
            if(randomAccion < 8){
                hit = true;
                gameObject.SendMessage("Uncovered");
                isCovered = false;
            }
            else if(randomAccion < 10){
                gameObject.SendMessage("Covered");
                isCovered = true;
                timeForNextAction = 1;
            }
            else{
                hit = false;
                gameObject.SendMessage("Uncovered");
                isCovered = false;
                timeForNextAction = 1;
            }
        }
        
        if(timeForNextAttack > 0){
            timeForNextAttack -= Time.deltaTime;
            canAttack = false;
        }
        if(timeForNextAttack < 0){
            timeForNextAttack = 0;
            canAttack = true;
        }
        
        if(hit){
            if(canAttack){
                var distOponente = Vector3.Distance(transform.position, player.transform.position);
                    
                if(distOponente < 3){
                    Attack();
                    timeForNextAction = actionRatio;
                }				
            }
        }
    }

    void Covered(){
        isCovered = true;
    }

    void Uncovered(){
        isCovered = false;
    }

    void Offensive(){

        controller = GetComponent<CharacterController>();
            
        moveDirection = new Vector3(0, 0, 1);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= movSpeed;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void Defensive(){

        controller = GetComponent<CharacterController>();
            
        moveDirection = new Vector3(0, 0, -1);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= movSpeed;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void Attack () {
        float randomGolpe = Random.Range(1,5);
        
        if(randomGolpe == 1)
            hitType = "jab";
        if(randomGolpe == 2)
            hitType = "cross";
        if(randomGolpe == 3)
            hitType = "uppercutleft";
        if(randomGolpe == 4)
            hitType = "uppercutright";
        
        iaAnimationScript.Hit(hitType);
        timeForNextAttack += attackRatio;
        LoseStamina(5);
        float dist = Vector3.Distance(transform.position, player.transform.position);
        
        if(hitType == "jab"){
            if(dist < 1.95){
                damageCaused = stamina * damageJab / 100;
                player.SendMessage("Damage",damageCaused);
                player.SendMessage("Impact",hitType);
                LoseStamina(2);
                audio.PlayOneShot(attackLeft);
            }else{
                audio.PlayOneShot(attackMissed);
            }
        }
        if(hitType == "cross"){
            if(dist < 1.95){
                damageCaused = stamina * damageCross / 100;
                player.SendMessage("Damage",damageCaused);
                player.SendMessage("Impact",hitType);
                LoseStamina(2);
                audio.PlayOneShot(attackRight);
            }else{
                audio.PlayOneShot(attackMissed);
            }
        }
        if(hitType == "uppercutleft"){
            if(dist < 1.85){
                damageCaused = stamina * damageUpperLeft / 100;
                player.SendMessage("Damage",damageCaused);
                player.SendMessage("Impact",hitType);
                LoseStamina(5);
                audio.PlayOneShot(attackLeft);
            }else{
                audio.PlayOneShot(attackMissed);
            }
        }
        if(hitType == "uppercutright"){
            if(dist < 1.85){
                damageCaused = stamina * damageUpperRight / 100;
                player.SendMessage("Damage",damageCaused);
                player.SendMessage("Impact",hitType);
                LoseStamina(5);
                audio.PlayOneShot(attackRight);
            }else{
                audio.PlayOneShot(attackMissed);
            }
        }
        
        levelManagerScript.AddHit("enemy");
                
    }

    void LoseLife(float amount){
        float totalAmount;
        
        if(isCovered){
            totalAmount = amount / 5;
        }else{
            totalAmount = amount;
            // If the player was damaged, add one successful hit to the enemy in LevelManager.js.
            levelManagerScript.AddSuccessfulHit("player");
        }
        life -= totalAmount;
        if(life <= 0){
            life = 0;
            isDead = true;
            gameObject.SendMessage("Dead");
        }
    }

    void Dead(){
        isDead = true;
        levelManagerScript.KO("enemy");
    }

    void Impact(){
        timeForNextAttack = 0.3f;
    }

    void RegenerateStamina(){
        if(canRegenerateStamina){
            stamina += Time.deltaTime * staminaRecuperationFactor;
            stamina = Mathf.Clamp(stamina,0,100);
        }
    }

    IEnumerator LoseStamina(float amount){
        stamina -= amount;
        if(stamina < 0){
            stamina = 0;
            canRegenerateStamina = false;
            yield return new WaitForSeconds(3f);
            canRegenerateStamina = true;
        }
    }
}
