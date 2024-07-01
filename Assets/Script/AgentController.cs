using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 4f;

    private int successCount = 0;
    private int episodeCount = 0;

    private float stepCount;

    private float maxStep = 1000;

    public override void OnEpisodeBegin()
    {
        episodeCount++;
        //agent
        //transform.localPosition = new Vector3(Random.Range(-4f,4f), 0.3f, Random.Range(-4f,4f));
        transform.localPosition = new Vector3(-9f, 0.3f, 8f);

        //pellet
        target.localPosition = new Vector3(9.3f, 0.3f, 9f);

        stepCount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        moveSpeed = 1f;

        Vector3 velocity = new Vector3(moveX, 0f, moveZ);
        velocity= velocity.normalized * Time.deltaTime * moveSpeed;

        transform.localPosition += velocity; 

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        float reward = 1.0f / distanceToTarget;
        SetReward(reward);


        if (stepCount >= maxStep)
        {
            SetReward(-1.0f);
            EndEpisode();
        }


        if (transform.localPosition.y < -1)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet")
        {
            AddReward(2f);
            EndEpisode();
        }

        if (other.gameObject.tag == "Wall")
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
    
    private void OnDestroy()
    {
        float successRate = (float)successCount / episodeCount;
        Debug.Log("Başarı Oranı: " + successRate);
    }
}
