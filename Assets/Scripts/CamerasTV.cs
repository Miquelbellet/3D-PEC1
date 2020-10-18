using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasTV : MonoBehaviour
{
    public GameObject m_TargeretCar;
    public GameObject m_ParentCamerasTVMap1, m_ParentCamerasTVMap2;
    public float m_TimeBetweenCamChecks;

    private GameObject[] m_CamerasTV;
    private float m_time;
    private int m_NumCameraActive;
    private int m_ghostMap;
    void Start()
    {
        m_ghostMap = PlayerPrefs.GetInt("MapSelected", 0);
    }

    void Update()
    {
        m_time += Time.deltaTime;
        if (m_time >= m_TimeBetweenCamChecks)
        {
            CheckClosestCamera();
            m_time -= m_TimeBetweenCamChecks;
        }
    }

    private void OnEnable()
    {
        m_ghostMap = PlayerPrefs.GetInt("MapSelected", 0);
        SetCamObjects();
        CheckClosestCamera();
    }

    private void CheckClosestCamera()
    {
        var distance = Mathf.Infinity;
        for(int i = 0; i < m_CamerasTV.Length; i++)
        {
            float newDist = Vector3.Distance(m_TargeretCar.transform.position, m_CamerasTV[i].transform.position);
            if (newDist < distance)
            {
                m_NumCameraActive = i;
                SetActiveCamera();
                distance = newDist;
            }
        }
    }

    private void SetActiveCamera()
    {
        m_ghostMap = PlayerPrefs.GetInt("MapSelected", 0);
        for (int i = 0; i < m_CamerasTV.Length; i++)
        {
            if (i == m_NumCameraActive)
            {
                if (m_ghostMap == 0)
                {
                    m_ParentCamerasTVMap1.transform.GetChild(i).gameObject.GetComponent<Camera>().depth = 5;
                    m_ParentCamerasTVMap1.transform.GetChild(i).gameObject.GetComponent<AudioListener>().enabled = true;
                    m_ParentCamerasTVMap1.transform.GetChild(i).gameObject.SetActive(true);
                }
                else if (m_ghostMap == 1)
                {
                    m_ParentCamerasTVMap2.transform.GetChild(i).gameObject.GetComponent<Camera>().depth = 5;
                    m_ParentCamerasTVMap2.transform.GetChild(i).gameObject.GetComponent<AudioListener>().enabled = true;
                    m_ParentCamerasTVMap2.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                if (m_ghostMap == 0)
                {
                    m_ParentCamerasTVMap1.transform.GetChild(i).gameObject.GetComponent<Camera>().depth = 0;
                    m_ParentCamerasTVMap1.transform.GetChild(i).gameObject.GetComponent<AudioListener>().enabled = false;
                    m_ParentCamerasTVMap1.transform.GetChild(i).gameObject.SetActive(false);
                }
                else if (m_ghostMap == 1)
                {
                    m_ParentCamerasTVMap2.transform.GetChild(i).gameObject.GetComponent<Camera>().depth = 0;
                    m_ParentCamerasTVMap2.transform.GetChild(i).gameObject.GetComponent<AudioListener>().enabled = false;
                    m_ParentCamerasTVMap2.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetCamObjects()
    {
        m_ghostMap = PlayerPrefs.GetInt("MapSelected", 0);
        int numOfCams = 0;
        if (m_ghostMap == 0) numOfCams = m_ParentCamerasTVMap1.transform.childCount;
        else if (m_ghostMap == 1) numOfCams = m_ParentCamerasTVMap2.transform.childCount;
        m_CamerasTV = new GameObject[numOfCams];
        
        for (int i = 0; i < numOfCams; i++)
        {
            if (m_ghostMap == 0) m_CamerasTV[i] = m_ParentCamerasTVMap1.transform.GetChild(i).gameObject;
            else if (m_ghostMap == 1) m_CamerasTV[i] = m_ParentCamerasTVMap2.transform.GetChild(i).gameObject;
        }
    }

    public void DesactivateAllCams()
    {
        m_ghostMap = PlayerPrefs.GetInt("MapSelected", 0);
        if (m_ghostMap == 0)
        {
            m_ParentCamerasTVMap1.SetActive(false);
        }
        else if (m_ghostMap == 1)
        {
            m_ParentCamerasTVMap2.SetActive(false);
        }
    }

    public void ActivateAllCams()
    {
        m_ghostMap = PlayerPrefs.GetInt("MapSelected", 0);
        if (m_ghostMap == 0)
        {
            m_ParentCamerasTVMap1.SetActive(true);
            m_ParentCamerasTVMap2.SetActive(false);
        }
        else if (m_ghostMap == 1)
        {
            m_ParentCamerasTVMap1.SetActive(false);
            m_ParentCamerasTVMap2.SetActive(true);
        }
    }
}
