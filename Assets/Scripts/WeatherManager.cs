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

    private string jsonData;

    private void Start()
    {
        searchButton.onClick.AddListener(SearchCurrent);
        nextDayButton.onClick.AddListener(SearchForecast);
        previousDayButton.onClick.AddListener(SearchCurrent);
    }

    private void SearchCurrent()
    {
        string location = locationInputField.text;
        if (string.IsNullOrEmpty(location))
        {
            Debug.Log("Inputfield Empty");
            return;
        }

        CurrentDayLayout();
        StartCoroutine(GetWeatherData(location));
    }

    private void SearchForecast()
    {
        NextDayLayout();
        ProcessForecastWeatherData(jsonData);
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

            jsonData = request.downloadHandler.text;
            ProcessCurrentWeatherData(jsonData);
        }
    }

    private void ProcessCurrentWeatherData(string json)
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

    private void ProcessForecastWeatherData(string json)
    {
        WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);
        LocationResponse locationResponse = JsonUtility.FromJson<LocationResponse>(json);

        //write out the data to the UI
        temperatureText.text = weatherResponse.forecast.forecastday[1].day.avgtemp_c.ToString() + "°C";
        locationText.text = locationResponse.location.name;
        conditionText.text = weatherResponse.forecast.forecastday[1].day.condition.text;
        humidtyText.text = weatherResponse.forecast.forecastday[1].day.avghumidity.ToString() + "%";
        windSpeedText.text = weatherResponse.forecast.forecastday[1].day.maxwind_mph.ToString() + "mph";


        string iconUrl = weatherResponse.forecast.forecastday[1].day.condition.icon;
        StartCoroutine(LoadIcon(iconUrl));
    }

    
    private IEnumerator LoadIcon(string iconUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(iconUrl);
        yield return request.SendWebRequest();

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        weatherIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }


    #region Loading Visual
    private void LoadingVisual()
    {
        temperatureText.text = "Loading";
        locationText.text = "Loading";
        conditionText.text = "Loading";
        humidtyText.text = "Loading";
        windSpeedText.text = "Loading";
    }

    private void CurrentDayLayout()
    {
        nextDayButton.gameObject.SetActive(true);
        previousDayButton.gameObject.SetActive(false);
    }

    private void NextDayLayout()
    {
        nextDayButton.gameObject.SetActive(false);
        previousDayButton.gameObject.SetActive(true);
    }

    #endregion

}
