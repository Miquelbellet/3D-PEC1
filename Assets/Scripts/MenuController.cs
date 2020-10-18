using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Image m_mapImageContainer;
    public Sprite[] m_availableMaps;
    public GameObject m_WatchLapBtn;

    private int m_mapSelected = 0;
    private int m_carSelected = 0;

    void Start()
    {
        if (File.Exists("Assets/Resources/GhostBestLap0.txt")) m_WatchLapBtn.SetActive(true);
        else m_WatchLapBtn.SetActive(false);
    }

    void Update()
    {
        
    }

    private void SetMapImage()
    {
        m_mapImageContainer.sprite = m_availableMaps[m_mapSelected];
    }

    public void PlusMapImage()
    {
        if(m_mapSelected + 1 == m_availableMaps.Length) m_mapSelected = 0;
        else m_mapSelected++;

        if (File.Exists("Assets/Resources/GhostBestLap"+ m_mapSelected + ".txt")) m_WatchLapBtn.SetActive(true);
        else m_WatchLapBtn.SetActive(false);

        SetMapImage();
    }

    public void SubsMapImage()
    {
        if (m_mapSelected == 0) m_mapSelected = m_availableMaps.Length - 1;
        else m_mapSelected--;

        if (File.Exists("Assets/Resources/GhostBestLap" + m_mapSelected + ".txt")) m_WatchLapBtn.SetActive(true);
        else m_WatchLapBtn.SetActive(false);

        SetMapImage();
    }

    public void PlayRace()
    {
        PlayerPrefs.SetInt("MapSelected", m_mapSelected);
        PlayerPrefs.SetInt("CarSelected", m_carSelected);
        PlayerPrefs.SetString("WatchLap", "false");
        SceneManager.LoadScene("GameScene");
    }

    public void WatchFastestLap()
    {
        PlayerPrefs.SetInt("MapSelected", m_mapSelected);
        PlayerPrefs.SetInt("CarSelected", m_carSelected);
        PlayerPrefs.SetString("WatchLap", "true");
        SceneManager.LoadScene("GameScene");
    }
}
