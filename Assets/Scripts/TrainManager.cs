using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    public static TrainManager instance;
    public bool trainMode;

    public float maxHamJudgeTime;
    public bool hamNoticed;

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
    }

    public void RecordStart()   //��ȭ ����, ���� '�� ��'
    {
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);

    }

    public void RecordFin() //��ȭ ����
    {

        DogAnimator.instance.animator.SetBool("petEat", true); //�ȳ� ����
        DogAnimator.instance.ActPose(-1);
    }

    public void Wrong() //�ƴ� �ٸ� �ڼ� �ҰԿ�
    {
        DogAnimator.instance.ActPose(-1);
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);

    }
}
