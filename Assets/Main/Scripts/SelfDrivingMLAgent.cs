using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class SelfDrivingMLAgent : Agent
{
    [SerializeField] private CheckpointsManager checksManager;
    [SerializeField] private Transform spawnPos;

    private ModCarController carController;

    private void Awake()
    {
        carController = GetComponent<ModCarController>();
    }

    private void Start()
    {
        checksManager.OnEnteringCorrectCheckPoint += CheckpointsManager_OnEnteringCorrectCheckPoint;
        checksManager.OnEnteringWrongCheckPoint += CheckpointsManager_OnEnteringWrongCheckPoint;
    }
    #region Reward   
    private void CheckpointsManager_OnEnteringCorrectCheckPoint(object sender, CheckpointsManager.CheckpointEventArgs e)
    {
        if(e.checkpointPassedTransform == transform)
        {
            AddReward(1f);
        } 
    }
    private void CheckpointsManager_OnEnteringWrongCheckPoint(object sender, CheckpointsManager.CheckpointEventArgs e)
    {
        if (e.checkpointPassedTransform == transform)
        {
            AddReward(-1f);
        }
    }
    #endregion
    public override void OnEpisodeBegin()
    {
        transform.position = spawnPos + new Vector3(Random.Range(-5, +5f), 0, (Random.Range(-5, +5f));
        transform.forward = spawnPos.forward;
        //checksManager.Restart(transform)
        //carDriver.Stop
    }

}
