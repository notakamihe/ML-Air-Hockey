using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class StrikerAgent : Agent
{
    public Team team;
    [HideInInspector] public Vector3 startPos;

    Field field;
    [HideInInspector] public Rigidbody rb;
    Transform side;
    Transform sideDeep;

    bool isMovingTowardPuck = false;

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Puck"))
        {
            Puck puck = collision.gameObject.GetComponent<Puck>();

            Vector3 hitDirection = (collision.GetContact(0).point - transform.position).normalized;
            puck.rb.AddForce(hitDirection * AirHockeySettings.instance.hitForce * Time.deltaTime);

            AddReward(1f);

            if (Vector3.Dot(hitDirection, team.goal.transform.position) < 0)
            {
                AddReward(1f);
            }
        }
        
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.2f);
        }
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        field = GetComponentInParent<Field>();
        side = transform.parent;
        sideDeep = side.GetChild(side.childCount - 1);
        startPos = transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(field.puck.HasServed);
        sensor.AddObservation(side.GetComponent<Collider>().bounds.Contains(field.puck.transform.position));
        sensor.AddObservation(Vector3.Distance(field.puck.transform.position, transform.position));
        sensor.AddObservation((field.transform.position - transform.position).normalized);
        sensor.AddObservation(field.puck.rb.velocity);
        sensor.AddObservation(rb.velocity.magnitude);
        sensor.AddObservation(sideDeep.GetComponent<Collider>().bounds.Contains(transform.position));
        sensor.AddObservation(isMovingTowardPuck);
    }

    public override void OnEpisodeBegin()
    {
        if (field.puck.HasServed)
            field.ResetField(Random.Range(1, 3) == 1 ? field.rightTeam.striker : field.leftTeam.striker);
    }

    public override void Heuristic(float[] actionsOut)
    {
        if (Input.GetKey(KeyCode.A))
            actionsOut[0] = 0;
        else if (Input.GetKey(KeyCode.D))
            actionsOut[0] = 1;
        else
            actionsOut[0] = 2;

        if (Input.GetKey(KeyCode.W))
            actionsOut[1] = 0;
        else if (Input.GetKey(KeyCode.S))
            actionsOut[1] = 1;
        else
            actionsOut[1] = 2;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        int horizontal = 0;
        int vertical = 0;

        if (vectorAction[0] == 0.0f)
            horizontal = -1;
        else if (vectorAction[0] == 1.0f)
            horizontal = 1;

        if (vectorAction[1] == 0.0f)
            vertical = 1;
        else if (vectorAction[1] == 1.0f)
            vertical = -1;

        Vector3 moveDirection = transform.right * horizontal * AirHockeySettings.instance.strikerSpeed;

        if (field.puck.HasServed)
        {
            if (side.GetComponent<Collider>().bounds.Contains(transform.position))
            {
                moveDirection += transform.forward * vertical * AirHockeySettings.instance.strikerSpeed;
            }
            else
            {
                Vector3 newVelocity = rb.velocity;
                newVelocity.z = 0;
                rb.velocity = newVelocity;

                rb.MovePosition((transform.position + transform.forward * -0.1f));
            }

            rb.AddForce(moveDirection * Time.deltaTime, ForceMode.VelocityChange);
        }

        if (IsPositionOnSide(field.puck.transform.position) && field.puck.HasServed)
            AddReward(-0.001f);

        if (rb.velocity.magnitude <= AirHockeySettings.instance.strikerSpeedThreshold && IsPositionOnSide(field.puck.transform.position))
        {
            AddReward(-0.002f);
        }

        if (Physics.SphereCast(transform.position, 1.0f, rb.velocity, out RaycastHit hit, 30.0f))
        {
            isMovingTowardPuck = hit.transform.CompareTag("Puck");

            if (!isMovingTowardPuck)
                AddReward(-1 / MaxStep);
            else
                AddReward(0.005f);
        }

        if (Vector3.Distance(field.puck.transform.position, transform.position) > AirHockeySettings.instance.maxDistanceFromPuckForReward)
        {
            AddReward(-1 / MaxStep);
        } else
        {
            AddReward(0.002f);
        }
    }

    public bool IsPositionOnSide(Vector3 pos) => side.GetComponent<Collider>().bounds.Contains(pos);
    public bool IsPositionOnSideDeep(Vector3 pos) => sideDeep.GetComponent<Collider>().bounds.Contains(pos);
}
