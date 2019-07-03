using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;
using System;
public class ChartController : MonoBehaviour
{
    public TextAsset data_stress;
    public TextAsset data_relax; 
    public GraphChart resp_chart;
    public GraphChart bvp_chart;

    const string category = "signal";

    List<int> relax_resp_data;
    List<int> relax_bvp_data;
    
    List<int> stress_resp_data;
    List<int> stress_bvp_data;

    int init_sec = 3;
    int sample_rate = 700;
    int sample_rate_unity = 50;

    // Start is called before the first frame update

    void init_data()
    {
        string[,] relax_data = CSVReader.SplitCsvGrid(data_relax.text);
        string[,] stress_data = CSVReader.SplitCsvGrid(data_stress.text);
        relax_resp_data = ReadData(relax_data,"resp");
        relax_bvp_data = ReadData(relax_data,"bvp1");
        stress_resp_data = ReadData(stress_data,"resp");
        stress_bvp_data = ReadData(stress_data,"bvp1");
    }
    void Start()
    {
        init_data();
        StartCoroutine(push_data("stress"));
        StartCoroutine(push_data("relax"));
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

    IEnumerator push_data(string type)
    {
        if(type=="stress")
        {
            int num = Math.Min(stress_bvp_data.Count,stress_resp_data.Count);
            for(int i=0;i<num;i++)
            {
                resp_chart.DataSource.AddPointToCategoryRealtime(category,DateTime.Now,stress_resp_data[i]);
                bvp_chart.DataSource.AddPointToCategoryRealtime(category,DateTime.Now,stress_bvp_data[i]);
                yield return null;
            }
        }
        else if(type=="relax")
        {
            int num = Math.Min(relax_bvp_data.Count,relax_resp_data.Count);
            for(int i=0;i<num;i++)
            {
                resp_chart.DataSource.AddPointToCategoryRealtime(category,DateTime.Now,relax_resp_data[i]);
                bvp_chart.DataSource.AddPointToCategoryRealtime(category,DateTime.Now,relax_bvp_data[i]);
                yield return null;
            }
        }

    }
}
