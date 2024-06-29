using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming

/// <summary>
/// This class is responsible for loading movie data and poster from the OMDb API.
/// </summary>
public class MovieLoader : MonoBehaviour
{
    private readonly string _url = "https://www.omdbapi.com/?i={0}&apikey=c1230b12";
    private string _movieId;
    public Image noMoreMoviesImage;
    private Image _image; // Cache the Image component
    public Text titleText;
    public Text yearText;
    public Text typeText;
    public Sprite loadingSprite;

    public MovieData movieData;
    
    /// <summary>
    /// Sets the movie ID.
    /// </summary>
    /// <param name="movieId">The movie ID.</param>
    public void SetMovieId(string movieId)
    {
        _movieId = movieId;
    }

    private void Awake()
    {
        _image = GetComponent<Image>(); // Cache the Image component
    }
    
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if (!string.IsNullOrEmpty(_movieId))
        {
            StartCoroutine(GetMovieData(string.Format(_url, _movieId)));
        }
        
    }

    /// <summary>
    /// Coroutine to get movie data from the OMDb API.
    /// </summary>
    /// <param name="apiUrl">The API URL.</param>
    private IEnumerator GetMovieData(string apiUrl)
    {
        _image.sprite = loadingSprite;
        
        using var www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            noMoreMoviesImage.gameObject.SetActive(true);
        }
        else
        {
            var json = www.downloadHandler.text;
            movieData = JsonUtility.FromJson<MovieData>(json);

            if (movieData != null)
            {
                if (!string.IsNullOrEmpty(movieData.Poster))
                {
                    // que complete GetMoviePoster antes de continuar
                    yield return StartCoroutine(GetMoviePoster(movieData.Poster));
                }

                titleText.text = movieData.Title;
                yearText.text = movieData.Year;
                typeText.text = movieData.Type;
            }
        }
    }

    /// <summary>
    /// Coroutine to get the movie poster from the provided URL.
    /// </summary>
    /// <param name="posterUrl">The URL of the poster.</param>
    private IEnumerator GetMoviePoster(string posterUrl)
    {
        using var www = UnityWebRequestTexture.GetTexture(posterUrl);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) yield break;
        var texture = DownloadHandlerTexture.GetContent(www);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        _image.sprite = sprite;
    }
}

/// <summary>
/// This class represents the movie data returned by the OMDb API.
/// </summary>
[System.Serializable]
public class MovieData
{
    public string Title;
    public string Year;
    public string imdbID;
    public string Type;
    public string Poster;
    public bool Liked;
}
