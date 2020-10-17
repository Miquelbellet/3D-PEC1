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

    private string path = "Assets/Resources/GhostBestLap.txt";
    private float m_frequencyGhostSamples;

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
        StreamWriter writer = new StreamWriter(path, false);
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
            StreamReader reader = new StreamReader(path);
            var numOfSamples = reader.ReadLine();

            string freq = reader.ReadLine();
            var freqStr = freq.Replace(".", ",");
            float.TryParse(freqStr, out m_frequencyGhostSamples);

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
        bool canReadData = true;
        try
        {
            StreamReader reader = new StreamReader(path);
            reader.ReadLine();
            canReadData = true;
        }
        catch
        {
            canReadData = false;
        }
        return canReadData;
    }

    public int GetNumGhostSamples()
    {
        return ghostCarPositions.Count;
    }

    public float GetGhostFrequencySamples()
    {
        return m_frequencyGhostSamples;
    }
}
