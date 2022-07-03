using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
public class FillScenariosList : MonoBehaviour
{
    void Start()
    {
        var dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        if (Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            var saves = Directory.EnumerateFiles(Application.persistentDataPath + "/Saves");
            var options = saves.Select(s => Path.GetFileName(s)).ToList();
            dropdown.AddOptions(options);
            dropdown.RefreshShownValue();
        }
    }
}
