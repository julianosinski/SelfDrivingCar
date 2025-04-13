using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class SelfDrivingMLAgent : Agent
{
    [SerializeField] private CheckpointsManager checkpointsManager;
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Transform spawnPos;

    private ModCarController carController;
    private CollisionReporter collisionReporter;

    protected override void Awake()
    {
        base.Awake();
        carController = GetComponent<ModCarController>();
        collisionReporter = carRb.GetComponent<CollisionReporter>();
    }

    private void Start()
    {
        checkpointsManager.OnEnteringCorrectCheckPoint += CheckpointsManager_OnEnteringCorrectCheckPoint;
        checkpointsManager.OnEnteringWrongCheckPoint += CheckpointsManager_OnEnteringWrongCheckPoint;
        collisionReporter.OnCollisionEnterReporter += CollisionReporter_OnCollisionEnterReporter;
        collisionReporter.OnCollisionStayReporter += CollisionReporter_OnCollisionStayReporter;
    }

    #region Reward   
    private void CollisionReporter_OnCollisionStayReporter(object sender, Collision e)
    {
        if(carRb.transform == e.transform)
        {
            AddReward(-0.5f);
        }
    }

    private void CollisionReporter_OnCollisionEnterReporter(object sender, Collision e)
    {
        if (carRb.transform == e.transform)
        {
            AddReward(-0.1f);
        }
    }

    private void CheckpointsManager_OnEnteringCorrectCheckPoint(object sender, CheckpointsManager.CheckpointEventArgs e)
    {
        if(e.checkpointParticipantTransform == transform)
        {
            AddReward(1f);
        } 
    }
    private void CheckpointsManager_OnEnteringWrongCheckPoint(object sender, CheckpointsManager.CheckpointEventArgs e)
    {
        if (e.checkpointParticipantTransform == transform)
        {
            AddReward(-1f);
        }
    }



    #endregion
    public override void OnEpisodeBegin()
    {
        transform.position = spawnPos.position + new Vector3(Random.Range(-5, 5f), 0f, Random.Range(-5, 5f));
        transform.forward = spawnPos.forward;
        checkpointsManager.ResetParticipantProgres(transform);
        carController.StopMovement();
    }
    /*
     * sensor (our case RayPerceptionSensorComponent3D) handles most of observations 
     * but its worth to add one more that tells how much car is facing new checkpoint
    */
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkPointForward = checkpointsManager.GetNextCheckPoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkPointForward);
        sensor.AddObservation(directionDot);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float accelerationAmount = 0;
        float turnAmount = 0;

        //Using Discrete Action it means that model will return keybord like action (all or nothing)
        switch (actions.DiscreteActions[0])
        {
            case 0: accelerationAmount = 0f; break;
            case 1: accelerationAmount = 1f; break;
            case 2: accelerationAmount = -1f; break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = 1f; break;
            case 2: turnAmount = -1f; break;
        }

        carController.SetInputVector(new Vector2(turnAmount, accelerationAmount));
    }

    
}
