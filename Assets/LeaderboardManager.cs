using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;

public struct BoardEntry
{
    public BoardEntry(string n, string t, Difficulty diff) { name = n; time = t; difficulty = diff; }
    public string name;
    public string time;
    public Difficulty difficulty;
}
public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] NameTagPage nameTagPage;
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

    public void SetLastEntryNameFromInputField()
    {
        BoardEntry lastEntry = entries[entries.Count - 1];
        lastEntry.name = nameTagPage.GetInputFieldName();
        entries[entries.Count - 1] = lastEntry;
    }

    public void ShowEntries()
    {
        DestroyRows();
        Debug.Log(entries.Count);
        entries.Sort((a, b) => a.time.CompareTo(b.time));
        for (int i = 0; i < entries.Count && i < 10; i++)
        {
            GameObject newEntry = Instantiate(row, leaderboardTable.transform);
            TextMeshProUGUI[] entryDetails = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            entryDetails[0].text = "" + (i+1);
            entryDetails[1].text = entries[i].name;
            entryDetails[2].text = entries[i].time;
            entryDetails[3].text = entries[i].difficulty.ToString();
        }
    }

    void DestroyRows()
    {
        while (leaderboardTable.transform.childCount > 0)
            DestroyImmediate(leaderboardTable.transform.GetChild(0).gameObject);
    }
}
