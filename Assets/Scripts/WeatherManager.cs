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
    [SerializeField] private Sprite loadingSprite;

    private string apiKey = "31cc40efd9094f0eab9175657241707";

    private string jsonData;

    private void Start()
    {
        //set up button listeners
        searchButton.onClick.AddListener(SearchCurrent);
        nextDayButton.onClick.AddListener(LoadForecast);
        previousDayButton.onClick.AddListener(SearchCurrent);
    }

    private void SearchCurrent()
    {
        //get location from input field
        string location = locationInputField.text;

        //check if input field is empty
        if (string.IsNullOrEmpty(location))
        {
            Debug.Log("Inputfield Empty");
            return;
        }


        CurrentDayLayout();
        StartCoroutine(GetWeatherData(location));
    }

    private void LoadForecast()
    {
        NextDayLayout();
        ProcessForecastWeatherData(jsonData);
    }


    private IEnumerator GetWeatherData(string postCode)
    {
        //store the url
        string url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={postCode}&days=3&aqi=no&alerts=no";

        //while it makes the request, it loads the loading visual
        LoadingVisual();

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();


        //check if the request was successful
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //get the json data
            jsonData = request.downloadHandler.text;
            //process the data
            ProcessCurrentWeatherData(jsonData);
        }
    }

    private void ProcessCurrentWeatherData(string json)
    {
        //convert the json data to a C# object
        WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);
        LocationResponse locationResponse = JsonUtility.FromJson<LocationResponse>(json);

        //write out the data to the UI
        temperatureText.text = weatherResponse.current.temp_c.ToString() + "°C";
        locationText.text = locationResponse.location.name;
        conditionText.text = weatherResponse.current.condition.text;
        humidtyText.text = weatherResponse.current.humidity.ToString() + "%";
        windSpeedText.text = weatherResponse.current.wind_mph.ToString() + "mph";

        //get the icon url
        string iconUrl = weatherResponse.current.condition.icon;

        //load the icon
        StartCoroutine(LoadIcon(iconUrl));
    }

    private void ProcessForecastWeatherData(string json)
    {
        //convert the json data to a C# object
        WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);
        LocationResponse locationResponse = JsonUtility.FromJson<LocationResponse>(json);

        //write out next day's data to the UI
        temperatureText.text = weatherResponse.forecast.forecastday[1].day.avgtemp_c.ToString() + "°C";
        locationText.text = locationResponse.location.name;
        conditionText.text = weatherResponse.forecast.forecastday[1].day.condition.text;
        humidtyText.text = weatherResponse.forecast.forecastday[1].day.avghumidity.ToString() + "%";
        windSpeedText.text = weatherResponse.forecast.forecastday[1].day.maxwind_mph.ToString() + "mph";

        //get the icon url
        string iconUrl = weatherResponse.forecast.forecastday[1].day.condition.icon;

        //load the icon
        StartCoroutine(LoadIcon(iconUrl));
    }


    private IEnumerator LoadIcon(string iconUrl)
    {

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(iconUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //download the icon
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            //set the icon sprite
            weatherIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }


    #region Loading Visual
    private void LoadingVisual()
    {
        temperatureText.text = "Loading";
        locationText.text = "Loading";
        conditionText.text = "Loading";
        humidtyText.text = "Loading";
        windSpeedText.text = "Loading";

        weatherIcon.sprite = loadingSprite;
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
