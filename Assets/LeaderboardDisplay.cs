using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardDisplay : MonoBehaviour
{
    public List<TextMeshProUGUI> names, scores;

    public void Display(List<string> nameList, List<string> scoreList)
    {
        for(int i = 0; i < names.Count; i++)
        {
            if (i < nameList.Count)
            {
                names[i].text = nameList[i];
                scores[i].text = scoreList[i];
            }
            else
            {
                names[i].text = "";
                scores[i].text = "";
            }
        }
    }
}
