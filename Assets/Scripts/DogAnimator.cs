using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogAnimator : MonoBehaviour
{
    public static DogAnimator instance;
    public Animator animator;
    public bool[] petPosesEnabled = new bool[6];

    /*
    [0] Crouch
    [1] Death
    [2] Jump
    [3] Lie
    [4] Sitting
    [5] Attack 
     */

    public Animator trainUIAnimator;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {   //Ű���� ���� Ű�е� ���� ������ ���� ���� ����
        if (Input.GetKeyDown(KeyCode.Keypad0))
            ActPose(0);
        else if (Input.GetKeyDown(KeyCode.Keypad1))
            ActPose(1);
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            ActPose(2);
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            ActPose(3);
        else if (Input.GetKeyDown(KeyCode.Keypad4))
            ActPose(4);
        else if (Input.GetKeyDown(KeyCode.Keypad5))
            ActPose(5);
        else if (Input.GetKeyDown(KeyCode.P))   //Ű���� p ������ �������� ���� �׸�
            ActPose(-1);
    }

    public void RandIdle()  //�������� �� 4���� ���ڸ� ���� �� �ϳ� ���� ���
    {
        animator.SetInteger("randIdle", Random.Range(0, 5));
    }

    public void PoseReady() //PoseReady animation���� key�� ����
    {
        animator.SetBool("petEat", false);
    }

   
    public void ActPose(int i)
    {   //�� i��° ���� ���
        if (i == -1)    //i�� -1�̸� ���� ����
        {
            animator.SetInteger("petPose", i);
            animator.SetTrigger("loopEnd");
            TrainManager.instance.hamNoticed = false;
            return;
        }
        animator.SetInteger("petPose", i);
        if (i == 4)
            Invoke("TrainUIAppear", 2);
        else if (i == 0)
            Invoke("TrainUIAppear", 3);
        else if (i == 2 || i == 3)
            Invoke("TrainUIAppear", 4);
        else if (i == 1 || i == 5)
            Invoke("TrainUIAppear", 5);
    }

    public void PoseReset()
    {   //���� ��� �� ����Ŭ ������, �ʱ�ȭ
        animator.SetInteger("petPose", -1);
        trainUIAnimator.SetBool("appear", false);
        if (!Player.instance.canvas.gameObject.activeSelf)
            Player.instance.laser.SetActive(false);
    }

    public void TrainUIAppear()
    {   //�Ʒø�� UI ����
        trainUIAnimator.SetBool("appear", true);
        Player.instance.laser.SetActive(true);
    }
}
