using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField postCodeInputField;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private Button searchButton;
    [SerializeField] private Image weatherIcon;

    private string apiKey = "31cc40efd9094f0eab9175657241707";

    private void Start()
    {
        searchButton.onClick.AddListener(Search);
    }

    private void Search()
    {
        string postCode = postCodeInputField.text;

        if (string.IsNullOrEmpty(postCode))
        {
            Debug.Log("Inputfield Empty");
            return;
        }

        StartCoroutine(GetWeatherData(postCode));
    }

    private IEnumerator GetWeatherData(string postCode)
    {


        string url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={postCode}&aqi=no";

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();


        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {

            Debug.Log(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            LoadWeatherData(json);

        }
    }

    private void LoadWeatherData(string json)
    {
        WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);

        temperatureText.text = weatherResponse.current.temp_c.ToString() + "°C";
    }

}
