using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Polaroid : NetworkBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject photoPrefab;
    [SerializeField] private GameObject PhotoHolder;
    [SerializeField] private GameObject Selector;
    [SerializeField] private GameObject PhotoZoom;
    [SerializeField] private GameObject PhotoShower;
    [SerializeField] private int maxPhotos=10;
    private List<GameObject> photoInstances = new();
    private List<Texture2D> photos = new();
    private int currentPhoto=0;
    
    public void SelectPhoto(InputAction.CallbackContext context)
    {
        if (!IsOwner || Menu.activeSelf || photos.Count == 0)
        {
            return;
        }
        if (context.started)
        {
            if (context.ReadValue<float>() > 0)
            {
                currentPhoto++;
            }
            else if (context.ReadValue<float>() < 0)
            {
                currentPhoto--;
            }
            currentPhoto = Mathf.Clamp(currentPhoto, 0, photos.Count - 1);
            SetSelectorPosition(currentPhoto);
        }
    }

    public void LookAtPhoto(InputAction.CallbackContext context)
    {
        if (!IsOwner || Menu.activeSelf || photos.Count == 0)
        {
            return;
        }
        if (context.started)
        {
            PhotoZoom.SetActive(true);
            PhotoZoom.GetComponent<Image>().sprite = photoInstances[currentPhoto].GetComponent<Image>().sprite;
        }
        if (context.canceled)
        {
            PhotoZoom.SetActive(false);
        }
    }

    public void ShowPhoto(InputAction.CallbackContext context)
    {
        if (!IsOwner || Menu.activeSelf || photos.Count == 0)
        {
            return;
        }
        if (context.started)
        {
            ShowPhotoServerRPC(photos[currentPhoto].EncodeToJPG());
        }

        if (context.canceled)
        {
            HidePhotoServerRPC();
        }
    }
    
    IEnumerator Screenshot(int width, int height)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Reinitialize(1920, 1080);
        tex.Apply();
        photos.Add(tex);
        photoInstances.Add(Instantiate(photoPrefab, PhotoHolder.transform));
        photoInstances[^1].GetComponent<Image>().sprite =
            Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        byte[] bytes = tex.EncodeToJPG();
        LayoutRebuilder.ForceRebuildLayoutImmediate(PhotoHolder.GetComponent<RectTransform>());
        SetSelectorPosition(currentPhoto);
    }

    private void SetSelectorPosition(int index)
    {
        if (!Selector.activeSelf)
        {
            Selector.SetActive(true);
        }
        Selector.GetComponent<RectTransform>().position = photoInstances[index].
            GetComponent<RectTransform>().position;
    }

    [Rpc(SendTo.Server)]
    private void HidePhotoServerRPC()
    {
        HidePhotoClientRPC();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void HidePhotoClientRPC()
    {
        PhotoShower.SetActive(false);
    }
    
    [Rpc(SendTo.Server)]
    private void ShowPhotoServerRPC(byte[] PNG)
    {
        ShowPhotoClientRPC(PNG);
    }

    [Rpc(SendTo.ClientsAndHost)] 
    private void ShowPhotoClientRPC(byte[] PNG)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(PNG);
        PhotoShower.SetActive(true);
        PhotoShower.GetComponent<MeshRenderer>().material.mainTexture = tex;
    }
    
    public void TakeScreenshot(InputAction.CallbackContext context)
    {
        if(IsOwner && !Menu.activeSelf && context.started && photos.Count < maxPhotos)
        {
            StartCoroutine(Screenshot(Screen.width, Screen.height));
        }
    }
}
