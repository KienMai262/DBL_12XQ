using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameStory;
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private Button button;
    public Button Button => button;

    public void Init(string name, bool isLocked)
    {
        nameStory.text = name;
        lockPanel.SetActive(isLocked);
        button.enabled = !isLocked;
    }
}
