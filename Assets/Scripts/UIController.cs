using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI m_CurrentTime;
    public TextMeshProUGUI m_NotCorrectPath;
    public TextMeshProUGUI m_CurrentLap;
    public TextMeshProUGUI m_bestTimeEver;
    public TextMeshProUGUI m_Lap1Time;
    public TextMeshProUGUI m_Lap2Time;
    public TextMeshProUGUI m_Lap3Time;
    public TextMeshProUGUI m_finishedRace;

    private int m_MaxLaps;
    private float m_time;
    private bool m_NotBlinkingTime;
    private bool m_RaceStarted;
    private float[] m_LapTimes;

    void Start()
    {
        m_NotBlinkingTime = true;
        m_NotCorrectPath.gameObject.SetActive(false);
        m_finishedRace.gameObject.SetActive(false);
        m_Lap1Time.gameObject.SetActive(false);
        m_Lap2Time.gameObject.SetActive(false);
        m_Lap3Time.gameObject.SetActive(false);
        m_MaxLaps = GetComponent<GameController>().m_NumberOfLaps;
        m_LapTimes = new float[m_MaxLaps];
    }

    void Update()
    {
        if (m_RaceStarted)
        {
            m_time += Time.deltaTime;
            if (m_NotBlinkingTime) m_CurrentTime.text = m_time.ToString("f3") + " s";
        }
    }

    public void Checkpoint(int i)
    {
        if (i == 1)
        {
            m_RaceStarted = true;
            return;
        }
        m_NotBlinkingTime = false;
        StartCoroutine(BlinkingTime(5, 0.2f));
        Invoke("EndCheckpoint", 2);
    }

    IEnumerator BlinkingTime(int numBlinks, float seconds)
    {
        for (int i = 0; i < numBlinks * 2; i++)
        {
            m_CurrentTime.enabled = !m_CurrentTime.enabled;
            yield return new WaitForSeconds(seconds);
        }
    }

    private void EndCheckpoint()
    {
        m_NotBlinkingTime = true;
    }

    public void CorrectPath(bool isCorrect)
    {
        if(isCorrect) m_NotCorrectPath.gameObject.SetActive(false);
        else m_NotCorrectPath.gameObject.SetActive(true);
    }

    public void SetLapTime(int lapNum)
    {
        m_CurrentLap.text = (lapNum + 1) + " / " + m_MaxLaps;
        if (lapNum == 1)
        {
            m_Lap1Time.text = "Lap 1: " + m_time.ToString("f3") + " s";
            m_Lap1Time.gameObject.SetActive(true);
        }
        else if (lapNum == 2)
        {
            m_Lap2Time.text = "Lap 2: " + m_time.ToString("f3") + " s";
            m_Lap2Time.gameObject.SetActive(true);
        }
        else if (lapNum == 3)
        {
            m_Lap3Time.text = "Lap 3: " + m_time.ToString("f3") + " s";
            m_Lap3Time.gameObject.SetActive(true);
        }
        if (lapNum == m_MaxLaps) m_RaceStarted = false;
        else ResetLapTime(lapNum);
    }

    private void ResetLapTime(int lapNumber)
    {
        m_LapTimes[lapNumber - 1] = m_time;
        m_time = 0;
    }

    public float GetCurrentTimer()
    {
        return m_time;
    }

    public void SetBestTime(float bestTime)
    {
        if(bestTime != 0)
        {
            m_bestTimeEver.text = "Best Time: "+bestTime.ToString("f3") + " s";
            m_bestTimeEver.gameObject.SetActive(true);
        }
        else m_bestTimeEver.gameObject.SetActive(false);
    }

    public void DesactivateCurrentLapInfo()
    {
        m_CurrentLap.gameObject.SetActive(false);
        m_CurrentTime.gameObject.SetActive(false);
    }

    public void ShowFinishedRace()
    {
        m_finishedRace.gameObject.SetActive(true);
    }

    public void WatchWithNormalCam()
    {
        GetComponent<GameController>().SetNormalCam();
    }

    public void WatchWithCinematicCam()
    {
        GetComponent<GameController>().SetCinematicCam();
    }

    public void RestartRace()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
