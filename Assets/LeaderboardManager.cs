using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField]GameObject panel;
    [SerializeField]Transform cameraTransform;
    public Leaderboard leaderboard;
    float distance = 2.0f;
    bool visible = false;

    private void Update()
    {
        if (visible)
        {
            leaderboard.transform.LookAt(new Vector3(cameraTransform.position.x, leaderboard.transform.position.y, cameraTransform.position.z));
            leaderboard.transform.forward *= -1;
        }
    }
    public void ShowLeaderboard()
    {
        DestroyRows();
        leaderboard.ShowEntries();
        leaderboard.transform.position = cameraTransform.position + new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized * distance;
        panel.SetActive(true);
        visible = true;
    }

    public void HideLeaderBoard()
    {
        panel.SetActive(false);
        visible = false;
    }

    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboard);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/leaderboard.json", json);
    }

    public Leaderboard LoadLeaderboard()
    {
        string path = Application.persistentDataPath + "/leaderboard.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson<Leaderboard>(json);
        }
        else
        {
            return new Leaderboard();
        }
    }
    void DestroyRows()
    {
        while (panel.transform.childCount > 0)
        {
            DestroyImmediate(panel.transform.GetChild(0).gameObject);
        }
    }
}
