using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGenerator : MonoBehaviour
{
    public GameObject[] zombiePrefabs;
    public GameObject mainCharacter;

    public int zombieNum = 10;
    public float radius = 10f;
    public float distance2char = 10f; 

    GameObject[] zombies;

    void Start()
    {
        zombies = new GameObject[zombieNum];
        Vector3[] randomPoses = RandomPositions(mainCharacter.transform.position+mainCharacter.transform.forward*distance2char,radius,zombieNum);
        for(int i=0;i<zombieNum;i++)
        {
            int rand_type = (int)Random.Range(0,zombiePrefabs.Length-1);
            zombies[i] = GameObject.Instantiate(zombiePrefabs[rand_type],randomPoses[i],Quaternion.Euler(Vector3.zero));
            if(zombies[i].GetComponent<ZombieControll>()!=null)
                zombies[i].GetComponent<ZombieControll>().SetTarget(mainCharacter.transform);
        }
    }

    Vector3[] RandomPositions(Vector3 center, float radius,int num)
    {
        Vector3[] randomPositions = new Vector3[num];
        for(int i=0;i<num;i++)
        {
            Vector3 randPos = new Vector3(Random.Range(center.x-radius,center.x+radius),
            0.0f,Random.Range(center.z-radius,center.z+radius));
            randomPositions[i] = randPos;
        }
        return randomPositions;
    }

    void Update()
    {
    }

}
