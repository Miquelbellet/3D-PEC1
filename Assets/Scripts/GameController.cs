using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_Car;
    public GameObject[] m_Checkpoints;
    public int m_NumberOfLaps;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void WatchCheckpoints(int numCheckpoint)
    {
        GetComponent<UIController>().Checkpoint(numCheckpoint);
    }
}
