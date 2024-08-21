using System;
using System.Collections;
using TMPro; 
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Requester : MonoBehaviour
{
    [SerializeField]
    string APIurl = "https://rickandmortyapi.com/api/character";

    [SerializeField]
    string DBurl = "https://my-json-server.typicode.com/EliteV1/SIDTaller2";

    [SerializeField]
    TextMeshProUGUI usernameText;  

    [SerializeField]
    RawImage[] cardImages;  

    [SerializeField]
    TextMeshProUGUI[] cardNames; 

    private int currentUserId = 1;  

    public int currentid; 

    void Start()
    {
        SendRequest(currentUserId);  
    }

    public void SendRequest(int id)
    {
        StartCoroutine(GetUser(id));
        currentid = id;
    }

    
    IEnumerator GetUser(int id)
    {
        UnityWebRequest request = UnityWebRequest.Get(DBurl + "/users/" + id);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                User user = JsonUtility.FromJson<User>(request.downloadHandler.text);

               
                usernameText.text = "Usuario: " + user.username;

                
                for (int i = 0; i < user.deck.Length && i < cardImages.Length; i++)
                {
                    StartCoroutine(GetCharacter(user.deck[i], i)); 
                }
            }
            else
            {
                string mensaje = "status:" + request.responseCode;
                mensaje += "\nError: " + request.error;
                Debug.Log(mensaje);
            }
        }
    }

   
    IEnumerator GetCharacter(int id, int index)
    {
        UnityWebRequest www = UnityWebRequest.Get(APIurl + "/" + id);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                Debug.Log(character.name + " is a " + character.species);

                
                cardNames[index].text = character.name;

                
                StartCoroutine(GetImage(character.image, index));
            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError: " + www.error;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator GetImage(string imageUrl, int index)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            cardImages[index].texture = texture;  
        }
    }

    
    public void NextUser()
    {
        
        currentUserId++;
        if (currentUserId > 3)
        {
            currentUserId = 1;
        }
        SendRequest(currentUserId);
    }
}


class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}

[Serializable]
class User
{
    public int id;
    public string username;
    public bool state;
    public int[] deck;
}