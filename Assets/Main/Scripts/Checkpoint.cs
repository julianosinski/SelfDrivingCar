using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private CheckpointsManager checkpointsManager;

    private void OnTriggerEnter(Collider other)
    {
        if (checkpointsManager && other.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            checkpointsManager.CheckpointEntered(this, other.transform);
        }
    }

    public void SetCheckpointsManagerRef(CheckpointsManager checkpointsManager)
    {
        this.checkpointsManager = checkpointsManager;
    }
}
