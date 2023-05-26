using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
};

public class PetSelectManager : MonoBehaviour
{
    public static PetSelectManager instance;
    public string sampleWallet_i;   //wallet id�� ���޹޾Ҵ� ����

    public PetNFT[] sampleNFTs; //n���� nft ������ ���޹޾Ҵ� ����, �ν�����â���� ���� ����

    public TextMeshProUGUI boyuPet, nftId, petName, age, gender; //Ŭ�����忡 ��Ÿ���� �� ������

    public SkinnedMeshRenderer corgiMesh;   //�� ��Ų ���� ���
    public Texture[] petTextures;

    public int petArrIdx;   //���� ȭ�鿡 �������� �������� �� ��° ���������� (0���� ����)

    public RawImage[] fades = new RawImage[2];   //�� �� �Ѿ �� ���̵�ƿ� ����
    public AudioSource audioSource; //�����
    



    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
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
            PetSelect();
    }

    public void PetChange() //�� �ؽ�ó ����, Ŭ�������� �ؽ�Ʈ ����
    {
        corgiMesh.materials[0].SetTexture("_BaseMap", sampleNFTs[petArrIdx].pet_color);

        boyuPet.text = "���� �� " + (petArrIdx + 1) + "/" + sampleNFTs.Length;
        nftId.text = "�� NFT ID\n" + sampleNFTs[petArrIdx].pet_token;
        petName.text = "�̸�: " + sampleNFTs[petArrIdx].pet_name;
        age.text = "����: " + sampleNFTs[petArrIdx].pet_age + "months";

        if (sampleNFTs[petArrIdx].pet_sex == "f")
            gender.text = "����: ��";
        else if (sampleNFTs[petArrIdx].pet_sex == "m")
            gender.text = "����: ��";
        else
            gender.text = "����: ?";
    }

    public void PrevPet()   //�� ��� �� ĭ ������ �̵�
    {
        petArrIdx--;
        if (petArrIdx < 0)
            petArrIdx += sampleNFTs.Length;
        PetChange();
    }
    public void NextPet()   //�� ��� �� ĭ �ڷ� �̵�
    {
        petArrIdx++;
        if (petArrIdx >= sampleNFTs.Length)
            petArrIdx -= sampleNFTs.Length;
        PetChange();
    }
    public void PetSelect() //�� ����
    {
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
