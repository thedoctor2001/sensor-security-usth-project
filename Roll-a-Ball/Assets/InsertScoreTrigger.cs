using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class InsertScoreTrigger : MonoBehaviour
{
    [Header("Score Information")]
    public string userID;
    public int value;
    public string token;

    // Method to start the score insertion process
    public void InsertScore()
    {
        StartCoroutine(PostScore(userID, value, token));
    }

    private IEnumerator PostScore(string userID, int value, string token)
    {
        // Create the JSON object
        var json = JsonUtility.ToJson(new ScoreData
        {
            userID = userID,
            value = value
        });

        // Convert JSON to byte array
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        // Create a new UnityWebRequest for a POST request
        UnityWebRequest request = new UnityWebRequest("https://localhost:3000/insert-score", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

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
    public class ScoreData
    {
        public string userID;
        public int value;
    }
}