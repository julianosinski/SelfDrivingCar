using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    //EventHandler is buildin delagate
    public event EventHandler<CheckpointEventArgs> OnEnteringCorrectCheckPoint;
    public event EventHandler<CheckpointEventArgs> OnEnteringWrongCheckPoint;

    public List<Transform> participantsList = new List<Transform>();
    //Dictionary thats hold index of next checkpoint to colect by participant
    private Dictionary<Transform, int> participantNextCheckpointIndex= new Dictionary<Transform, int>();
    private List<Checkpoint> checkpointsList = new List<Checkpoint>();
    public class CheckpointEventArgs
    {
        public Transform checkpointParticipantTransform { get; set; }
    }


    private void Awake()
    {
        if (!PopulateparticipantNextCheckpointIndex(participantsList)) { Debug.Log("PopulateparticipantNextCheckpointIndexfaild!"); return; }
        if (!PopulateCheckpointsList()) { Debug.Log("PopulateCheckpointsList faild!"); return; }
    }
    
    #region hellpers
    private bool PopulateparticipantNextCheckpointIndex(List<Transform> participantTransforms)
    {
        foreach (var transform in participantTransforms)
        {
            participantNextCheckpointIndex[transform] = 0;
        }
        return participantNextCheckpointIndex.Count == 0 ?  false : true;
    }

    private bool PopulateCheckpointsList()
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
    //Called by invidual Checkpoint to inform that somebody entered it
    public void CheckpointEntered(Checkpoint checkpoint, Transform participantTransform)
    {
        if (checkpointsList.IndexOf(checkpoint) == participantNextCheckpointIndex[participantTransform])
        {
            Debug.Log("Correct: " + checkpointsList.IndexOf(checkpoint) + ", expected: " + participantNextCheckpointIndex[participantTransform]);
            if(participantNextCheckpointIndex[participantTransform] < checkpointsList.Count - 1)
            {
                ResetParticipantProgres(participantTransform);
            }
            else
            {
                participantNextCheckpointIndex[participantTransform]++;
            }
            OnEnteringCorrectCheckPoint?.Invoke(this, new CheckpointEventArgs { checkpointParticipantTransform = participantTransform });
        }
        else
        {
            Debug.Log("Wrong: " + checkpointsList.IndexOf(checkpoint) + ", expected: " + participantNextCheckpointIndex[participantTransform]);
            OnEnteringWrongCheckPoint?.Invoke(this, new CheckpointEventArgs { checkpointParticipantTransform = participantTransform});
        }
    }
    //Extra methods for members outside Checkpoint system alows for controling and geting state. 
    public Checkpoint GetNextCheckPoint(Transform participantTransform)
    {
        int participantPosition = GetParticipantPosition(participantTransform);
        if (participantPosition < checkpointsList.Count - 1) { return checkpointsList[participantPosition]; }
        return checkpointsList[0];
    }
    public void ResetParticipantProgres(Transform participantTransform)
    {
        participantNextCheckpointIndex[participantTransform] = 0;
    }
    public int GetParticipantPosition(Transform participantTransform)
    {
        return participantNextCheckpointIndex[participantTransform];
    }
}
