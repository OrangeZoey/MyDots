using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private int score;//�÷�
    public Text ScoreText;//�÷��ı�

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //����ǰ�÷ֲ����
        if(score!=SharedData.gameShareData.Data.deadCounter)
        {
            score = SharedData.gameShareData.Data.deadCounter;
            ScoreText.text=score.ToString();
        }
    }
}
