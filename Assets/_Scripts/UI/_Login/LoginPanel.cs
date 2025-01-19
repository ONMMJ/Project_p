using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_Text idText;
    [SerializeField] TMP_Text passwordText;
    [SerializeField] Button signUpButton;
    [SerializeField] Button loginButton;

    // Start is called before the first frame update
    void Start()
    {
        signUpButton.onClick.AddListener(() => {
            BackendLogin.Instance.CustomSignUp(idText.text, passwordText.text);
            Debug.Log(idText.text + " / " + passwordText.text);
        });
        loginButton.onClick.AddListener(() => {
            BackendLogin.Instance.CustomLogin(idText.text, passwordText.text);
            Debug.Log(idText.text + " / " + passwordText.text);
        });
    }
}
