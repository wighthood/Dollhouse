using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class User
{
    public string username;
    public string password;

    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

public class Connection : MonoBehaviour
{
    [Header("account info")]
    [SerializeField]
    private TMP_InputField username;
    [SerializeField]
    private TMP_InputField password;
    [SerializeField]
    private TMP_InputField passwordVerif;
    
    [Header("account deletion")]
    [SerializeField] TMP_InputField usernameDelete;
    [SerializeField] TMP_InputField passwordDelete;
    
    
    [Header("display")] 
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject accountPanel;
    [SerializeField] private GameObject buttonConfirm;
    [SerializeField] private GameObject buttonConnect;
    [SerializeField] private GameObject buttoncreate;
    [SerializeField] private GameObject textPasswordVerif;
    
    [Header("FriendUI")] [SerializeField] private Button friendsList;
    
    [Header("return message")]
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private TMP_Text textReturn;

    [Header("buttons")]
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject accountOption;
    
    [Header("token")]
    [SerializeField] private SessionData SessionData;

    public void CreateAccount()
    {
        if (username.text == "" || password.text == "" || passwordVerif.text != password.text)
        {
            answerPanel.SetActive(true);
            if (username.text == "" || password.text == "" || passwordVerif.text =="")
            {
                textReturn.text = "please fill all the fields";
            }
            if (passwordVerif.text != password.text)
            {
                textReturn.text = "password do not match. try again";
            }
            cleartext();
            return;
        }
        StartCoroutine(CreateAccountCoroutine());
    }

    private void cleartext()
    {
        username.text = "";
        password.text = "";
        passwordVerif.text = "";
        usernameDelete.text = "";
        passwordDelete.text = "";
    }

    private void AccountCreationResult(string result)
    {
        cleartext();
        answerPanel.SetActive(true);
        if (result == "false")
        {
            textReturn.text = "Account already exists";
            return;
        }
        textReturn.text = "Account created";
        passwordVerif.gameObject.SetActive(false);
        textPasswordVerif.SetActive(false);
        buttonConfirm.SetActive(false);
        buttonConnect.SetActive(true);
        buttoncreate.SetActive(true);
    }
    
    IEnumerator CreateAccountCoroutine()
    {
        User newUser = new User(username.text, password.text);
        string json = JsonUtility.ToJson(newUser);
        byte [] data = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(APIConfig.API_URL + "/connexion/createAccount","POST");
        request.uploadHandler = new UploadHandlerRaw(data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        AccountCreationResult(request.downloadHandler.text);
    }

    public void Connect()
    {
        if (username.text == "" || password.text == "")
        {
            answerPanel.SetActive(true);
            textReturn.text = "please fill all the fields";
            cleartext();
            return;
        }
        StartCoroutine(ConnectCoroutine());
    }

    IEnumerator ConnectCoroutine()
    {
        User connectingUser = new User(username.text, password.text);
        string json = JsonUtility.ToJson(connectingUser);
        byte [] data = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(APIConfig.API_URL + "/connexion","POST");
        request.uploadHandler = new UploadHandlerRaw(data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        ConnectionResult(request.downloadHandler.text);
    }
    
    private void ConnectionResult(string result)
    {
        if (result == "")
        {
            answerPanel.SetActive(true);
            textReturn.text = "invalid username or password";
            cleartext();
        }
        else
        { 
            friendsList.interactable = true;
            accountOption.GetComponentInChildren<TMP_Text>().text = username.text;
            cleartext();
            SessionData.token = result;
            loginButton.SetActive(false);
            accountOption.gameObject.SetActive(true);
            connectionPanel.SetActive(false);
        }
    }

    public void DeleteAccount()
    {
        if (usernameDelete.text == "" || passwordDelete.text == "")
        {
            answerPanel.SetActive(true);
            textReturn.text = "please fill all the fields";
            cleartext();
            return;
        }
        StartCoroutine(DeleteAccountCoroutine());
    }

    IEnumerator DeleteAccountCoroutine()
    {
        User deletingUser = new User(username.text, password.text);
        string json = JsonUtility.ToJson(deletingUser);
        byte [] data = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(APIConfig.API_URL + "/user/delete","POST");
        request.uploadHandler = new UploadHandlerRaw(data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        DeleteAccountResult(request.downloadHandler.text);
    }

    private void DeleteAccountResult(string result)
    {
        if (result == "")
        {
            answerPanel.SetActive(true);
            textReturn.text = "invalid username or password";
            cleartext();
        }
        else
        {
            Disconnect();
        }
    }
    
    
    public void Disconnect()
    {
        accountOption.gameObject.SetActive(false);
        loginButton.SetActive(true);
        accountPanel.SetActive(false);
        SessionData.token = "";
        friendsList.interactable = false;
    }
}
