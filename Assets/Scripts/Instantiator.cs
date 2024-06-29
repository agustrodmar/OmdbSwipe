using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// Resharper Disable All 
/// <summary>
/// Class for instantiating card prefabs and assigning them movie IDs.
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class Instantiator : MonoBehaviour
{
    public GameObject cardPrefab; // The card prefab to instantiate
    public InputField searchInputField; // The input field for movie search
    private List<string> _movieIds = new List<string>(); // List of movie IDs
    // private readonly List<GameObject> _inactiveCards = new List<GameObject>(); // List of inactive cards that can be reused

    /// <summary>
    /// Instantiates a new card or reuses an inactive one, assigns it a movie ID, and sets it as the first sibling.
    /// </summary>
    void InstantiateCard(int index)
    {
        GameObject newCard = Instantiate(cardPrefab, transform, false);
        

        // Pass the movie ID to MovieLoader
        var movieLoader = newCard.GetComponent<MovieLoader>();
        if (movieLoader != null)
        {
            movieLoader.SetMovieId(_movieIds[index]);
        }
        else
        {
            throw new Exception("MovieLoader component not found on the new card.");
        }

        newCard.transform.SetAsFirstSibling();
    }

    /// <summary>
    /// Initiates a search for movies based on the user's input.
    /// </summary>
    public void SearchMovies()
    {
        string searchQuery = searchInputField.text;
        if (!string.IsNullOrEmpty(searchQuery))
        {
            StartCoroutine(GetMovieIds(searchQuery));
        }
    }

    /// <summary>
    /// Coroutine to get movie IDs from the OMDB API based on the search query.
    /// </summary>
    private IEnumerator GetMovieIds(string searchQuery)
    {
        if (!string.IsNullOrEmpty(searchQuery))
        {
            string apiUrl = "https://www.omdbapi.com/?s=" + searchQuery + "&apikey=c1230b12";
            UnityWebRequest www = UnityWebRequest.Get(apiUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception("Error while fetching movie IDs: " + www.error);
            }
            else
            {
                var json = www.downloadHandler.text;
                var searchResults = JsonUtility.FromJson<SearchResults>(json);
                _movieIds = searchResults.Search.Select(movie => movie.imdbID).ToList();
            }
        }
    }

    /// <summary>
    /// Called once per frame. If the number of child objects is less than the number of movie IDs, instantiate a new card.
    /// </summary>
    void Update()
    {
        if (transform.childCount < _movieIds.Count)
        {
            InstantiateCard(transform.childCount);
        }
        else
        {
            // All movies have been shown
        }
    }
}

[Serializable]
public class SearchResults
{
    public List<MovieData> Search;
}