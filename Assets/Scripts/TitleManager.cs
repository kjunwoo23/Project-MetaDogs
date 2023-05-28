using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Data;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using System;

public class TitleManager : MonoBehaviour
{
    public Camera cam;
    public GameObject dog;
    public RawImage qrCodeImage;
    public string data = "https://klipwallet.com/?target=/a2a?request_key=";
    public string getData = "https://a2a-api.klipwallet.com/v2/a2a/result?request_key=";

    public class Status
    {
        public string request_key;
        public string result;
        public string status;
        public string expiration_time;
    }

    // Start is called before the first frame update
    void Start()
    {
        qrCodeImage.enabled = false;
        StartCoroutine(UnityWebRequestPostTest());
    }

    IEnumerator UnityWebRequestPostTest()
    {
        string url = "https://a2a-api.klipwallet.com/v2/a2a/prepare";
        // Create the JSON data to send
        string json = "{\"bapp\": {\"name\": \"METADOGS\"}, \"callback\": {\"success\": \"mybapp://klipwallet/success\", \"fail\": \"mybapp://klipwallet/fail\"}, \"type\": \"auth\"}";

        // Create a UnityWebRequest
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        // Wait for the response and then get our data
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            Status convertJson = JsonUtility.FromJson<Status>(www.downloadHandler.text);
            data += convertJson.request_key;    // Request Key�� �����Ǹ� �ش� Ű�� �̿��Ͽ� ����� ���� ��ũ�� ����
            getData += convertJson.request_key;  // ����� ������ �Ϸ�Ǿ����� �����ϴ� ��ũ���� Request Key�� �ʿ���.
        }
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.LookAt(dog.transform);
    }

    private void GenerateQR()   // QR�ڵ� ���� �Լ�
    {
        int width = 256;
        int height = 256;
        Debug.Log(getData);
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };

        Color32[] colorArray = writer.Write(data);
        Texture2D qrCodeTexture = new Texture2D(width, height);
        qrCodeTexture.SetPixels32(colorArray);
        qrCodeTexture.Apply();

        qrCodeImage.texture = qrCodeTexture;
    }

    private IEnumerator GET()  // ����ڰ� ������ �ߴ��� Ȯ���ϴ� ���� GET������� ����
    {
        string result;
        UnityWebRequest www = UnityWebRequest.Get(getData);
        yield return www.SendWebRequest();
        result = www.downloadHandler.text;
        Debug.Log(result);
        Status convertJson = JsonUtility.FromJson<Status>(result);  // �����(string)�� json ���·� ��ȯ
        Debug.Log(convertJson.status);

        if (convertJson.status == "prepared")   // ����ڰ� ������ ���� ������ ��� prepared ������ ���� ���ƿ�
        {
            yield return new WaitForSeconds(5.0f);  // �ణ�� �ð� ������ �־
            StartCoroutine(GET());                  // ��͸� �̿��Ͽ� ��� ȣ��.
        }
        else
        {
            Debug.Log("Completed");  // ����� ������ �Ϸ�Ǹ� �Ϸ� �α� ���
            StartCoroutine(POSTdb());

            // ���Ŀ� NFT ������ ���� ������ �̵��ϴ� �ڵ� �ۼ� ����.
        }
    }
    private IEnumerator POSTdb()
    {
        string url = "http://203.250.148.33:20080/db/load_pet_list";
        string json = "{\"wallet_id\": \"1\"}";

        // Create a UnityWebRequest
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        SceneManager.LoadScene("PetSelect");
    }


    public void OnClickLogIn()
    {
        qrCodeImage.enabled = true;
        GenerateQR();   // Ŭ������ �α��� ��ư�� ������ QR�ڵ带 ������ ��
        StartCoroutine(GET());  // QR�ڵ� ������ ���ÿ� �ڷ�ƾ�� �����Ͽ� ����� ������ Ȯ����

    }
}