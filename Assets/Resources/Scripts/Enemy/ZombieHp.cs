using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHp : MonoBehaviour
{
    float hp;
    Vector3 initScale;
    // Start is called before the first frame update
    void Start()
    {
        initScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        hp = this.transform.parent.GetComponent<ZombieControll>().Get_Hp_percent();
        if(hp<0)
            hp=0;
        this.transform.localScale = new Vector3(hp,initScale.y,initScale.z);
    }
}
