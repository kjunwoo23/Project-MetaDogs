using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class GestureAI : Agent
{
    [SerializeField]  private GestureManager gestureManager;

    public override void Initialize()
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        GestureType resultDogGesture = (GestureType)actions.DiscreteActions[0];

        // ���丸 SetReward(10f);
        // �⺻���� SetReward(0f);
        // Ʋ�� �� SetReward(-10f);
        switch (resultDogGesture)
        {
            case GestureType.None:
                break;
            case GestureType.SitDown:
                break;
            case GestureType.SitSide:
                break;
            case GestureType.Lie:
                break;
            case GestureType.Jump:
                break;
            case GestureType.Die:
                break;
            case GestureType.Attack:
                break;
            default:
                break;
        }
        
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionOut = actionsOut.DiscreteActions;
        // ������ �ൿ �ο�

        //if (Input.GetKey(KeyCode.A))
        //{
        //    rb.AddForce(speed * Vector3.right);
        //    discreteActionOut[0] = 1;
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    rb.AddForce(speed * Vector3.left);
        //    discreteActionOut[0] = 2;
        //}
        //else
        //{
        //    discreteActionOut[0] = 0;
        //}
    }

    // ������ ���
    public override void CollectObservations(VectorSensor sensor)
    {
        // Space size = 2
        // +) ������ �ൿ ����
        sensor.AddObservation((int)gestureManager.CurrentType);
    }

    public override void OnEpisodeBegin()
    {
        // ������ ���� ����
    }
}
