using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI m_CurrentTime;

    private float m_time;
    private bool m_NotBlinkingTime;
    private bool m_RaceStarted;

    void Start()
    {
        m_NotBlinkingTime = true;
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
        if (i == 1) m_RaceStarted = true;
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
}
