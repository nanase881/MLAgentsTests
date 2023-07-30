using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerMove : Agent
{
    Rigidbody rb;
    public Transform ClearSpace;
    //public Transform Obstacle;
    public int moveSpeed = 5;
    //public int jumpForce = 6;
    //public bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.position.y < 0 || this.transform.position.x > 50f || this.transform.position.x < -1f)
        {
            this.transform.position = new Vector3(0, 0.8f, 0);
        }
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(ClearSpace.position);
        //sensor.AddObservation(Obstacle.position);
        sensor.AddObservation(this.rb.velocity.x);
        sensor.AddObservation(this.rb.velocity.y);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.y = actions.ContinuousActions[1];
        rb.velocity = new Vector3(controlSignal.x * moveSpeed, controlSignal.y * moveSpeed, rb.velocity.z);


        float distanceToClearSpace = Vector3.Distance(this.transform.position, ClearSpace.position);
        
        if(distanceToClearSpace < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (this.transform.position.y < -0.1f || this.transform.position.x > 50f || this.transform.position.x < -1f)
        {
            EndEpisode();
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    rb.velocity = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y, rb.velocity.z);

    //    if(Input.GetKeyDown(KeyCode.Space) && !(rb.velocity.y < -0.5f))
    //    {

    //        Jump();
    //    }
    //}

    //void Jump()
    //{
    //    isJumping = true;
    //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Ground"))
    //    {
    //        isJumping = false;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            //Debug.Log("enemy!");
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        rb.velocity = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical")*moveSpeed, rb.velocity.z);

        if (Input.GetKeyDown(KeyCode.Space) )
        {
            //Jump();
        }
    }
}
