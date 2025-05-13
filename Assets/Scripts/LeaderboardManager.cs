using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    int maxEntriesSize = 10;

    private void Start()
    {
        LoadLeaderboard();
    }

    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(entries.GetRange(0,maxEntriesSize-1));
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
        nameTagPage.EraseInputField();
    }

    public void ShowEntries()
    {
        DestroyRows();
        entries.Sort((a, b) =>
        {
            // First sort by difficulty (Hard > Medium > Easy)
            int diffA = (int)a.difficulty;
            int diffB = (int)b.difficulty;

            if (diffA != diffB)
                return diffB.CompareTo(diffA); // Reverse to get Hard first

            // If same difficulty, sort by time (smaller is better)
            return a.time.CompareTo(b.time);
        });
        //entries.Sort((a, b) => a.time.CompareTo(b.time));
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

    void DestroyRows()
    {
        while (leaderboardTable.transform.childCount > 0)
            DestroyImmediate(leaderboardTable.transform.GetChild(0).gameObject);
    }
}
