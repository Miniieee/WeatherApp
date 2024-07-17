using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    [Header("Input Field")]
    [SerializeField] private TMP_InputField locationInputField;

    [Header("Text elements")]
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI humidtyText;
    [SerializeField] private TextMeshProUGUI windSpeedText;

    [Header("Button")]
    [SerializeField] private Button searchButton;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Button previousDayButton;


    [Header("Images")]
    [SerializeField] private Image weatherIcon;

    private string apiKey = "31cc40efd9094f0eab9175657241707";

    private void Start()
    {
        searchButton.onClick.AddListener(Search);
    }

    private void Search()
    {
        string location = locationInputField.text;

        if (string.IsNullOrEmpty(location))
        {
            Debug.Log("Inputfield Empty");
            return;
        }

        StartCoroutine(GetWeatherData(location));
    }

    private IEnumerator GetWeatherData(string postCode)
    {
        string url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={postCode}&days=3&aqi=no&alerts=no";
        

        LoadingVisual();

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();


        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {

            Debug.Log(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            ProcessWeatherData(json);
        }
    }

    
    private void ProcessWeatherData(string json)
    {
        WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);
        LocationResponse locationResponse = JsonUtility.FromJson<LocationResponse>(json);

        //write out the data to the UI
        temperatureText.text = weatherResponse.current.temp_c.ToString() + "°C";
        locationText.text = locationResponse.location.name;
        conditionText.text = weatherResponse.current.condition.text;
        humidtyText.text = weatherResponse.current.humidity.ToString() + "%";
        windSpeedText.text = weatherResponse.current.wind_mph.ToString() + "mph";

        string iconUrl = weatherResponse.current.condition.icon;
        StartCoroutine(LoadIcon(iconUrl)); 
    }

    private IEnumerator LoadIcon(string iconUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(iconUrl);
        yield return request.SendWebRequest();

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        weatherIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    private void LoadingVisual()
    {
        temperatureText.text = "Loading";
        locationText.text = "Loading";
        conditionText.text = "Loading";
        humidtyText.text = "Loading";
        windSpeedText.text = "Loading";
    }

}
