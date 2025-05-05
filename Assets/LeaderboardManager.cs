using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public struct BoardEntry
{
    public BoardEntry(string n, float t, Difficulty diff) { name = n; time = t; difficulty = diff; }
    public string name;
    public float time;
    public Difficulty difficulty;
}
public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] GameObject leaderboardTable;
    [SerializeField] GameObject row;
    List<BoardEntry> entries = new List<BoardEntry>();

    private void Start()
    {
        LoadLeaderboard();
    }

    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(entries);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/leaderboard.json", json);
    }

    void LoadLeaderboard()
    {
        string path = Application.persistentDataPath + "/leaderboard.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            entries = JsonUtility.FromJson<List<BoardEntry>>(json);
        }
        else
            entries = new List<BoardEntry>();
    }

    public void AddLeaderboardEntry(BoardEntry entry)
    {
        entries.Add(entry);
    }
    public void ShowEntries()
    {
        DestroyRows();
        entries.Sort((a, b) => a.time.CompareTo(b.time));
        for (int i = 1; i < entries.Count && i <= 10; i++)
        {
            GameObject newEntry = Instantiate(row, leaderboardTable.transform);
            TextMeshProUGUI[] entryDetails = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            entryDetails[0].text = "" + i;
            entryDetails[1].text = entries[i].name;

            int minutes = Mathf.FloorToInt(entries[i].time / 60.0f);
            int seconds = Mathf.FloorToInt(entries[i].time - minutes * 60.0f);
            entryDetails[2].text = string.Format("{0:00}:{1:00}", minutes, seconds);

            entryDetails[3].text = entries[i].difficulty.ToString();
        }
    }

    void DestroyRows()
    {
        while (leaderboardTable.transform.childCount > 0)
            DestroyImmediate(leaderboardTable.transform.GetChild(0).gameObject);
    }
}
