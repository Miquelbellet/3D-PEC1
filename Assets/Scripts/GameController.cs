using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Race Configuration")]
    public GameObject m_Car;
    public GameObject m_MainCamera;
    public GameObject[] m_Checkpoints;
    public int m_NumberOfLaps;

    [Header("Ghost Car")]
    public GameObject m_ghostCarPrefab;
    public float m_frequencyBetweenSamples;

    [Header("Map Configuration")]
    public GameObject[] m_Maps;
    public GameObject[] m_Cars;

    private GhostCarScript m_GhostCarScript;

    private int m_currentLap = 1;
    private int m_currentCheckpoint = 1;
    private int m_currentGhostSample = 1;
    private int m_ghostNumSamples;
    private int m_MapType;
    private int m_CarType;
    private int m_ghostMap;
    private int m_ghostCar;

    private bool m_passedCheckpointCtr = true;
    private bool m_RaceStarted;
    private bool m_isGhostCarData;
    private bool m_stopGhost;
    private bool m_ghostStarted;
    private bool m_isWatchingRace;

    private float m_BestTimeMap;
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
        SetMapAndCar();
        StartGhostCar();
        SetWatchingRace();
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
                    if (m_isWatchingRace) SceneManager.LoadScene("MenuScene");
                }
            }
        }
    }

    private void SetWatchingRace()
    {
        string isWatching = PlayerPrefs.GetString("WatchLap", "false");
        if (isWatching == "true") m_isWatchingRace = true;
        else if (isWatching == "false") m_isWatchingRace = false;

        if (m_isWatchingRace)
        {
            if (m_ghostMap == 0)
            {
                m_Maps[0].SetActive(true);
                m_Maps[1].SetActive(false);
            }
            else if (m_ghostMap == 1)
            {
                m_Maps[0].SetActive(false);
                m_Maps[1].SetActive(true);
            }
            m_Car.SetActive(false);
            SetCinematicCam();
            GetComponent<CamerasTV>().m_TargeretCar = m_ghostCarPrefab;
            GetComponent<UIController>().DesactivateCurrentLapInfo();
            m_RaceStarted = true;
        }
        else
        {
            m_Car.SetActive(true);
            GetComponent<CamerasTV>().m_TargeretCar = m_Car;
            SetNormalCam();
        }
    }

    private void SetMapAndCar()
    {
        m_MapType = PlayerPrefs.GetInt("MapSelected", 0);
        m_CarType = PlayerPrefs.GetInt("CarSelected", 0);

        if (m_MapType == 0)
        {
            m_Maps[0].SetActive(true);
            m_Maps[1].SetActive(false);
            m_Car.transform.position = new Vector3(640f, 0.2f, 475f);
            m_Car.transform.rotation = Quaternion.Euler(0, 268f, 0);
        }
        else if (m_MapType == 1)
        {
            m_Maps[0].SetActive(false);
            m_Maps[1].SetActive(true);
            m_Car.transform.position = new Vector3(601f, 32f, 501f);
            m_Car.transform.rotation = Quaternion.Euler(0, 282f, 0);
        }
    }

    private void StartGhostCar()
    {
        m_GhostCarScript = ScriptableObject.CreateInstance("GhostCarScript") as GhostCarScript;
        if(m_MapType == 0) m_BestTimeMap = PlayerPrefs.GetFloat("BestTimeMap0", 0f);
        else if (m_MapType == 1) m_BestTimeMap = PlayerPrefs.GetFloat("BestTimeMap1", 0f);
        GetComponent<UIController>().SetBestTime(m_BestTimeMap);
        m_isGhostCarData = CheckIfGhostCarData();
        if (m_isGhostCarData)
        {
            m_GhostCarScript.ReadGhostCarTXT();
            m_ghostMap = m_GhostCarScript.GetGhostMap();
            m_ghostCar = m_GhostCarScript.GetGhostCar();
            if (m_MapType != m_ghostMap) return;
            GameObject check1 = GameObject.FindGameObjectWithTag("Checkpoint1");
            if (!check1) return;
            m_lastSamplePosition = m_initCarPos = new Vector3(check1.transform.position.x, m_Car.transform.position.y, check1.transform.position.z);
            m_lastSampleRotation = m_initCarRot = m_Car.transform.rotation;
            m_GhostCarScript.GetDataAt(0, out m_nextPosition, out m_nextRotation);
            m_ghostNumSamples = m_GhostCarScript.GetNumGhostSamples();
            m_ghostFrequencySamples = m_GhostCarScript.GetGhostFrequencySamples();
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
        GetComponent<UIController>().ShowFinishedRace();
        Invoke("GoToMenu", 4);
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
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
                CheckGhostTime();
                GetComponent<UIController>().SetLapTime(m_currentLap);
                FinalRace();
                return;
            }
            else
            {
                CheckGhostTime();
                GetComponent<UIController>().SetLapTime(m_currentLap);
                m_currentLap++;
                PassCheckpoint(numCheckpoint);
                if(m_isGhostCarData) RestartGhostCar();
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

    private void RestartGhostCar()
    {
        m_GhostCarScript.Reset();
        m_ghostStarted = false;
        m_isGhostCarData = true;
        m_stopGhost = false;
        m_GhostCarScript.ReadGhostCarTXT();
        m_ghostNumSamples = m_GhostCarScript.GetNumGhostSamples();
        m_ghostFrequencySamples = m_GhostCarScript.GetGhostFrequencySamples();
        m_lastSamplePosition = m_initCarPos;
        m_lastSampleRotation = m_initCarRot;
        m_GhostCarScript.GetDataAt(0, out m_nextPosition, out m_nextRotation);
        m_currentGhostSample = 1;
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
        if (currentTimer < m_BestTimeMap || m_BestTimeMap == 0)
        {
            m_BestTimeMap = currentTimer;
            if (m_MapType == 0) PlayerPrefs.SetFloat("BestTimeMap0", currentTimer);
            else if (m_MapType == 1) PlayerPrefs.SetFloat("BestTimeMap1", currentTimer);
            GetComponent<UIController>().SetBestTime(currentTimer);
            m_GhostCarScript.WriteGhostCarTXT(m_frequencyBetweenSamples);
        }
        m_GhostCarScript.Reset();
    }

    private bool CheckIfGhostCarData()
    {
        return m_GhostCarScript.CheckGhostCarData();
    }

    public void SetNormalCam()
    {
        m_MainCamera.SetActive(true);
        GetComponent<CamerasTV>().DesactivateAllCams();
        GetComponent<CamerasTV>().enabled = false;
    }

    public void SetCinematicCam()
    {
        GetComponent<CamerasTV>().enabled = true;
        GetComponent<CamerasTV>().ActivateAllCams();
        m_MainCamera.SetActive(false);
    }
}
