using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieControll : MonoBehaviour
{
    public GameObject explosionEffet;
    const float crawl_speed = 0.5f;
    const float walk_speed = 0.6f;
    const float run_speed = 1.0f;
    const float max_delay = 2;
    string[] zombie_states = new string[3]{"Run","Crawl","Walk"};
    float[] attack_choice = new float[4]{0.25f,0.5f,0.75f,1.0f};
    string state = "Idle";
    const int zombie_maxHP = 10;
    int zombie_hp = zombie_maxHP;
    Animator _anim;
    // Start is called before the first frame update

    Transform target; //always move toward the target
    float rotationSpeed = 0.8f;
    float maxDistance;

    float moveSpeed=0;
    float speedNoise;

    float start_delay;
    //variables for trace target
    Vector3 targetPosition;

    bool isAttacking;
    void Start()
    {
        _anim = this.GetComponent<Animator>();
        moveSpeed = 0;
        speedNoise = Random.Range(-0.1f,0.5f);
        StartCoroutine(ZombieStartMoving());
    }

    IEnumerator ZombieStartMoving()
    {
        start_delay = Random.Range(0,max_delay);
        yield return new WaitForSeconds(start_delay);
        state = zombie_states[(int)Random.Range(0f,3f)];
        _anim.SetBool(state,true);
    }


    // Update is called once per frame
    void Update()
    {
        switch(state){
            case "Crawl": 
                moveSpeed = crawl_speed+speedNoise;break;
            case "Walk": 
                moveSpeed = walk_speed+speedNoise;break;
            case "Run": 
                moveSpeed = run_speed+speedNoise;break;
            case "Idle":
                moveSpeed = 0;break;
            case "Die":
                moveSpeed = 0;break;
        }
        if(target!=null)
            FollowTarget();
        if(zombie_hp<=0)
        {   
            ZombieDie();
        }
    }

    public void SetTarget(Transform _target)
    {
        this.target = _target;
    }

    public void FollowTarget()
    {
        Debug.DrawLine (target.position, this.transform.position, Color.red);//在玩家跟怪物中间画一条直的红线方便查看
        //设置怪物转身,正面始终朝向玩家
        targetPosition = new Vector3 (target.position.x, 0, target.position.z);//得到怪物脚下xz坐标
        this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation(targetPosition - this.transform.position), rotationSpeed * Time.deltaTime);//挂物转身朝向玩家
            
        //设置怪物想玩家移动
        maxDistance = Vector3.Distance(targetPosition, this.transform.position);//获取玩家与怪物之间的距离
        if(maxDistance >= 2)
        {
                //当距离大于两米时移动
                this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;//让怪物朝着自己的正面移动
        }
        else
        {
            if(isAttacking)
                return;
            //当距离小于两米时的动作
            _anim.SetBool("Attack",true);
            int rand_choice = (int)Random.Range(0f,4f);
            _anim.SetFloat("attack_type",attack_choice[rand_choice]);
            isAttacking = true;
        }

    }

    public void ZombieDie()
    {
        state = "Die";
        moveSpeed = 0;
        _anim.SetBool("Die",true);
    }

    public void Destroyself()
    {
        Destroy(this.gameObject);
    }

    public float Get_Hp_percent()
    {
        return ((float)zombie_hp/(float)zombie_maxHP);
    }

    public void shoot_zombie(int bullet_power)
    {
        zombie_hp -= bullet_power;
        if(bullet_power>50)
        {
            GameObject effect = GameObject.Instantiate(explosionEffet,this.gameObject.transform);
            effect.transform.parent = this.transform;
            effect.transform.position = Vector3.zero;
        }
    }
}


