using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public class DecisionBehaviour : MonoBehaviour
{
    private GestureAI ai;
    private BehaviorParameters behavior;
    private DecisionRequester adecision;

    // Start is called before the first frame update
    void Awake()
    {
        ai = GetComponent<GestureAI>();
        behavior = GetComponent<BehaviorParameters>();
        adecision = GetComponent<DecisionRequester>();
        ai.enabled = false;
        behavior.enabled = false;
        adecision.enabled = false;
    }

    public void SetDogBehavior(BehaviorType type)
    {
        StartCoroutine(GetBehaviorCoroutine(type));
    }

    private IEnumerator GetBehaviorCoroutine(BehaviorType type)
    {
        // �������� �ش� ����ó�� ���� �ൿ�� ���� ���� �����´�.
        // ���� �� �Ʒ� ����

        ai.enabled = true;
        behavior.enabled = true;
        adecision.enabled = true;
        yield return new WaitUntil(() => ai.Decision != -1);

        if (ai.Decision == 0)   // ����
        {
            // ������ �ൿ
        }
        else if (ai.Decision == 1)  // �����
        {
            // ������ �ൿ
        }
        else  // ����
        {
            // ������ �ൿ
        }
        ai.enabled = false;
        behavior.enabled = false;
        adecision.enabled = false;
    }
}
