using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public SkinnedMeshRenderer corgiMesh;
    public GameObject autoToy;

    public Transform[] playerSpawnPoints = new Transform[2];
    public Transform[] petSpawnPoints = new Transform[2];

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("NftManager"))
            NftManager.instance.NftSetting();
        PositionSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void PositionSetting()
    {
        //playMode�� 0�̸� �ϻ���, 1�̸� �Ʒø�� ��ġ�� ����
        //playMode�� ����UI���� ��� ��ȯ ���� ������ �ٲ�
        //PlayerPrefs�� ����Ǳ� ������ �Ʒø���� ä�� ���� ���� Ű�� �Ʒø��� ������
        if (PlayerPrefs.HasKey("playMode"))
        {
            if (PlayerPrefs.GetInt("playMode") == 0)
            {
                Player.instance.transform.position = playerSpawnPoints[0].position;
                Player.instance.transform.rotation = playerSpawnPoints[0].rotation;
                DogAnimator.instance.gameObject.transform.position = petSpawnPoints[0].position;
                DogAnimator.instance.gameObject.transform.rotation = petSpawnPoints[0].rotation;
                TrainManager.instance.TrainModeDisable();
            }
            else if (PlayerPrefs.GetInt("playMode") == 1)
            {
                TrainManager.instance.table.SetActive(false);
                Player.instance.transform.position = playerSpawnPoints[1].position;
                Player.instance.transform.rotation = playerSpawnPoints[1].rotation;
                DogAnimator.instance.gameObject.transform.position = petSpawnPoints[1].position;
                DogAnimator.instance.gameObject.transform.rotation = petSpawnPoints[1].rotation;
                TrainManager.instance.TrainModeEnable();
            }
        }
        else
        {   //������ ó�� ������ �ϻ���� ����
            PlayerPrefs.SetInt("playMode", 0);
            Player.instance.transform.position = playerSpawnPoints[0].position;
            Player.instance.transform.rotation = playerSpawnPoints[0].rotation;
            DogAnimator.instance.gameObject.transform.position = petSpawnPoints[0].position;
            DogAnimator.instance.gameObject.transform.rotation = petSpawnPoints[0].rotation;
        }
    }
}
