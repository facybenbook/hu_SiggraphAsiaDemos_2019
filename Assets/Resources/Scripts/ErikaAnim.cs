using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikaAnim : MonoBehaviour
{
    Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            anim.CrossFade("DeepBreath",1.0f);
        }
        
        if(Input.GetKeyDown(KeyCode.S))
        {
            anim.CrossFade("ShallowBreath",1.0f);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            anim.CrossFade("QuickBreath",1.0f);
        }
    }
}
