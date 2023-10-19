using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public TMP_Text content;
    public Button connectBtn;
    public Button sendBtn;
    public TMP_InputField inputField;

    public static ChatManager Instance;

    void Start()
    {
        Instance = this;
        connectBtn.onClick.AddListener(() =>
        {
            NetWorkManager.Connect();
        });
        sendBtn.onClick.AddListener(() =>
        {
            NetWorkManager.Send(inputField.text);
            inputField.text = "";
        });
    }
}
