using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private int score;//得分
    public Text ScoreText;//得分文本

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //跟当前得分不相等
        if(score!=SharedData.gameShareData.Data.deadCounter)
        {
            score = SharedData.gameShareData.Data.deadCounter;
            ScoreText.text=score.ToString();
        }
    }
}
