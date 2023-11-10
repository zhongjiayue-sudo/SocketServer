using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField passwordAgainInputField;

    public Button loginBtn;
    public Button BackBtn;
    public Button registerBtn;

    public TMP_Text content;

    public static LoginManager Instance;

    private bool isRegister = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetWorkManager.Connect();
        loginBtn.onClick.AddListener(() =>
        {
            NetWorkManager.Send("Login|" + userInputField.text + "|" + passwordInputField.text);//协议规定发送命令
            ResetInputField();
        });
        BackBtn.onClick.AddListener(() =>
        {
            ResetInputField();
            loginBtn.gameObject.SetActive(true);
            isRegister = false;
            BackBtn.gameObject.SetActive(false);
        });
        registerBtn.onClick.AddListener(() =>
        {
            if (!isRegister)
            {
                passwordAgainInputField.gameObject.SetActive(true);
                isRegister = true;
                loginBtn.gameObject.SetActive(false);
                BackBtn.gameObject.SetActive(true);
            }
            else
            {
                if (passwordInputField.text == passwordAgainInputField.text)
                    NetWorkManager.Send("Register|" + userInputField.text + "|" + passwordInputField.text);
            }
            ResetInputField();
        });
    }

    private void ResetInputField()
    {
        userInputField.text = "";
        passwordInputField.text = "";
        passwordAgainInputField.text = "";
    }
}
