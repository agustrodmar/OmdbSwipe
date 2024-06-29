using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for controlling the lists of liked and disliked movies.
/// </summary>
public class ListController : MonoBehaviour
{
    public GameObject textPrefab; // The text prefab to instantiate
    public Transform likesList; // The list for liked movies
    public Transform dislikesList; // The list for disliked movies

    /// <summary>
    /// Adds a movie title to the appropriate list (likes or dislikes).
    /// </summary>
    /// <param name="liked">Whether the movie was liked or not.</param>
    /// <param name="movieTitle">The title of the movie.</param>
    public void AddToList(bool liked, string movieTitle)
    {
        // Select the correct list
        Transform list = liked ? likesList : dislikesList;

        // Create a new text element and add it to the list
        GameObject textElement = Instantiate(textPrefab, list);
        Text textComponent = textElement.GetComponent<Text>();
        if (textComponent != null)
        {
            textComponent.text = movieTitle;
        }
        else
        {
            throw new Exception("Text component not found on the new text element.");
        }
    }
}
