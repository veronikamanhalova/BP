using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public struct BoardEntry
{
    public BoardEntry(string n, float t){ name = n; time = t; }
    public string name;
    public float time;
}
public class Leaderboard : MonoBehaviour
{
    [SerializeField] GameObject row;
    public List<BoardEntry> entries = new List<BoardEntry>();

    public void ShowEntries()
    {
        entries.Sort();
        for (int i = 1; i <= 10; i++)
        {
            GameObject newEntry = Instantiate(row, transform);
            Text[] entryDetails = newEntry.GetComponentsInChildren<Text>();
            entryDetails[0].text = "" + i;
            entryDetails[1].text = entries[i].name;

            int minutes = Mathf.FloorToInt(entries[i].time / 60.0f);
            int seconds = Mathf.FloorToInt(entries[i].time - minutes * 60.0f);
            entryDetails[2].text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    public void AddLeaderboardEntry(BoardEntry entry)
    {
        entries.Add(entry);
    }
}
