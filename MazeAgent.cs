using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class MazeAgent : Agent
{
    public Transform target;  // The target the agent needs to reach
    public float moveSpeed = 1.0f;  // Speed of the agent
    private Rigidbody2D rb;

    // Called once at the start
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called at the start of each episode
    public override void OnEpisodeBegin()
    {
        // Reset positions (optional: you can remove randomization)
        transform.localPosition = new Vector2(-4.31f, -3.73f);  // Fixed position for the agent
        target.localPosition = new Vector2(3.64f, 2.96f);  // Fixed position for the target
    }

    // Collect observations from the environment
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agent's position
        sensor.AddObservation(transform.localPosition);

        // Observe the target's position
        sensor.AddObservation(target.localPosition);

        // Observe the direction to the target
        sensor.AddObservation((target.localPosition - transform.localPosition).normalized);
    }

    // Apply actions received from the neural network
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Debug.Log($"MoveX: {moveX}, MoveY: {moveY}, Velocity: {rb.velocity}");

        // Apply movement using Rigidbody2D
        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;
        rb.velocity = movement;
    }

    // Optional: allow manual control for testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    // Handling collision with objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            // Reward the agent for reaching the target
            SetReward(1.0f);
            EndEpisode();  // End the episode
        }
        else if (other.CompareTag("Wall"))
        {
            // Penalize the agent for hitting a wall
            SetReward(-1.0f);
            EndEpisode();  // End the episode
        }
    }
}