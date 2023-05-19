using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogMove : MonoBehaviour
{
    public static DogMove instance;

    public Transform target;
    public NavMeshAgent agent;
    public Transform toy;
    public GameObject attentionMark;

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
        //qwer ���� 1, 2, 3 Ű���� ���� ������ ���� �ٸ� �ȱ� �������� ������� �ϱ�
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            agent.SetDestination(target.position);
            agent.speed = 1.5f;
            DogAnimator.instance.animator.SetInteger("moveSpeed", 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            agent.SetDestination(target.position);
            agent.speed = 1f;
            DogAnimator.instance.animator.SetInteger("moveSpeed", 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            agent.SetDestination(target.position);
            agent.speed = 3f;
            DogAnimator.instance.animator.SetInteger("moveSpeed", 2);
        }
        else if (agent.remainingDistance <= agent.stoppingDistance) //��������� ����
        {
            DogAnimator.instance.animator.SetInteger("moveSpeed", -1);
            //Debug.Log(0);
        }
        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            DogAnimator.instance.animator.SetInteger("moveSpeed", -1);
        }*/

        if (TrainManager.instance.trainMode)    //�Ʒø�� ���� �� ������
            return;

        if ((transform.position - target.position).magnitude < 3)
        {
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            //transform.rotation = Quaternion.Euler(0, transform.rotation.y * Mathf.Rad2Deg, 0);
            if (!attentionMark.activeSelf) attentionMark.SetActive(true);
        }
        else if (attentionMark.activeSelf)
        {
            attentionMark.SetActive(false);
        }

        if (Player.instance.ham.activeSelf) //���� ��� ������ �Ѿƿ�
        {
            if ((transform.position - target.position).magnitude > agent.stoppingDistance + 1)
            {
                agent.SetDestination(target.position);
                agent.stoppingDistance = 2f;
                agent.speed = 3f;
                DogAnimator.instance.animator.SetInteger("moveSpeed", 2);
            }
        }
        if (Player.instance.controller.activeSelf) //�峭�� �Ѿư�
        {
            if (!DogAnimator.instance.animator.GetBool("toyOn"))
                DogAnimator.instance.animator.SetBool("toyOn", true);
            if ((transform.position - toy.position).magnitude > agent.stoppingDistance + 1f)
            {
                agent.SetDestination(toy.position);
                agent.stoppingDistance = 1f;
                agent.speed = 3f;
                DogAnimator.instance.animator.SetInteger("moveSpeed", 2);
            }
        }
        else
            DogAnimator.instance.animator.SetBool("toyOn", false);
    }
}
