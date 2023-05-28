using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;
using Unity.Barracuda;
using Unity.Barracuda.ONNX;
using System.IO;

// ����ǥ ������Ʈ�� ����
public class DailyGesture : MonoBehaviour
{
    [SerializeField] private GestureManager gestureManager;
    [SerializeField] private GestureAI ai;

    private IEnumerator curCoroutine;

    private void OnEnable()
    {
        DogAnimator.instance.animator.SetBool("dailyRecordStart", true);
        //Debug.Log("�� �ȳ���!!!!!!!");
        curCoroutine = DetectionCoroutine();
        StartCoroutine(curCoroutine);
    }

    private void OnDisable()
    {
        gestureManager.InitText();
        StopCoroutine(curCoroutine);
        DogAnimator.instance.animator.SetBool("dailyRecordStart", false);
        if (DogAnimator.instance.animator.GetInteger("petPose") != -1)
            DogAnimator.instance.animator.SetTrigger("loopEnd");
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

            RequestManager.Instance.StartCoroutine("LoadAIModel", (int)behavior);

            var ONNXPath = Application.streamingAssetsPath + @"\model_" + (int)behavior + ".onnx";
            var converter = new ONNXModelConverter(optimizeModel: true); // requires the Unity.Barracuda.ONNX assembly

            // Read file at path and convert to byte array
            byte[] modelData = File.ReadAllBytes(ONNXPath);
            var model = converter.Convert(modelData); // type is Unity.Barracuda.Model

            ai.Decision = -1;
            ai.IsStart = true;
            while (ai.Decision == -1)
            {
                ai.RequestDecision();
                yield return Time.fixedDeltaTime;
            }
            ai.IsStart = false;

            Debug.Log(ai.Decision);
            if (ai.Decision == 0)   // ����
            {
                DogAnimator.instance.animator.SetInteger("petPose", (int)behavior);
                Debug.Log(behavior.ToString());
            }
            else if (ai.Decision == 1)  // �⺻ �ൿ (�����?)
            {
                gestureManager.SetTextUnrecognizable();
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }
            else // ����
            {
                gestureManager.SetTextUnrecognizable();
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }

            yield return new WaitForSeconds(3f);
            DogAnimator.instance.animator.SetTrigger("loopEnd");
        }
    }
}
