using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public int loadType;
    // Start is called before the first frame update
    void Start()
    {
        if (loadType == 0)  //�� ���� ȭ�� �ε�
        {
            RequestManager.Instance.StartLoadingTitleToPetSelect();
        }
        else if (loadType == 1) //�ΰ��� �� �ε�
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
