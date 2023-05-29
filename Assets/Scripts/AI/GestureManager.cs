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

    [Header("GUID ����")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject gestureChangePanel;
    [SerializeField] private Transform target;

    [Header("����ó �ν� ����")]
    [SerializeField] [Range(0f, 45f)] private float penaltyAngle = 15f;
    [SerializeField] [Range(0f, 1f)] private float curveDelay = 0.15f;
    [SerializeField] [Range(0.1f, 20f)] private float shakingCorrectionAngle = 10f;

    private Dictionary<BehaviorType, List<Vector3>> dataSet;
    private bool isObserving = false;
    private bool isStopped = false;
    private bool isWorking = false;
    private bool isTraining = false;

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

    private IEnumerator TrainingCoroutine(BehaviorType type)
    {
        isTraining = true;
        yield return RequestManager.Instance.StartCoroutine("LoadMLAgent", (int)type);
        isTraining = false;
    }


    public BehaviorType CurrentBehaviorType { private set; get; } = BehaviorType.Undecided;
    public void StartSensing(BehaviorType type)
    {
        text.gameObject.SetActive(true);
        gestureChangePanel.SetActive(false);
        text.SetText("������ A��ư����\n�Ʒ� �����ϱ�");
        //EffectManager.instance.PlayEffect(3);
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
        text.SetText("������ A��ư����\n����ó �Է��ϱ�");
        //EffectManager.instance.PlayEffect(3);

        CurrentBehaviorType = BehaviorType.Undecided;
        mode = ManagerMode.Validating;
    }
    
    public void SetTextUnrecognizable()
    {
        text.SetText("�������� �������� ���� �� ���ƿ�.");
        EffectManager.instance.PlayEffect(1);
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
                EffectManager.instance.PlayEffect(3);
            }
        }
        else if (mode == ManagerMode.Validating)
        {
            if (!isWorking && OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Z))
            {
                isWorking = true;
                StartCoroutine(MatchGestureCoroutine());
                EffectManager.instance.PlayEffect(3);
            }
        }

        // ����ó ���� ����
        if (isObserving && OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.X))
        {
            isStopped = true;
            //EffectManager.instance.PlayEffect(2);
        }
    }

    private IEnumerator AddGestureCoroutine(BehaviorType type)
    {
        ObserveGesture(out var inputGesture);
        yield return new WaitUntil(() => !isObserving);
        text.SetText("");
        //text.gameObject.SetActive(false);

        // ���� �н� �����Ͱ� �ִٸ�
        if (dataSet.ContainsKey(type))
        {
            // ���� ����ó�� ��ġ�ϸ� �н� �Ϸ�
            if (IsMatched(inputGesture, type) < 1f)
            {
                StartCoroutine(TrainingCoroutine(type));
                TrainManager.instance.RecordFin(true, false);
            }
            // �ٸ� ����ó ���� ��
            else
            {
                BehaviorType resultType = MatchAllGesture(inputGesture);

                // ó�� ���� ����ó���, ����ó ������ �� ������ ���´�.
                if (resultType == BehaviorType.Undecided)
                {
                    gestureChangePanel.SetActive(true);
                    Player.instance.laser.SetActive(true);
                    yield return new WaitUntil(() => isAllowedChangingGesture != 0);
                    gestureChangePanel.SetActive(false);
                    Player.instance.laser.SetActive(false);

                    // ����ó ������ ������� ��, ���� ������ �ʱ�ȭ
                    if (isAllowedChangingGesture == 1)
                    {
                        isAllowedChangingGesture = 0;
                        dataSet[type] = inputGesture;
                        UpdateData();
                        StartCoroutine(TrainingCoroutine(type));
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
                }
                // �ٸ� �ൿ�� �̹� ���� �ִ� �����̶�� ����
                else
                {
                    isWorking = false;
                    isAllowedChangingGesture = 0;
                    text.SetText($"�ش� ����ó�� {resultType}�� �����մϴ�!");
                    EffectManager.instance.PlayEffect(1);
                    yield return new WaitForSeconds(2f);

                    StartSensing(givenBehavior);
                    yield break;
                }
            }
        }
        else
        {
            BehaviorType resultType = MatchAllGesture(inputGesture);

            // ���� ����ó �߰�
            if (resultType == BehaviorType.Undecided)
            {
                dataSet[type] = inputGesture;
                UpdateData();
                StartCoroutine(TrainingCoroutine(type));
                TrainManager.instance.RecordFin(true, true);
            }
            //  �ٸ� �ൿ�� �ش� ����ó�� ���� �ִٸ� ����
            else
            {
                isWorking = false;
                isAllowedChangingGesture = 0;
                text.SetText($"�ش� ����ó�� {resultType}�� �����մϴ�!");
                EffectManager.instance.PlayEffect(1);
                yield return new WaitForSeconds(2f);

                StartSensing(givenBehavior);
                yield break;
            }
        }

        mode = ManagerMode.None;
        isWorking = false;
    }

    private IEnumerator MatchGestureCoroutine()
    {
        DogAnimator.instance.animator.ResetTrigger("loopEnd");
        ObserveGesture(out var curGesture);
        yield return new WaitUntil(() => !isObserving);

        BehaviorType resultType = MatchAllGesture(curGesture);

        if (resultType == BehaviorType.Undecided)
        {
            text.SetText("��ġ�ϴ� ����ó�� �����ϴ�!");
            EffectManager.instance.PlayEffect(1);
            yield return new WaitForSeconds(1f);
            CurrentBehaviorType = BehaviorType.None;
        }
        else
        {
            CurrentBehaviorType = resultType;
            text.SetText($"{CurrentBehaviorType}\n�������� ��");
            EffectManager.instance.PlayEffect(0);
            mode = ManagerMode.None;
        }
        isWorking = false;
    }

    private BehaviorType MatchAllGesture(List<Vector3> gesture)
    {
        float minError = float.MaxValue;
        BehaviorType resultType = BehaviorType.Undecided;
        foreach (var item in dataSet)
        {
            float curError = IsMatched(gesture, item.Key);
            if (curError < 1f && curError < minError)
            {
                minError = curError;
                resultType = item.Key;
            }
        }
        return resultType;
    }

    private float IsMatched(List<Vector3> curGesture, BehaviorType type)
    {
        // Set long & short vector
        List<Vector3> longVec = curGesture;
        List<Vector3> shortVec = dataSet[type];

        if (longVec.Count < shortVec.Count)
        {
            longVec = dataSet[type];
            shortVec = curGesture;
        }

        if (longVec.Count - shortVec.Count > 4) return float.MaxValue;


        // Check Error
        float gestureError = 0f;
        int shortIdx = 0;
        for (int longIdx = 0; longIdx < longVec.Count; ++longIdx)
        {
            // Cosine similarity
            float curError = Mathf.Abs(Vector3.Angle(longVec[longIdx], shortVec[shortIdx]));
            if (shortIdx + 1 < shortVec.Count)
            {
                float nextError = Mathf.Abs(Vector3.Angle(longVec[longIdx], shortVec[shortIdx + 1]));
                if (curError < nextError)
                {
                    gestureError += curError;
                }
                else
                {
                    gestureError += nextError;
                    ++shortIdx;
                }
            }
            else
            {
                gestureError += curError;
            }
        }

        int totalCount = longVec.Count;

        // Penalty
        for (; shortIdx < shortVec.Count; ++shortIdx)
        {
            gestureError += penaltyAngle;
            ++totalCount;
        }

        gestureError /= totalCount;

        // [10, 55) ������ ���������ϴ� �Լ�
        float dirError = -45 * Mathf.Exp(-0.15f * totalCount) + 55;

        Debug.Log($"�ൿ: {type} | {Mathf.Round(gestureError)} / {Mathf.Round(dirError)}");
        return gestureError / dirError;
    }


    // �Լ� ������: isObserving == false
    private void ObserveGesture(out List<Vector3> newList)
    {
        text.SetText("����ó ������...\n������ A��ư���� �����ϱ�");
        EffectManager.instance.PlayEffect(3);
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

        // �ʱ⿡ �ణ �������� �ν� ����
        do
        {
            yield return observeDelay;
            endPos = target.localPosition;
            prevVec = endPos - startPos;
            if (isStopped)
            {
                text.SetText("����ó�� �ʹ� �۽��ϴ�!\n�� �������ּ���.");
                EffectManager.instance.PlayEffect(1);
            }
        } while (prevVec.magnitude < 0.1f);
        startPos = endPos;
        newList.Add(prevVec);

        while (!isStopped)
        {
            yield return observeDelay;
            endPos = target.localPosition;
            Vector3 diff = endPos - startPos;

            // ����, � ����
            if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > shakingCorrectionAngle)
            {
                yield return curveDelay;
                endPos = target.localPosition;
                diff = endPos - startPos;
                if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > shakingCorrectionAngle)
                {
                    newList.Add(diff);
                    prevVec = diff;
                }
            }
            else
            {
                newList[^1] += newList[^1].normalized * diff.magnitude;
            }
            startPos = endPos;
        }
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
    public void InitText()
    {
        text.gameObject.SetActive(false);
    }
}
