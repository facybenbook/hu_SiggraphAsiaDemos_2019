using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AttackAnimationEvent : StateMachineBehaviour
{
    const int damage = 1;
    float attack_interval = 0.5f;

    bool attacked_flag = false;
    float timer = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        //clip_length = clips[0].clip.length;  
        if(!attacked_flag)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControll>().Damaged(damage);
            attacked_flag=true;
        }
        if(attacked_flag)
        {
            timer += Time.deltaTime;
            if(timer>attack_interval)
            {
                attacked_flag=false;
                timer = 0;
            }
        }
    }

    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
