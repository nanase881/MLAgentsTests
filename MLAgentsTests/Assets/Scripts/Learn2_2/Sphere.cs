using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Sphere : Agent
{

    Rigidbody rb;
    bool reached1;
    bool reached2;
    float minutes = 30f;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public Transform Cube;

    public override void OnEpisodeBegin()
    {
        this.rb.angularVelocity = Vector3.zero;
        this.rb.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        reached1 = false;
        reached2 = false;
        timer = 0;


    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= minutes)
        {
            SetReward(-0.5f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Cube.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);

    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.y = actions.ContinuousActions[1];
        //rb.AddForce(controlSignal * forceMultiplier);
        rb.velocity = new Vector3(controlSignal.x * forceMultiplier, controlSignal.y * forceMultiplier, rb.velocity.z);

        float distanceToCube = Vector3.Distance(this.transform.localPosition, Cube.localPosition);

        if (distanceToCube < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 6 || this.transform.localPosition.x < -3)
        {
            SetReward(-0.3f);
            EndEpisode();
        }

        //if (distanceToCube < 13f)
        //{
        //    if (!(reached1))
        //    {
        //        reached1 = true;
        //        SetReward(0.5f);
        //    }
           
        //}

        //if (distanceToCube < 20f)
        //{
        //    if (!(reached2))
        //    {
        //        reached2 = true;
        //        SetReward(0.2f);
        //    }
           
        //}
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("enemy");
            SetReward(-0.1f);
            EndEpisode();
        }
    }

   

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}
