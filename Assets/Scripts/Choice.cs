using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Choice : MonoBehaviour
{
    public string choice;
    public int index;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject choiceSelectBox;
    public Button button;
    public Parameter parameter;

    public void SetChoice(string choice, int index, Parameter parameter)
    {
        this.choice = choice;
        this.index = index;
        this.parameter = parameter;
        text.text = choice;
    }
}
