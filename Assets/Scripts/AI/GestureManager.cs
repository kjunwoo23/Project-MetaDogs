using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BehaviorType
{
    None = -2,
    Undecided = -1,
    SitSide,
    Die,
    Jump,
    Lie,
    SitDown,
    Attack,
}

public class GestureManager : MonoBehaviour
{
    private enum ManagerMode
    {
        None,
        Sensing,
        Validating,
    }

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject gestureChangePanel;
    [SerializeField] private Transform target;
    [SerializeField] private float errorRatePerCount = 3750f;
    [SerializeField] [Range(0f, 1f)] private float curveDelay = 0.15f;
    [SerializeField] [Range(100f, 10000f)] private float countPenalty = 3000f;

    private Dictionary<BehaviorType, List<Vector3>> dataSet;
    private float errorRate = 0f;
    private bool isObserving = false;
    private bool isStopped = false;
    private bool isWorking = false;

    private int isAllowedChangingGesture = 0;
    private BehaviorType givenBehavior;
    private ManagerMode mode = ManagerMode.None;

    private void Awake()
    {
        text.gameObject.SetActive(false);
        gestureChangePanel.SetActive(false);
        SaveDataManager.Instance.LoadTrainingData(out dataSet);
    }

    private void UpdateData()
    {
        SaveDataManager.Instance.SaveTrainingData(dataSet);
    }


    public BehaviorType CurrentBehaviorType { private set; get; } = BehaviorType.Undecided;
    public void StartSensing(BehaviorType type)
    {
        text.gameObject.SetActive(true);
        gestureChangePanel.SetActive(false);
        text.SetText("������ A��ư���� �Ʒ� �����ϱ�");
        givenBehavior = type;
        CurrentBehaviorType = type;
        mode = ManagerMode.Sensing;
    }
    public void StartValidate()
    {
        text.gameObject.SetActive(true);
        gestureChangePanel.SetActive(false);

        if (dataSet.Count == 0)
        {
            text.SetText("");
            return;
        }
        text.SetText("������ A��ư���� ����ó �Է��ϱ�");


        CurrentBehaviorType = BehaviorType.Undecided;
        mode = ManagerMode.Validating;
    }


    void Update()
    {
        if (mode == ManagerMode.None) return;

        else if (mode == ManagerMode.Sensing)
        {
            if (!isWorking && (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Z)))
            {
                isWorking = true;
                StartCoroutine(AddGestureCoroutine(givenBehavior));
            }
        }
        else if (mode == ManagerMode.Validating)
        {
            if (!isWorking && OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Z))
            {
                isWorking = true;
                StartCoroutine(MatchGestureCoroutine());
            }
        }

        // ����ó ���� ����
        if (isObserving && OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.X))
        {
            isStopped = true;
        }
    }

    private IEnumerator AddGestureCoroutine(BehaviorType type)
    {
        ObserveGesture(out var inputGesture);
        yield return new WaitUntil(() => !isObserving);
        text.SetText("");
        //text.gameObject.SetActive(false);

        if (dataSet.ContainsKey(type))
        {
            // ���� �ൿ�� �ٸ� ����ó ���� ��
            if (!IsMatched(inputGesture, type))
            {
                gestureChangePanel.SetActive(true);
                Player.instance.laser.SetActive(true);
                yield return new WaitUntil(() => isAllowedChangingGesture != 0);
                gestureChangePanel.SetActive(false);
                Player.instance.laser.SetActive(false);

                // ����ó ������ ������� ��, ���� ������ �ʱ�ȭ
                if (isAllowedChangingGesture == 1)
                {
                    dataSet[type] = inputGesture;
                    UpdateData();
                    TrainManager.instance.RecordFin(true, true);
                }
                // ����ó ������ ���� �ʾ��� ��, ���� �Է� ����
                else if (isAllowedChangingGesture == -1)
                {
                    isWorking = false;
                    isAllowedChangingGesture = 0;
                    StartSensing(givenBehavior);
                    yield break;
                }
                isAllowedChangingGesture = 0;
            }
            // ���� ����ó �н�
            else
            {
                TrainManager.instance.RecordFin(true, false);
            }
        }
        else
        {
            // �̹� �ش� ����ó�� �ٸ� ����ó�� ���� ���� �� Ȯ��
            foreach (var item in dataSet)
            {
                if (IsMatched(inputGesture, item.Key))
                {
                    isWorking = false;
                    isAllowedChangingGesture = 0;
                    text.SetText($"�ش� ����ó�� �̹� {item.Key}�� ������Դϴ�!");
                    //text.text = "�ش� ����ó�� �̹� " + item.Key.ToString() + "�� ������Դϴ�!";
                    Debug.Log("����!!!");
                    yield return new WaitForSeconds(1f);

                    StartSensing(givenBehavior);
                    yield break;
                }
            }

            // ���� ����ó �߰�
            dataSet[type] = inputGesture;
            UpdateData();
            TrainManager.instance.RecordFin(true, true);
        }

        mode = ManagerMode.None;
        isWorking = false;
    }

    private IEnumerator MatchGestureCoroutine()
    {
        ObserveGesture(out var curGesture);
        yield return new WaitUntil(() => !isObserving);

        foreach(var item in dataSet)
        {
            if (IsMatched(curGesture, item.Key))
            {
                CurrentBehaviorType = item.Key;
                isWorking = false;
                text.SetText($"{CurrentBehaviorType} �������� ��");
                mode = ManagerMode.None;
                yield break;
            }
        }

        text.SetText("��ġ�ϴ� ����ó�� �����ϴ�!");
        yield return new WaitForSeconds(1f);

        CurrentBehaviorType = BehaviorType.None;
        isWorking = false;
    }

    private bool IsMatched(List<Vector3> curGesture, BehaviorType type)
    {
        // Set long & short vector
        List<Vector3> longVec = curGesture;
        List<Vector3> shortVec = dataSet[type];
        errorRate = shortVec.Count * errorRatePerCount;

        if (longVec.Count < shortVec.Count)
        {
            longVec = dataSet[type];
            shortVec = curGesture;
        }

        // Check Error
        float dirError = 0f;
        int shortIdx = 0;
        for (int longIdx = 0; longIdx < longVec.Count; ++longIdx)
        {
            // Cosine similarity�� �̿��� MSE ���
            float curError = Mathf.Pow(Vector3.Angle(longVec[longIdx], shortVec[shortIdx]), 2);
            if (shortIdx + 1 < shortVec.Count)
            {
                float nextError = Mathf.Pow(Vector3.Angle(longVec[longIdx], shortVec[shortIdx + 1]), 2);
                if (curError < nextError)
                {
                    dirError += curError;
                }
                else
                {
                    dirError += nextError;
                    ++shortIdx;
                }
            }
            else
            {
                if (curError < 0.01f)
                {
                    dirError += countPenalty;
                }
                else
                {
                    dirError += curError;
                }
            }
        }

        // Penalty
        for (; shortIdx < shortVec.Count; ++shortIdx)
        {
            dirError += countPenalty;
        }

        Debug.Log(type.ToString() + " | " + "��ǥ ����: " + errorRate + " | ������: " + dirError);

        if (dirError > errorRate)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    // �Լ� ������: isObserving == false
    private void ObserveGesture(out List<Vector3> newList)
    {
        text.SetText("����ó ������...\n ������ A��ư���� �����ϱ�");
        newList = new();
        isObserving = true;
        StartCoroutine(ObserveGestureCoroutine(newList));
    }

    private IEnumerator ObserveGestureCoroutine(List<Vector3> newList)
    {
        var observeDelay = new WaitForSeconds(0.1f);
        var curveDelay = new WaitForSeconds(this.curveDelay);

        Vector3 startPos = target.localPosition;
        Vector3 endPos;
        Vector3 prevVec;

        isStopped = false;

        // �ʱ⿡ 0.1 �̻� �������� �ν� ����
        do
        {
            yield return observeDelay;
            endPos = target.localPosition;
            prevVec = endPos - startPos;
            if (isStopped)
            {
                text.SetText("����ó�� �ʹ� �۽��ϴ�!\n �� �������ּ���.");
            }
        } while (prevVec.magnitude < 0.1f);
        startPos = endPos;
        newList.Add(prevVec);

        while (!isStopped)
        {
            yield return observeDelay;
            endPos = target.localPosition;
            Vector3 diff = endPos - startPos;
            if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > 15f)
            {
                // � ���� ����
                yield return curveDelay;
                endPos = target.localPosition;
                diff = endPos - startPos;
                if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > 15f)
                {
                    newList.Add(diff);
                    prevVec = diff;
                }
            }
            else
            {
                newList[^1] = newList[^1].normalized * diff.magnitude;
            }
            startPos = endPos;
        }
        errorRate = newList.Count * errorRatePerCount;
        isObserving = false;
    }


    public void OnClickChangeOK()
    {
        isAllowedChangingGesture = 1;
    }
    public void OnClickChangeNo()
    {
        isAllowedChangingGesture = -1;
    }
}
