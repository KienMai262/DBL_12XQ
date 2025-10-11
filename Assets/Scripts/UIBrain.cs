using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIBrain : MonoBehaviour
{
    public static UIBrain instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private UICanonical uiCanonical;
    [SerializeField] private UITransition uiTransition;
    public UICanonical UICanonical => uiCanonical;
    public UITransition UITransition => uiTransition;
    
    [SerializeField] private Button btnCanonical;
    [SerializeField] private Button btnNextChapter;
    public Button BtnCanonical => btnCanonical;
    public Button BtnNextChapter => btnNextChapter;

    private void Start()
    {
        // uiCanonical.Init();
    }
}
