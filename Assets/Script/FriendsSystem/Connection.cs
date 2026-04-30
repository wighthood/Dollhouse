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

    [Header("display")] 
    [SerializeField] private GameObject ConnectionPanel;
    [SerializeField] private GameObject buttonConfirm;
    [SerializeField] private GameObject buttonConnect;
    [SerializeField] private GameObject buttoncreate;
    [SerializeField] private GameObject textPasswordVerif;
    
    [Header("return message")]
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private TMP_Text textReturn;

    [Header("Login")]
    [SerializeField] private Button loginButton;
    
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

    [Header("FriendUI")] [SerializeField] private Button friendsList;
    
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
            loginButton.gameObject.GetComponentInChildren<TMP_Text>().text = username.text;
            cleartext();
            SessionData.token = result;
            ConnectionPanel.SetActive(false);
        }
    }
    
    public void Disconnect()
    {
        //add more data/functionalities that need to be deleted/removed here
        loginButton.gameObject.GetComponent<TMP_Text>().text = "login";
        friendsList.interactable = false;
    }
}
