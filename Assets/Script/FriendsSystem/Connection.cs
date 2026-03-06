using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
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
    

    public void CreateAccount()
    {
        if (username.text == "" || password.text == "" || passwordVerif.text != password.text)
        {
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
        if (result != "User created")
        {
            Debug.Log(result);
            return;
        }
        
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
        byte [] data = System.Text.Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(URL + "/set/users","POST");
        request.uploadHandler = new UploadHandlerRaw(data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        AccountCreationResult(request.downloadHandler.text);
        Debug.Log(request.downloadHandler.text);
    }
}
