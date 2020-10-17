using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private int m_mapSelected = 0;
    private int m_carSelected = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayRace()
    {
        PlayerPrefs.SetInt("MapSelected", m_mapSelected);
        PlayerPrefs.SetInt("CarSelected", m_carSelected);
        SceneManager.LoadScene("GameScene");
    }

    public void WatchFastestLap()
    {
        PlayerPrefs.SetString("WatchLap", "true");
        SceneManager.LoadScene("GameScene");
    }
}
