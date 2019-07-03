using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControll : MonoBehaviour
{
    public Slider hp_slider;

    const int MaxHp = 100;
    int Hp = MaxHp;

    void Update()
    {
        hp_slider.value = (float)Hp/(float)MaxHp;
        if(Hp<10)
        {
            //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake();
            GameObject.FindGameObjectWithTag("GameController").GetComponent<EnvController>().GameOver();
        }
    }

    public void Damaged(int damage)
    {
        Hp -= damage;
    }




}
