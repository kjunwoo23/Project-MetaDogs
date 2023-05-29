using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;
    public Coroutine saveCor;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToDailyMode()
    {   //�Ʒø���
        if (saveCor != null) return;
        PlayerPrefs.SetInt("playMode", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToTrainingMode()
    {   //�ϻ����
        if (saveCor != null) return;
        PlayerPrefs.SetInt("playMode", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToPetSelect()
    {   //�� ���� ����
        if (saveCor != null) return;
        if (GameObject.Find("NftManager"))
            NftManager.instance.DestroyThis();
        SceneManager.LoadScene("PetSelect");
    }
    public void Save()
    {   //���� ���̺� Ŭ��
        if (saveCor != null) return;
        saveCor = StartCoroutine(SaveData());
    }

    public void ExitGame()
    {   //����
        if (saveCor != null) return;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }
    public IEnumerator ExitCor()
    {
        yield return StartCoroutine(SaveData());
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }
    public IEnumerator SaveData()
    {
        Debug.Log(1);
        yield return StartCoroutine(RequestManager.Instance.SavePetProperty());
        yield return StartCoroutine(RequestManager.Instance.SaveSettings());
        saveCor = null;
    }
}
