using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

// ����ǥ ������Ʈ�� ����
public class DailyGesture : MonoBehaviour
{
    [SerializeField] private GestureManager gestureManager;
    [SerializeField] private GestureAI ai;
    [SerializeField] private BehaviorParameters param;

    private IEnumerator curCoroutine;

    private void OnEnable()
    {
        curCoroutine = DetectionCoroutine();
        StartCoroutine(curCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(curCoroutine);
    }

    private IEnumerator DetectionCoroutine()
    {
        while (true)
        {
            gestureManager.StartValidate();
            yield return new WaitUntil(() => gestureManager.CurrentBehaviorType != BehaviorType.Undecided);

            var behavior = gestureManager.CurrentBehaviorType;


            // �������� �ش� �ൿ�� ���� .onnx������ �����;� ��.
            // param.Model = 


            ai.enabled = true;
            yield return new WaitUntil(() => ai.Decision != -1);
            ai.EndEpisode();
            ai.enabled = false;

            if (ai.Decision == 0)   // ����
            {
                DogAnimator.instance.ActPose((int)behavior);
            }
            else if (ai.Decision == 1)  // �⺻ �ൿ (�����?)
            {
                DogAnimator.instance.ActPose(-1);
            }
            else // ����
            {
                DogAnimator.instance.ActPose(-1);
            }

            yield return new WaitForSeconds(1f);
            ai.enabled = false;
        }
    }
}
