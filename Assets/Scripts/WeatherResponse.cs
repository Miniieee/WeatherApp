using UnityEngine.Rendering;

[System.Serializable]
public class WeatherResponse
{
    public Current current;

    [System.Serializable]
    public class Current
    {
        public float temp_c;

        public Condition condition;

        [System.Serializable]
        public class Condition
        {
            public string text;
            public string icon;
        }
    }

}

/*

sample

  "location": {
    "name": "Stockport",
    "region": "Cheshire",
    "country": "UK",
    "lat": 53.4,
    "lon": -2.17,
    "tz_id": "Europe/London",
    "localtime_epoch": 1721241805,
    "localtime": "2024-07-17 19:43"
  },
  "current": {
    "last_updated_epoch": 1721241000,
    "last_updated": "2024-07-17 19:30",
    "temp_c": 21.1,
    "temp_f": 70,
    "is_day": 1,
    "condition": {
      "text": "Sunny",
      "icon": "//cdn.weatherapi.com/weather/64x64/day/113.png",
      "code": 1000
    },
    "wind_mph": 3.8,
    "wind_kph": 6.1,
    "wind_degree": 180,
    "wind_dir": "S",
    "pressure_mb": 1019,
    "pressure_in": 30.09,
    "precip_mm": 0.02,
    "precip_in": 0,
    "humidity": 60,
    "cloud": 0,
    "feelslike_c": 21.1,
    "feelslike_f": 70,
    "windchill_c": 17.7,
    "windchill_f": 63.8,
    "heatindex_c": 17.7,
    "heatindex_f": 63.8,
    "dewpoint_c": 13.7,
    "dewpoint_f": 56.7,
    "vis_km": 10,
    "vis_miles": 6,
    "uv": 4,
    "gust_mph": 8.8,
    "gust_kph": 14.2
  }
}*/
