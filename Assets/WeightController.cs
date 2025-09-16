using UnityEngine;

public class WeightController : MonoBehaviour
{
    public static WeightController Instance { get; private set; }
    private string weightKey = "PlayerWeight";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetWeight(string weight)
    {
        PlayerPrefs.SetString(weightKey, weight);
        PlayerPrefs.Save();
        Debug.Log("Weight saved: " + weight);
    }

    public string GetWeight()
    {
        return PlayerPrefs.GetString(weightKey, "");
    }
}
