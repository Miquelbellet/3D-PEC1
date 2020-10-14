using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarColliderController : MonoBehaviour
{
    public GameObject[] m_Wheels;
    private GameController m_GameController;
    private bool m_IsInTrack;
    private bool m_HasExitTrack;
    private float m_time;
    void Start()
    {
        m_GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (m_IsInTrack) GetComponent<Rigidbody>().drag = 0.1f;
        else GetComponent<Rigidbody>().drag = 0.4f;
    }

    private void FixedUpdate()
    {
        CheckRoadCollision();
    }

    private void CheckRoadCollision()
    {
        bool isInTrack = true;
        for (int i=0; i<m_Wheels.Length;i++)
        {
            var wheelPos = m_Wheels[i].transform.position;
            wheelPos = new Vector3(wheelPos.x, wheelPos.y + 0.5f, wheelPos.z);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(wheelPos, -Vector3.up, 10.0F);
            if (hits[1].collider.tag == "TrackCollider")
            {
                m_Wheels[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                isInTrack = false;
                m_Wheels[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        m_IsInTrack = isInTrack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint1") m_GameController.WatchCheckpoints(1);
        else if (other.tag == "Checkpoint2") m_GameController.WatchCheckpoints(2);
        else if (other.tag == "Checkpoint3") m_GameController.WatchCheckpoints(3);
        else if (other.tag == "Checkpoint4") m_GameController.WatchCheckpoints(4);
        else if (other.tag == "Checkpoint5") m_GameController.WatchCheckpoints(5);
    }
}
