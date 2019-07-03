using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    Animation animation;
    private bool isDead = false;
    public bool walking = false;
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

    // Update is called once per frame
    void Update()
    {	
        CharacterController controller = GetComponent<CharacterController>();
        if(!isDead){
            if(walking){
                if(controller.velocity.z < 0){
                    animation.CrossFade("avanzar",0.1f);
                }
                else if(controller.velocity.z > 0){
                    animation.CrossFade("caminar_atras",0.1f);
                }
            }else{
                animation.CrossFade("idle",0.1f);
            }
        }else{
            animation.CrossFade("muerte",0.1f);
            this.enabled = false;
        }
    }

    public void Hit (string attackType) {
        animation.CrossFadeQueued(attackType,0.1f,QueueMode.PlayNow);
    }

    void Impact (string hitType) {
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
        animation.CrossFade("cubrirse",0.1f);
    }

    void Uncovered () {
        animation.CrossFade("descubrirse",0.1f);
    }

    void Dead () {
        isDead = true;
    }
}
