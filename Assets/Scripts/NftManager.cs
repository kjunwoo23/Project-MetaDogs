using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PetNFT
{
    public string pet_token;
    public string pet_name;
    public int pet_age;
    public string pet_sex; //(f�� ����, m�� ����)
    //pet_color: png ����
    public Texture pet_color;
    public float pet_emotion;
    //model: onnx ����
    //property: yaml ����
};

[System.Serializable]
public class PetJson
{
    public PetNFT[] nftList;
};

public class NftManager : MonoBehaviour
{
    public static NftManager instance;

    public string walletID;

    public PetJson petJson;
    //public PetNFT[] nftList;
    public PetNFT selected;
    public int listLen;

    public bool jumpScroll;
    public bool attackScroll;

    public bool autoToy;

    public int weather;

    private void Awake()
    {
        instance = this;
        //nftList = new PetNFT[listLen];
    }
    // Start is called before the first frame update
    void Start()
    {


        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
    public void LoadTextures()  //�� NFT ������Ƽ�� ������ �� ���� ȣ�����ּ���
    {
        for (int i = 0; i < petJson.nftList.Length; i++)
        {
            //petJson.nftList[i].pet_color = Resources.Load<Texture2D>(petJson.nftList[i].pet_token + "_texture");
            //Texture2D tmp = new Texture2D(2, 2);
           // FileStream file = File.Open(Application.streamingAssetsPath + "/" + petJson.nftList[i].pet_token + "_texture.png", FileMode.Open);
            //Debug.Log(Application.streamingAssetsPath + "/" + petJson.nftList[i].pet_token + "_texture.png");
            //tmp.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + "/" + petJson.nftList[i].pet_token + "_texture.png"));
            
            petJson.nftList[i].pet_color = LoadImage(Application.streamingAssetsPath + "/" + petJson.nftList[i].pet_token + "_texture.png");
        }
    }
    public void NftSetting()
    {
        SpawnManager.instance.corgiMesh.materials[0].SetTexture("_BaseMap", selected.pet_color);

        if (jumpScroll)
            TrainManager.instance.petPosesEnabled[2] = true;
        if (attackScroll)
            TrainManager.instance.petPosesEnabled[5] = true;

        if (autoToy)
            SpawnManager.instance.autoToy.SetActive(true);

        WeatherManager.instance.ChangeWeather(weather);
    }
    public Texture LoadImage(string name)
    {
        byte[] byteTexture = System.IO.File.ReadAllBytes(name);
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(byteTexture);
        return texture;
    }

}
