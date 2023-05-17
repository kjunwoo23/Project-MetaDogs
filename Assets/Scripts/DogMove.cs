using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogMove : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

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

        if (Player.instance.ham.activeSelf) //���� ��� ������ �Ѿƿ�
        {
            if ((DogAnimator.instance.gameObject.transform.position - target.position).magnitude > agent.stoppingDistance + 1)
            {
                agent.SetDestination(target.position);
                agent.speed = 3f;
                DogAnimator.instance.animator.SetInteger("moveSpeed", 2);
            }
        }
    }
}
