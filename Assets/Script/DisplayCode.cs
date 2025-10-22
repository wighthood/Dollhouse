using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DisplayCode : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI codeDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsHost)
        {
            codeDisplay.text = StaticCode.GameCode;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
