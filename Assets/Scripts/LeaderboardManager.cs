using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct BoardEntry
{
    public BoardEntry(string n, string t, Difficulty diff) { name = n; time = t; difficulty = diff; }
    public string name;
    public string time;
    public Difficulty difficulty;
}
[System.Serializable]
class LeaderboardData
{
    public List<BoardEntry> entries = new List<BoardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] NameTagPage nameTagPage;
    [SerializeField] GameObject leaderboardTable;
    [SerializeField] GameObject row;
    List<BoardEntry> entries = new List<BoardEntry>();
    int maxEntriesSize = 10;

    private void Start()
    {
        LoadLeaderboard();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveLeaderboard();
    }

    private void OnApplicationQuit()
    {
        SaveLeaderboard();
    }

    public void SaveLeaderboard()
    {
        // Sort a copy so the newest entry stays last until it gets its name
        List<BoardEntry> sorted = new List<BoardEntry>(entries);
        SortEntries(sorted);
        LeaderboardData data = new LeaderboardData();
        data.entries = sorted.GetRange(0, Mathf.Min(maxEntriesSize, sorted.Count));
        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/leaderboard.json", json);
    }

    void LoadLeaderboard()
    {
        string path = Application.persistentDataPath + "/leaderboard.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            entries = data != null && data.entries != null ? data.entries : new List<BoardEntry>();
        }
        else
            entries = new List<BoardEntry>();
    }

    public void AddLeaderboardEntry(BoardEntry entry)
    {
        entries.Add(entry);
        SaveLeaderboard();
    }

    public void SetLastEntryNameFromInputField()
    {
        BoardEntry lastEntry = entries[entries.Count - 1];
        lastEntry.name = nameTagPage.GetInputFieldName();
        entries[entries.Count - 1] = lastEntry;
        nameTagPage.EraseInputField();
        SaveLeaderboard();
    }

    public void ShowEntries()
    {
        DestroyRows();
        SortEntries(entries);
        for (int i = 0; i < entries.Count && i < maxEntriesSize; i++)
        {
            GameObject newEntry = Instantiate(row, leaderboardTable.transform);
            TextMeshProUGUI[] entryDetails = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            entryDetails[0].text = "" + (i+1);
            entryDetails[1].text = entries[i].name;
            entryDetails[2].text = entries[i].time;
            entryDetails[3].text = entries[i].difficulty.ToString();
        }
    }

    void SortEntries(List<BoardEntry> list)
    {
        list.Sort((a, b) =>
        {
            // First sort by difficulty (Hard > Medium > Easy)
            int diffA = (int)a.difficulty;
            int diffB = (int)b.difficulty;

            if (diffA != diffB)
                return diffB.CompareTo(diffA); // Reverse to get Hard first

            // If same difficulty, sort by time (smaller is better)
            return a.time.CompareTo(b.time);
        });
    }

    void DestroyRows()
    {
        while (leaderboardTable.transform.childCount > 0)
            DestroyImmediate(leaderboardTable.transform.GetChild(0).gameObject);
    }
}
