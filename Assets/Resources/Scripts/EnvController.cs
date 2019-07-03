using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
public class EnvController : MonoBehaviour
{
    public int BulletPower=1;
    public Light _light;
    public Color colorScare = Color.white;
     public Color colorRelax = Color.red;
     public float duration = 5f;
     Color lerpedColor = Color.white;
 
     private float t=0;
     private bool flag; 
     private string state="Relax";

     bool gameOver = false;
     public GameObject redCanvas;

     // Use this for initialization
     void Start () {
         _light.color = colorRelax;
     }

    void Update() {
        if (state == "Scare" && flag)
        {
            lerpedColor = Color.Lerp(colorRelax, colorScare, t);
            _light.GetComponent<Light>().color = lerpedColor;
            t += Time.deltaTime / duration;
            if (t > 0.99f)
                flag = false;
        }
        else if (state == "Relax" && flag)
        {
            lerpedColor = Color.Lerp(colorRelax, colorScare, t);
            _light.color = lerpedColor;
            t -= Time.deltaTime / duration;
            if (t < 0.01f)
                flag = false;
        }

        if (gameOver && redCanvas!=null)
         {
             redCanvas.SetActive(true);
             GameObject.FindGameObjectWithTag("Weapon").GetComponent<GunScript>().GameOver();
             //camera shake for 4 seconds
             CameraShaker.Instance.ShakeOnce(2f,3f,2.3f,2.3f);
         }

     }


     public void ChangeLight2Relax()
     {
            state = "Scare";
            flag=true;
     }


     public void ChangeLight2Scare()
     {
            state="Relax";
            flag=true;
     }


     public void GameOver()
     {
         gameOver = true;
     }
}
