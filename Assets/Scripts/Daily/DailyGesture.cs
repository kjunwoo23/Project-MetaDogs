using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

// ����ǥ ������Ʈ�� ����
public class DailyGesture : MonoBehaviour
{
    [SerializeField] private GestureManager gestureManager;
    [SerializeField] private GestureAI ai;

    private IEnumerator curCoroutine;

    private void OnEnable()
    {
        curCoroutine = DetectionCoroutine();
        StartCoroutine(curCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(curCoroutine);
        DogAnimator.instance.animator.SetBool("dailyRecordStart", false);
    }

    private IEnumerator DetectionCoroutine()
    {
        while (true)
        {
            gestureManager.StartValidate();
            yield return new WaitUntil(() => gestureManager.CurrentBehaviorType != BehaviorType.Undecided);

            var behavior = gestureManager.CurrentBehaviorType;

            if (behavior == BehaviorType.None)  // �ش� ����ó�� ������ ����
            {
                DogAnimator.instance.animator.SetInteger("petPose", -1);
                continue;
            }

            // �������� �ش� �ൿ�� ���� .onnx������ �����;� ��.
            // param.Model = 

            ai.Decision = -1;
            int count = 0;
            while (ai.Decision == -1)
            {
                ai.RequestDecision();
                yield return Time.fixedDeltaTime;
                ++count;
                if (count > 100) break;
            }

            Debug.Log(ai.Decision);
            if (ai.Decision == 0)   // ����
            {
                DogAnimator.instance.animator.SetInteger("petPose", (int)behavior);
                Debug.Log(behavior.ToString());
            }
            else if (ai.Decision == 1)  // �⺻ �ൿ (�����?)
            {
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }
            else // ����
            {
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }

            yield return new WaitForSeconds(3f);
            DogAnimator.instance.animator.SetBool("loopEnd", true);
        }
    }
}
