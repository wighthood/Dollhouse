using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

[Serializable]
public class user
{
    public string username;
    public string password;

    public user(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

public class Connection : MonoBehaviour
{
    
    [SerializeField]
    private string URL = "http://192.168.2.33:4242";
    [Header("account info")]
    [SerializeField]
    private TMP_InputField username;
    [SerializeField]
    private TMP_InputField password;
    [SerializeField]
    private TMP_InputField passwordVerif;

    [Header("display")] 
    [SerializeField] private GameObject buttonConfirm;
    [SerializeField] private GameObject buttonConnect;
    [SerializeField] private GameObject buttoncreate;
    [SerializeField] private GameObject textPasswordVerif;
    
    [Header("return message")]
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private TMP_Text textReturn;
    

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

    public void cleartext()
    {
        username.text = "";
        password.text = "";
        passwordVerif.text = "";
    }

    private void AccountCreationResult(string result)
    {
        cleartext();
        answerPanel.SetActive(true);
        if (result != "User created")
        {
            
            Debug.Log(result);
            if (result == "{\"error\":\"ERROR: duplicate key value violates unique constraint \"users_username_key\"\n  Detail: Key (username)=(Wighthood) already exists.\"}")
                result = "username is taken";
            textReturn.text = result;
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
        user newUser = new user(username.text, password.text);
        string json = JsonUtility.ToJson(newUser);
        byte [] data = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(URL + "/set/users","POST");
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
        UnityWebRequest request = UnityWebRequest.Get(URL + "/get/users/" + username.text);
        
        
        yield return request.SendWebRequest();
        ConnectionResult(request.downloadHandler.text);
    }
    
    private void ConnectionResult(string result)
    {
        if (result != "success")
        {
            answerPanel.SetActive(true);
            textReturn.text = "invalid username or password";
            cleartext();
            return;
        }
        Debug.Log(result);
    }
}
