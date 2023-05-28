using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
/*
[System.Serializable]
public class PetNFT
{
    public int pet_token;
    public string pet_name;
    public int pet_age;
    public string pet_sex; //(f�� ����, m�� ����)
    //pet_color: png ����
    public Texture pet_color;
    //model: onnx ����
    //property: yaml ����
};*/

public class PetSelectManager : MonoBehaviour
{
    public static PetSelectManager instance;
    NftManager nftManager;
    
    public string sampleWallet_i;   //wallet id�� ���޹޾Ҵ� ����

    public PetNFT[] sampleNFTs; //n���� nft ������ ���޹޾Ҵ� ����, �ν�����â���� ���� ����

    public TextMeshProUGUI boyuPet, nftId, petName, age, gender; //Ŭ�����忡 ��Ÿ���� �� ������

    public SkinnedMeshRenderer corgiMesh;   //�� ��Ų ���� ���
    public Texture[] petTextures;

    public int petArrIdx;   //���� ȭ�鿡 �������� �������� �� ��° ���������� (0���� ����)

    public RawImage[] fades = new RawImage[2];   //�� �� �Ѿ �� ���̵�ƿ� ����
    public AudioSource audioSource; //�����

    public TextMeshProUGUI nftDlc;
    public GameObject[] nftUIs;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        nftManager = NftManager.instance;

        if (PlayerPrefs.HasKey("devLogin"))
            if (PlayerPrefs.GetInt("devLogin") == 1)
            {   
                //������ �α����� ��� ���� NFT���� NftManager�� ����
                nftManager.petJson.nftList = new PetNFT[sampleNFTs.Length];
                for (int i = 0; i < sampleNFTs.Length; i++)
                    nftManager.petJson.nftList[i] = sampleNFTs[i];

                //�Ʒú����, �峭���� ��� ����
                nftManager.jumpScroll = true;
                nftManager.attackScroll = true;
                nftManager.autoToy = true;
            }
            else
            {
                /*
                �Ϲ� �α����� ���
                ���� NFT���� NftManager�� ����
                */
            }
        else
        { 
            //������ �α����� ��� ���� NFT���� NftManager�� ����
            PlayerPrefs.SetInt("devLogin", 1);
            nftManager.petJson.nftList = new PetNFT[sampleNFTs.Length];
            for (int i = 0; i < sampleNFTs.Length; i++)
                nftManager.petJson.nftList[i] = sampleNFTs[i];
            
            //�Ʒú����, �峭���� ��� ����
            nftManager.jumpScroll = true;
            nftManager.attackScroll = true;
            nftManager.autoToy = true;
        }

        if (nftManager.attackScroll)
            nftUIs[0].SetActive(true);
        if (nftManager.jumpScroll)
            nftUIs[1].SetActive(true);
        if (nftManager.autoToy)
            nftUIs[2].SetActive(true);

        PetChange();
    }

    // Update is called once per frame
    void Update()
    {   //�� �ѱ��, �� ����
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            PrevPet();
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            NextPet();
        if (Input.GetKeyDown(KeyCode.Space))
            OnClickPetSelect();
    }

    public void PetChange() //�� �ؽ�ó ����, Ŭ�������� �ؽ�Ʈ ����
    {
        corgiMesh.materials[0].SetTexture("_BaseMap", nftManager.petJson.nftList[petArrIdx].pet_color);

        //Debug.Log(nftManager.petJson.nftList[petArrIdx].pet_token);
        boyuPet.text = "���� �� " + (petArrIdx + 1) + "/" + nftManager.petJson.nftList.Length;
        nftId.text = "�� NFT ID\n" + nftManager.petJson.nftList[petArrIdx].pet_token;
        petName.text = "�̸�: " + nftManager.petJson.nftList[petArrIdx].pet_name;
        age.text = "����: " + nftManager.petJson.nftList[petArrIdx].pet_age + "months";
        //Debug.Log(age.text);
        //Debug.Log(nftManager.petJson.nftList[petArrIdx].pet_age + "months");

        if (nftManager.petJson.nftList[petArrIdx].pet_sex == "f")
            gender.text = "����: ��";
        else if (nftManager.petJson.nftList[petArrIdx].pet_sex == "m")
            gender.text = "����: ��";
        else
            gender.text = "����: ?";
    }

    public void PrevPet()   //�� ��� �� ĭ ������ �̵�
    {
        petArrIdx--;
        if (petArrIdx < 0)
            petArrIdx += nftManager.petJson.nftList.Length;
        PetChange();
    }
    public void NextPet()   //�� ��� �� ĭ �ڷ� �̵�
    {
        petArrIdx++;
        if (petArrIdx >= nftManager.petJson.nftList.Length)
            petArrIdx -= nftManager.petJson.nftList.Length;
        PetChange();
    }
    public void OnClickPetSelect() //�� ����
    {
        nftManager.selected = nftManager.petJson.nftList[petArrIdx];
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()   //���̵�ƿ�, �� ��ȯ
    {
        float tmpVol = audioSource.volume;
        while (fades[0].transform.localScale.x < 9)
        {
            fades[0].transform.localScale += new Vector3(9 * Time.deltaTime, 0, 0);
            fades[1].transform.localScale += new Vector3(9 * Time.deltaTime, 0, 0);
            audioSource.volume -= tmpVol * Time.deltaTime;

            yield return null;
        }
        //DontDestroyOnLoad(gameObject);
        //yield return new WaitForSeconds(1);
        //SceneManager.LoadScene("CutScene");
        ToIngameScene();
    }

    public void ToIngameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
