using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Race Configuration")]
    public GameObject m_Car;
    public GameObject[] m_Checkpoints;
    public int m_NumberOfLaps;

    [Header("Ghost Car")]
    public GameObject m_ghostCarPrefab;
    public float m_frequencyBetweenSamples;

    private GhostCarScript m_GhostCarScript;

    private int m_currentLap = 1;
    private int m_currentCheckpoint = 1;
    private int m_currentGhostSample = 1;
    private int m_ghostNumSamples;

    private bool m_passedCheckpointCtr = true;
    private bool m_RaceStarted;
    private bool m_isGhostCarData;
    private bool m_stopGhost;
    private bool m_ghostStarted;

    private float m_BestTimeEver;
    private float m_currentTimeBetweenSamples;
    private float m_currenttimeBetweenPlaySamples;
    private float m_ghostFrequencySamples;

    private Vector3 m_lastSamplePosition;
    private Vector3 m_nextPosition;
    private Quaternion m_lastSampleRotation;
    private Quaternion m_nextRotation;
    private Vector3 m_initCarPos;
    private Quaternion m_initCarRot;

    void Start()
    {
        m_GhostCarScript = ScriptableObject.CreateInstance("GhostCarScript") as GhostCarScript;
        m_BestTimeEver = PlayerPrefs.GetFloat("BestTime", 0f);
        m_isGhostCarData = CheckIfGhostCarData();
        if (m_isGhostCarData)
        {
            m_GhostCarScript.ReadGhostCarTXT();
            GameObject check1 = GameObject.FindGameObjectWithTag("Checkpoint1");
            m_lastSamplePosition = m_initCarPos = new Vector3(check1.transform.position.x, m_Car.transform.position.y, check1.transform.position.z);
            m_lastSampleRotation = m_initCarRot = m_Car.transform.rotation;
            m_GhostCarScript.GetDataAt(0, out m_nextPosition, out m_nextRotation);
            m_ghostNumSamples = m_GhostCarScript.GetNumGhostSamples();
            m_ghostFrequencySamples = m_GhostCarScript.GetGhostFrequencySamples();
        }
    }

    void Update()
    {
        if (m_RaceStarted)
        {
            SaveGhostCarInfo();
            if (m_isGhostCarData)
            {
                m_ghostCarPrefab.SetActive(true);
                ShowGhostCar();
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_RaceStarted && m_isGhostCarData)
        {
            float percentageBetweenFrames = m_currenttimeBetweenPlaySamples / m_ghostFrequencySamples;
            m_ghostCarPrefab.transform.position = Vector3.Slerp(m_lastSamplePosition, m_nextPosition, percentageBetweenFrames);
            m_ghostCarPrefab.transform.rotation = Quaternion.Slerp(m_lastSampleRotation, m_nextRotation, percentageBetweenFrames);

            if (m_ghostStarted)
            {
                var distance = Vector3.Distance(m_ghostCarPrefab.transform.position, m_initCarPos);
                if (distance < 0.1f)
                {
                    m_isGhostCarData = false;
                    m_ghostCarPrefab.SetActive(false);
                }
            }
        }
        
    }

    private void PassCheckpoint(int numCheckpoint)
    {
        m_currentCheckpoint = numCheckpoint;
        GetComponent<UIController>().CorrectPath(true);
        GetComponent<UIController>().Checkpoint(numCheckpoint);
    }

    private void FinalRace()
    {
        Debug.Log("FINAL");
    }

    public void WatchCheckpoints(int numCheckpoint)
    {
        //Compobar que nomes passa per aqui 1 cop ja que el cotxe te varis colliders
        if (!m_passedCheckpointCtr) return;
        else
        {
            m_passedCheckpointCtr = false;
            Invoke("CheckpointPassed", 3);
        }

        //final de volta
        if (m_currentCheckpoint == 5 && numCheckpoint == 1)
        {
            if (m_currentLap == 3)
            {
                GetComponent<UIController>().SetLapTime(m_currentLap);
                CheckGhostTime();
                FinalRace();
                return;
            }
            else
            {
                GetComponent<UIController>().SetLapTime(m_currentLap);
                m_currentLap++;
                PassCheckpoint(numCheckpoint);
                CheckGhostTime();
                return;
            }
        }

        //Començament de la carrera, per activar el temps
        if (numCheckpoint == 1 && m_currentLap == 1)
        {
            m_RaceStarted = true;
            GetComponent<UIController>().Checkpoint(numCheckpoint);
            return;
        }

        //sumar 1 al current checkpoint
        if (numCheckpoint == m_currentCheckpoint + 1)
        {
            PassCheckpoint(numCheckpoint);
        }
        else
        {
            GetComponent<UIController>().CorrectPath(false);
        }
    }

    private void CheckpointPassed()
    {
        m_passedCheckpointCtr = true;
    }

    private void SaveGhostCarInfo()
    {
        m_currentTimeBetweenSamples += Time.deltaTime;
        if (m_currentTimeBetweenSamples >= m_frequencyBetweenSamples)
        {
            m_GhostCarScript.AddNewData(m_Car.transform);
            m_currentTimeBetweenSamples -= m_frequencyBetweenSamples;
        }
    }

    private void ShowGhostCar()
    {
        m_currenttimeBetweenPlaySamples += Time.deltaTime;
        if (m_currentGhostSample >= m_ghostNumSamples)
        {
            if (m_stopGhost) return;
            m_stopGhost = true;
            ShowFinalGhostPart();
            return;
        }

        if (m_currenttimeBetweenPlaySamples >= m_ghostFrequencySamples)
        {
            m_ghostStarted = true;

            m_lastSamplePosition = m_nextPosition;
            m_lastSampleRotation = m_nextRotation;

            m_GhostCarScript.GetDataAt(m_currentGhostSample, out m_nextPosition, out m_nextRotation);
            m_currenttimeBetweenPlaySamples -= m_ghostFrequencySamples;
            m_currentGhostSample++;
        }
    }

    private void ShowFinalGhostPart()
    {
        m_nextPosition = m_initCarPos;
        m_nextRotation = m_initCarRot;
    }

    private void CheckGhostTime()
    {
        var currentTimer = GetComponent<UIController>().GetCurrentTimer();
        if (currentTimer < m_BestTimeEver || m_BestTimeEver == 0)
        {
            PlayerPrefs.SetFloat("BestTime", m_BestTimeEver);
            m_GhostCarScript.WriteGhostCarTXT(m_frequencyBetweenSamples);
        }
        m_GhostCarScript.Reset();
    }

    private bool CheckIfGhostCarData()
    {
        return m_GhostCarScript.CheckGhostCarData();
    }
}
