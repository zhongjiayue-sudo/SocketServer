using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager:MonoBehaviour
{
    public TMP_Text text;
    public Button connectBtn;

    public static UIManager Instance;

    private void Start()
    {  
        Instance = this;
        connectBtn.onClick.AddListener(() =>
        {
            WuZiQiManager.Instance.Init();
            connectBtn.gameObject.SetActive(false);
        });
    }


}
