using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    /*

    This script controlls all the player animations.

    */
    public AnimationState cover;
    public AnimationState uncover;

    private bool isCovered = false;
    private bool isDead = false;

    Animation animation;

    void Start()
    {
        animation = this.GetComponent<Animation>();
        animation["jab"].layer = 1;
        animation["jab"].blendMode = AnimationBlendMode.Additive;

        animation["cross"].layer = 1;
        animation["cross"].blendMode = AnimationBlendMode.Additive;

        animation["uppercutleft"].layer = 1;
        animation["uppercutleft"].blendMode = AnimationBlendMode.Additive;

        animation["uppercutright"].layer = 1;
        animation["uppercutright"].blendMode = AnimationBlendMode.Additive;
        
        animation["cubrirse"].layer = 2;
        animation["cubrirse"].blendMode = AnimationBlendMode.Additive;
        
        animation["descubrirse"].layer = 2;
        animation["descubrirse"].blendMode = AnimationBlendMode.Additive;
        
        animation["impactoDerecho"].layer = 1;
        animation["impactoDerecho"].blendMode = AnimationBlendMode.Additive;
        
        animation["impactoIzquierdo"].layer = 1;
        animation["impactoIzquierdo"].blendMode = AnimationBlendMode.Additive;
        
        animation["impactoBajoDerecho"].layer = 1;
        animation["impactoBajoDerecho"].blendMode = AnimationBlendMode.Additive;
        
        animation["impactoBajoIzquierdo"].layer = 1;
        animation["impactoBajoIzquierdo"].blendMode = AnimationBlendMode.Additive;
    }

    void Update(){

        CharacterController controller = this.GetComponent<CharacterController>();

        if(!isDead){
            if(Input.GetAxis("Vertical") > 0){
                animation.CrossFade("avanzar",0.1f);
            }
            else if(Input.GetAxis("Vertical") < 0){
                animation.CrossFade("caminar_atras",0.1f);
            }
            else if(Input.GetAxis("Horizontal") > 0){
                animation.CrossFade("caminar_derecha",0.1f);
            }
            else if(Input.GetAxis("Horizontal") < 0){
                animation.CrossFade("caminar_izquierda",0.1f);
            }
            else if(Input.GetAxis("Vertical")==0 && Input.GetAxis("Horizontal")==0){
                animation.CrossFade("idle",0.1f);
            }
        }else{
            animation.CrossFade("muerte",0.2f);
            this.enabled = false;
        }
    }


    void Attack(string attackType) {
	    animation.Stop();
	    animation.Blend(attackType,1,0.1f);
    }

    void Impact(string hitType) {
        
        if(!isDead){
            if(hitType == "jab"){
                animation.CrossFadeQueued("impactoIzquierdo",0.1f,QueueMode.PlayNow);
            }
            if(hitType == "cross"){
                animation.CrossFadeQueued("impactoDerecho",0.1f,QueueMode.PlayNow);
            }
            if(hitType == "uppercutleft"){
                animation.CrossFadeQueued("impactoBajoIzquierdo",0.1f,QueueMode.PlayNow);
            }
            if(hitType == "uppercutright"){
                animation.CrossFadeQueued("impactoBajoDerecho",0.1f,QueueMode.PlayNow);
            }
        }
        
    }

    void Covered () {
        if(!isCovered){
            animation.Blend("cubrirse",1,0.1f);
            isCovered = true;
        }
    }

    void Uncovered () {
        if(isCovered){
            animation.CrossFade("descubrirse",0.2f);
            isCovered = false;
        }
    }

    void Dead () {
        isDead = true;
    }
}
