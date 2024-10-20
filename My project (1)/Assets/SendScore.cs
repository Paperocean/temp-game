using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendScore : MonoBehaviour
{
    public Button sendScoreButton; // Przycisk do wysyłania wyniku
    public Button getScoreButton;   // Przycisk do pobierania wyniku
    public InputField scoreInputField; // Pole tekstowe do wprowadzania wyniku
    private string urlSend = "http://localhost:5084/sendscore"; // URL do wysyłania wyniku
    private string urlGet = "http://localhost:5084/getscore"; // URL do pobierania wyniku

    void Start()
    {
        // Przypisz funkcję do przycisku wysyłania wyniku
        sendScoreButton.onClick.AddListener(OnSendScoreClicked);
        // Przypisz funkcję do przycisku pobierania wyniku
        getScoreButton.onClick.AddListener(OnGetScoreClicked);
    }

    // Funkcja wywoływana po kliknięciu przycisku wysyłania
    void OnSendScoreClicked()
    {
        if (int.TryParse(scoreInputField.text, out int score))
        {
            StartCoroutine(SendScoreToServer(score));
        }
        else
        {
            Debug.LogError("Wprowadź prawidłowy wynik.");
        }
    }

    // Funkcja wywoływana po kliknięciu przycisku pobierania
    void OnGetScoreClicked()
    {
        StartCoroutine(GetScoreFromServer());
    }

    // Korutyna do wysyłania danych
    IEnumerator SendScoreToServer(int score)
    {
        string json = JsonUtility.ToJson(new ScoreData { Score = score });

        using (UnityWebRequest request = new UnityWebRequest(urlSend, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Wynik wysłany pomyślnie: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Błąd podczas wysyłania wyniku: " + request.error);
            }
        }
    }

    // Korutyna do pobierania danych
    IEnumerator GetScoreFromServer()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(urlGet))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ScoreData scoreData = JsonUtility.FromJson<ScoreData>(request.downloadHandler.text);
                Debug.Log("Ostatni wynik: " + scoreData.Score);
            }
            else
            {
                Debug.LogError("Błąd podczas pobierania wyniku: " + request.error);
            }
        }
    }
}

// Model danych do serializacji JSON
[System.Serializable]
public class ScoreData
{
    public int Score;
}
