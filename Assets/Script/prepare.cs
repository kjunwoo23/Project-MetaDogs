using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class prepare : MonoBehaviour
{
    private string clientId = "MetaDogs";
    private string redirectUri = "YOUR_APP_REDIRECT_URI";
    private string accessToken = "";

    IEnumerator GetAccessToken(string authCode)
    {
        string url = $"https://kauth.kakao.com/oauth/token?grant_type=authorization_code&client_id={clientId}&redirect_uri={redirectUri}&code={authCode}";
        UnityWebRequest www = UnityWebRequest.Post(url, "");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            // JSON ������ ó���Ͽ� �׼��� ��ū�� ����ϴ�.
            //accessToken = ProcessJson(jsonResponse);
            Debug.Log(jsonResponse);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
}