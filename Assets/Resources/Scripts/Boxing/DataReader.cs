using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ToucanSystems;
using System.Collections.Generic;
/// <summary>
/// Test script for pushing FPS values onto the chart.
/// </summary>
public class DataReader : MonoBehaviour
{
    public float start_delay=2;
    public GameObject white_fade;
    public GameObject warningText_d;
    public GameObject warningText_s;
    public Animation resp_anim;
    public TextAsset data_stress;
    public TextAsset data_relax; 

    public string scene_name = "boxing";

    List<int> relax_resp_data;
    List<int> relax_bvp_data;
    
    List<int> stress_resp_data;
    List<int> stress_bvp_data;

    List<int> resp_wave = new List<int>();
    List<int> bvp_wave = new List<int>();
    
    [SerializeField]
    private ChartDataMonitor resp_monitor;

    [SerializeField]
    private ChartDataMonitor bvp_monitor;


    void init_data()
    {
        string[,] relax_data = CSVReader.SplitCsvGrid(data_relax.text);
        string[,] stress_data = CSVReader.SplitCsvGrid(data_stress.text);
        relax_resp_data = ReadData(relax_data,"resp");
        relax_bvp_data = ReadData(relax_data,"bvp1");
        stress_resp_data = ReadData(stress_data,"resp");
        stress_bvp_data = ReadData(stress_data,"bvp1");
    }
    List<int> ReadData(string[,] data_list,string type)
    {
        List<int> signal = new List<int>();
        // bvp1 : data[1,:]
        // resp : data[3,:]
        int index = 0;
        if(type=="resp")
            index = 3;
        else if(type=="bvp1")
            index=1;
        else if (type=="bvp2")
            index=2;
        int data_num = data_list.Length/5 - 3;
        for(int i=0;i<data_num;i++)
        {
            if(data_list[index,i]!=type && data_list[index,i]!=null)
            {
                signal.Add(int.Parse(data_list[index,i]));
            }
        }
        return signal;
    }    
    
    private void OnEnable()
    {
        init_data();
        StartCoroutine(PushDataToGraph());
    }

    private IEnumerator PushDataToGraph(){
        if (scene_name=="boxing")
            yield return ForBoxing();
        else if(scene_name=="virtual_resp")
            yield return ForVirtualResp();
        
    }

    private IEnumerator DataFlowCoroutine(string type)
    {
        //yield return new WaitForSeconds(1);
        int count = 0;
        if(type=="stress")
        {
            bvp_wave = stress_bvp_data;
            resp_wave = stress_resp_data;
        }
        else if(type=="relax")
        {
            bvp_wave = relax_bvp_data;
            resp_wave = relax_resp_data;
        }

        while (count<resp_wave.Count && count<bvp_wave.Count)
        {
            //monitor.monitorValue = 1.0f / Time.deltaTime;
            resp_monitor.monitorValue = resp_wave[count];
            //debugText_resp.text = "Resp: " + resp_monitor.monitorValue.ToString("0");

            bvp_monitor.monitorValue = bvp_wave[count];
            //debugText_bvp.text = "BVP: " + bvp_monitor.monitorValue.ToString("0");

            yield return null;
            count++;
        }
    }

    public IEnumerator ForBoxing()
    {
        //yield return new WaitForSeconds(start_delay);
        yield return DataFlowCoroutine("stress");
        white_fade.SetActive(true);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animation>().CrossFade("CameraBreath");
        yield return DataFlowCoroutine("relax");
    }

    public IEnumerator ForVirtualResp()
    {
        //yield return new WaitForSeconds(start_delay);
        warningText_s.SetActive(true);
        yield return DataFlowCoroutine("relax");
        warningText_s.GetComponent<Animation>().CrossFade("ExpandOut");
        warningText_d.SetActive(true);
        resp_anim.CrossFade("DeepBreath");
        yield return DataFlowCoroutine("stress");

    }

}
