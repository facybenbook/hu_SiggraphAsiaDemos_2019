using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ToucanSystems;
using System.Collections.Generic;
/// <summary>
/// Test script for pushing FPS values onto the chart.
/// </summary>
public class SimpleFPSReader : MonoBehaviour
{
    public TextAsset data_stress;
    public TextAsset data_relax; 

    List<int> relax_resp_data;
    List<int> relax_bvp_data;
    
    List<int> stress_resp_data;
    List<int> stress_bvp_data;
    [SerializeField]
    private Text debugText;
    [SerializeField]
    private ChartDataMonitor monitor;
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
        StartCoroutine(DataFlowCoroutine());
    }

    private IEnumerator DataFlowCoroutine()
    {
        //yield return new WaitForSeconds(1);
        int count = 0;
        while (count<stress_bvp_data.Count && count<stress_resp_data.Count)
        {
            //monitor.monitorValue = 1.0f / Time.deltaTime;
            monitor.monitorValue = stress_bvp_data[count];
            debugText.text = "FPS: " + monitor.monitorValue.ToString("0.00");
            yield return new WaitForSeconds(0.01f);
            count++;
        }
    }
}
