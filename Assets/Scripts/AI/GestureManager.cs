using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GestureType
{
    SitDown
}

public class GestureManager : MonoBehaviour
{
    [SerializeField] private Transform target;

    private readonly Dictionary<GestureType, List<Vector3>> dataSet = new();
    private bool isObserving = false;
    private bool isMatched = false;
    private bool isStopped = false;

    private bool flag = true;

    void Update()
    {
        if (flag && Input.GetKeyDown(KeyCode.Z))
        {
            flag = false;
            StartCoroutine(AddGestureCoroutine(GestureType.SitDown));
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            isStopped = true;
        }
        if (flag && Input.GetKeyDown(KeyCode.C))
        {
            flag = false;
            StartCoroutine(MatchGestureCoroutine());
        }
    }

    private IEnumerator AddGestureCoroutine(GestureType type)
    {
        List<Vector3> inputGesture = new();

        isObserving = true;
        StartCoroutine(ObserveGestureCoroutine(inputGesture));
        yield return new WaitUntil(() => !isObserving);
        dataSet[type] = inputGesture;
        flag = true;
    }

    private IEnumerator MatchGestureCoroutine()
    {
        List<Vector3> curGesture = new();

        isObserving = true;
        StartCoroutine(ObserveGestureCoroutine(curGesture));
        yield return new WaitUntil(() => !isObserving);

        foreach(var item in dataSet)
        {
            isMatched = MatchGesture(curGesture, item.Key);
            if (isMatched) break;
        }

        if (isMatched)
            Debug.Log("�ɱ⿡��~~");
        else
            Debug.Log("������ �̰�?");
        flag = true;
    }

    private bool MatchGesture(List<Vector3> curGesture, GestureType type)
    {
        if (dataSet[type].Count != curGesture.Count)
        {
            Debug.Log("����: " + dataSet[type].Count + " ����: " + curGesture.Count);
            return false;
        }

        float dirError = 0f;
        for (int i = 0; i < curGesture.Count; ++i)
        {
            dirError += Mathf.Pow(Vector3.Angle(dataSet[type][i], curGesture[i]), 2);
        }

        Debug.Log("dirError: " + dirError);
        if (dirError > 1000f) return false;
        else return true;
    }

    private IEnumerator ObserveGestureCoroutine(List<Vector3> newList)
    {
        var delay = new WaitForSeconds(0.1f);
        var longDelay = new WaitForSeconds(0.5f);

        Vector3 startPos = target.position;
        Vector3 endPos;
        Vector3 prevVec;

        // �ʱ⿡ 0.5 �̻� �������� �ν� ����
        do
        {
            yield return delay;
            endPos = target.position;
            prevVec = endPos - startPos;
        } while (prevVec.magnitude < 0.5f);
        startPos = endPos;
        Debug.Log(newList.Count + "����: " + prevVec.normalized);
        newList.Add(prevVec);

        isStopped = false;
        while (!isStopped)
        {
            yield return delay;
            endPos = target.position;
            Vector3 diff = endPos - startPos;
            if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > 15f)
            {
                // � ������ �����Ѵ�.
                yield return longDelay;
                endPos = target.position;
                diff = endPos - startPos;
                if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > 15f)
                {
                    newList.Add(diff);
                    prevVec = diff;
                    Debug.Log(newList.Count + "����: " + prevVec.normalized);
                }
            }
            else
            {
                newList[^1] = newList[^1].normalized * diff.magnitude;
            }
            startPos = endPos;
        }
        Debug.Log("��¡�� ������ ����: " + newList.Count);
        isObserving = false;
    }
}
