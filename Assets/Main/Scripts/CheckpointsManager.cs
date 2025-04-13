using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    //EventHandler is buildin delagate
    public event EventHandler<CheckpointEventArgs> OnEnteringCorrectCheckPoint;
    public event EventHandler<CheckpointEventArgs> OnEnteringWrongCheckPoint;

    public List<Transform> carsList = new List<Transform>();

    private Dictionary<Transform, int> carCheckpointStageMap = new Dictionary<Transform, int>();
    private List<Checkpoint> checkpointsList = new List<Checkpoint>();
    public class CheckpointEventArgs
    {
        public Transform checkpointPassedTransform { get; set; }
    }


    private void Awake()
    {
        if (!populateCarCheckpointStageMap(carsList)) { Debug.Log("populateCarCheckpointStageMap faild!"); return; }
        if (!populateCheckpointsList()) { Debug.Log("populateCheckpointsList faild!"); return; }
    }
    #region hellpers
    private bool populateCarCheckpointStageMap(List<Transform> carTransforms)
    {
        foreach (var transform in carTransforms)
        {
            carCheckpointStageMap[transform] = 0;
        }
        return carCheckpointStageMap.Count == 0 ?  false : true;
    }

    private bool populateCheckpointsList()
    {
        Transform checkpointsTransforms = this.transform;
        foreach (Transform checkpointTransform in checkpointsTransforms)
        {
            Checkpoint checkpoint = checkpointTransform.GetComponent<Checkpoint>();
            checkpoint.SetCheckpointsManagerRef(this);
            checkpointsList.Add(checkpoint);
        }
        return checkpointsList.Count == 0 ? false: true;
    }
    #endregion
    public void CheckpointEntered(Checkpoint checkpoint, Transform carTransform)
    {
        if (checkpointsList.IndexOf(checkpoint) == carCheckpointStageMap[carTransform])
        {
            Debug.Log("Correct: " + checkpointsList.IndexOf(checkpoint) + ", expected: " + carCheckpointStageMap[carTransform]);
            carCheckpointStageMap[carTransform] = (carCheckpointStageMap[carTransform] < checkpointsList.Count-1) ? carCheckpointStageMap[carTransform] + 1 : 0;
            OnEnteringCorrectCheckPoint?.Invoke(this, new CheckpointEventArgs { checkpointPassedTransform = carTransform });
        }
        else
        {
            Debug.Log("Wrong: " + checkpointsList.IndexOf(checkpoint) + ", expected: " + carCheckpointStageMap[carTransform]);
            OnEnteringWrongCheckPoint?.Invoke(this, new CheckpointEventArgs { checkpointPassedTransform = carTransform});
        }
    }
}
