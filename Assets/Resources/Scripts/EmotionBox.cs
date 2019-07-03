using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionBox : MonoBehaviour
{
    GameObject effect;
    public GameObject globalController;
    public GameObject PoweUpWarning;

    void Start()
    {
        effect = this.transform.Find("Effect").gameObject;
    }
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log ("Trigger entered");
        effect.SetActive(true);
        if(this.gameObject.name=="Fear" && globalController!=null)
        {
            globalController.GetComponent<EnvController>().ChangeLight2Relax();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(this.gameObject.name=="Fear" && globalController!=null)
        {
            globalController.GetComponent<EnvController>().BulletPower = 1000;
            if(PoweUpWarning!=null)
                PoweUpWarning.SetActive(true);
        }
        else
        {
            globalController.GetComponent<EnvController>().BulletPower = 1;
        }

    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log ("Trigger exit");
        effect.SetActive(false);
        if(this.gameObject.name=="Fear" && globalController!=null)
        {
            if(PoweUpWarning!=null)
                PoweUpWarning.GetComponent<Animation>().Play("ExpandOut");
        }
    }

}
