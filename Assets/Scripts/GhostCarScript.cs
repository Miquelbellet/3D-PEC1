using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using System.Globalization;

[CreateAssetMenu]
public class GhostCarScript : ScriptableObject
{
    List<Vector3> carPositions = new List<Vector3>();
    List<Quaternion> carRotations = new List<Quaternion>();
    List<Vector3> ghostCarPositions = new List<Vector3>();
    List<Quaternion> ghostCarRotations = new List<Quaternion>();

    private float m_frequencyGhostSamples;
    private int m_mapSelected;
    private int m_carSelected;

    public void AddNewData(Transform transform)
    {
        carPositions.Add(transform.position);
        carRotations.Add(transform.rotation);
    }

    public void GetDataAt(int sample, out Vector3 position, out Quaternion rotation)
    {
        position = ghostCarPositions[sample];
        rotation = ghostCarRotations[sample];
    }

    public void Reset()
    {
        carPositions.Clear();
        carRotations.Clear();
    }

    public void WriteGhostCarTXT(float frequencySamples)
    {
        string path = "Assets/Resources/GhostBestLap"+PlayerPrefs.GetInt("MapSelected", 0)+".txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(PlayerPrefs.GetInt("MapSelected", 0).ToString());
        writer.WriteLine(PlayerPrefs.GetInt("CarSelected", 0).ToString());

        writer.WriteLine(carPositions.Count.ToString());
        writer.WriteLine(frequencySamples.ToString());
        for (int i = 0; i < carPositions.Count; i++) {
            writer.WriteLine(carPositions[i]+"/"+ carRotations[i]);
        }
        writer.Close();
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();
    }

    public void ReadGhostCarTXT()
    {
        try
        {
            string path = "Assets/Resources/GhostBestLap" + PlayerPrefs.GetInt("MapSelected", 0) + ".txt";
            StreamReader reader = new StreamReader(path);
            m_mapSelected = int.Parse(reader.ReadLine());
            m_carSelected = int.Parse(reader.ReadLine());

            var numOfSamples = reader.ReadLine();

            string freq = reader.ReadLine();
            var freqStr = freq.Replace(".", ",");
            float.TryParse(freqStr, out m_frequencyGhostSamples);

            ghostCarPositions.Clear();
            ghostCarRotations.Clear();
            for (int i = 0; i < int.Parse(numOfSamples); i++)
            {
                var line = reader.ReadLine();
                var ghostPos = line.Split('/')[0];
                var ghostRot = line.Split('/')[1];
                ghostPos = ghostPos.Replace("(", "");
                ghostPos = ghostPos.Replace(")", "");
                ghostPos = ghostPos.Replace(" ", "");
                ghostRot = ghostRot.Replace("(", "");
                ghostRot = ghostRot.Replace(")", "");
                ghostRot = ghostRot.Replace(" ", "");

                var xP = ghostPos.Split(',')[0].Replace(".", ",");
                float xPos;
                float.TryParse(xP, out xPos);
                var yP = ghostPos.Split(',')[1].Replace(".", ",");
                float yPos;
                float.TryParse(yP, out yPos);
                var zP = ghostPos.Split(',')[2].Replace(".", ",");
                float zPos;
                float.TryParse(zP, out zPos);
                ghostCarPositions.Add(new Vector3(xPos, yPos, zPos));

                var wR = ghostRot.Split(',')[0].Replace(".", ",");
                float wRot;
                float.TryParse(wR, out wRot);
                var xR = ghostRot.Split(',')[1].Replace(".", ",");
                float xRot;
                float.TryParse(xR, out xRot);
                var yR = ghostRot.Split(',')[2].Replace(".", ",");
                float yRot;
                float.TryParse(yR, out yRot);
                var zR = ghostRot.Split(',')[3].Replace(".", ",");
                float zRot;
                float.TryParse(zR, out zRot);
                ghostCarRotations.Add(new Quaternion(wRot, xRot, yRot, zRot));
            }
            reader.Close();
        }
        catch
        {
            Debug.Log("No ghost car info detected.");
        }
    }

    public bool CheckGhostCarData()
    {
        string path = "Assets/Resources/GhostBestLap" + PlayerPrefs.GetInt("MapSelected", 0) + ".txt";
        return File.Exists(path);
    }

    public int GetNumGhostSamples()
    {
        return ghostCarPositions.Count;
    }

    public float GetGhostFrequencySamples()
    {
        return m_frequencyGhostSamples;
    }

    public int GetGhostMap()
    {
        return m_mapSelected;
    }

    public int GetGhostCar()
    {
        return m_carSelected;
    }
}
