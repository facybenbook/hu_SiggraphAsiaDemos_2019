using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movSpeed = 2.3f;
    public int rotSpeed = 5;

    // Some private variables.
    private bool isDead = false;
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion rotInitial;
    private GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.GetComponent<PlayerStatus>().enemy;
        // Set this initial Rotation.
        rotInitial = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead)
        {
            controller = GetComponent<CharacterController>();

            if(enemy){
                // Autorotate to the enemy.
                Quaternion newRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(enemy.transform.position - transform.position), rotSpeed * Time.deltaTime);
                newRotation.x = rotInitial.x;
                newRotation.z = rotInitial.z;
                this.transform.rotation = newRotation;
            }
            
            // Walk movement
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= movSpeed;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    void Dead(){
        isDead = true;
    }
}
