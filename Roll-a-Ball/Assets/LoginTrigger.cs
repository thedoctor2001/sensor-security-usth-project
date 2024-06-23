using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginTrigger : MonoBehaviour
{
    [Header("User Login Information")]
    public string email;
    public string password;

    // Method to start the login process
    public void LoginUser()
    {
        Hash128 hash128 = new Hash128();
        hash128.Append(password);
        password = hash128.ToString();
        StartCoroutine(PostUserLogin(email, password));
    }

    private IEnumerator PostUserLogin(string email, string password)
    {
        // Create the JSON object
        var json = JsonUtility.ToJson(new UserLogin
        {
            email = email,
            password = password
        });

        // Convert JSON to byte array
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        // Create a new UnityWebRequest for a POST request
        UnityWebRequest request = new UnityWebRequest("https://localhost:3000/login", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Use the custom certificate handler to bypass SSL verification
        request.certificateHandler = new CustomCertificateHandler();

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class UserLogin
    {
        public string email;
        public string password;
    }
}