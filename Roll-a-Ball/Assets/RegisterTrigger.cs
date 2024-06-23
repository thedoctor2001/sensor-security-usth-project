using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RegisterTrigger : MonoBehaviour
{
    [Header("User Information")]
    public string username;
    public string email;
    public string hashedPassword;

    // Method to start the registration process
    public void RegisterUser()
    {
        Hash128 hash128 = new Hash128();
        hash128.Append(hashedPassword);
        hashedPassword = hash128.ToString();
        StartCoroutine(PostUserRegistration(username, email, hashedPassword));
    }

    private IEnumerator PostUserRegistration(string username, string email, string hashedPassword)
    {
        // Create the JSON object
        var json = JsonUtility.ToJson(new User
        {
            username = username,
            email = email,
            hashedPassword = hashedPassword
        });

        // Convert JSON to byte array
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        // Create a new UnityWebRequest for a POST request
        UnityWebRequest request = new UnityWebRequest("https://localhost:3000/insert-user", "POST");
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
    public class User
    {
        public string username;
        public string email;
        public string hashedPassword;
    }
}