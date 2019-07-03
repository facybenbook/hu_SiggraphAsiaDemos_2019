using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    Text hp_text ;
    Slider hp_slider; 

    // Start is called before the first frame update
    void Start()
    {
        hp_text = this.gameObject.GetComponent<Text>();
        hp_slider = this.transform.parent.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        hp_text.text = "HP:"+((int)(hp_slider.value*100)).ToString()+"%";
    }
    
}
