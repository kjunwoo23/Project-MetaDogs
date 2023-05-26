using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    public static TrainManager instance;
    public bool trainMode;

    public GestureManager gestureManager;
    private BehaviorType behaviorType;

    public float maxHamJudgeTime;
    public bool hamNoticed;

    public bool[] petPosesEnabled = new bool[6];
    public GameObject[] hitBoxes = new GameObject[6];

    public GameObject table;

    //public BoxCollider[] hamArea = new BoxCollider[6];

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        trainMode = DogAnimator.instance.animator.GetBool("trainMode");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))    //�Ʒø�� ���� ��ȯ (�������� ����)
        {
            if (DogAnimator.instance.animator.GetBool("trainMode"))
            {
                TrainModeDisable();
            }
            else
            {
                TrainModeEnable();
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
            HitBoxOnOff();
    }

    public void TrainModeEnable()
    {
        DogAnimator.instance.animator.SetBool("trainMode", true);
        trainMode = true;
    }

    public void TrainModeDisable()
    {
        DogAnimator.instance.animator.SetBool("trainMode", false);
        trainMode = false;
    }

    public void HamNotice(int i)
    {   //�Ʒø�忡�� �������� ������ �ν��ߴٴ� ��, ��� ��� ���ϰ� ������ �ٸ� ���� ��Ÿ���� �Ĵٺ��� ����
        //i�� �̰� � �������� ǥ��
        if (!trainMode) return;

        hamNoticed = true;
        DogAnimator.instance.ActPose(i);
        behaviorType = (BehaviorType)i;
    }

    public void RecordStart()   //��ȭ ����
    {
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);
        gestureManager.StartSensing(behaviorType);
    }

    public void RecordFin(bool didFeedHam, bool isNewGesture = false) //��ȭ ����
    {
        DogAnimator.instance.animator.SetBool("petEat", didFeedHam);
        DogAnimator.instance.ActPose(-1);

        if (didFeedHam)
        {
            if (isNewGesture)
            {
                // ������ behaviorType ������
                // ������ ������ �����ϸ� �����ϱ�
                // ����� .onnx �޾ƿ���
                // ����ó�� ���� ������ �ൿ ���� (���ξ��� or �����)
            }
            else
            {
                // ������ behaviorType ������
                // ����� .onnx �޾ƿ���
                // ����ó�� ���� ������ �ൿ ���� (�����)
            }
        }
        
    }

    public void Wrong() //�ƴ� �ٸ� �ڼ� �ҰԿ�
    {
        DogAnimator.instance.ActPose(-1);
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);

    }

    public void HitBoxOnOff()
    {
        if (hitBoxes[0].activeSelf)
            for (int i = 0; i < 6; i++)
            {
                hitBoxes[i].SetActive(false);
            }
        else
            for (int i = 0; i < 6; i++)
            {
                hitBoxes[i].SetActive(true);
            }
    }
}
