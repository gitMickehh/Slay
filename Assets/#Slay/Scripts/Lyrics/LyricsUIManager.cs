using System.Collections;
using System.Collections.Generic;
using Techno;
using UnityEngine;
using UnityEngine.UIElements;

public class LyricsUIManager : MonoBehaviour
{
    private LyricsManager lyricsManager;
    Label subtitleLabel;
    LyricLine currentLine;
    string[] splitContent;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        subtitleLabel = root.Q<Label>("subtitles");
    }

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        if (!ServiceLocator<LyricsManager>.HasService)
        {
            Debug.LogError("Lyrics Manager prefab is not found!");
        }
        else
        {
            lyricsManager = ServiceLocator<LyricsManager>.Service;
        }

        ServiceLocator<LyricsUIManager>.Service = this;
    }

    public void UpdateSubtitles(float timeStamp)
    {
        subtitleLabel.text = GetHighlightedString(timeStamp);
    }

    private string GetHighlightedString(float timeStamp)
    {
        string returnString = "<color=\"red\">";

        LyricLine lyricLine = lyricsManager.GetLyricLineFromTime(timeStamp);
        if (lyricLine != null)
        {
            //returnString = lyricLine.content;

            if (!lyricLine.IsEqual(currentLine))
            {
                currentLine = lyricLine;
                splitContent = lyricLine.content.Split(' ');
            }

            float completionPercentage = (timeStamp - lyricLine.begin) / (lyricLine.end - lyricLine.begin); //getting a progression number throughout the line's lifetime
            int completedWords = Mathf.CeilToInt(completionPercentage * splitContent.Length);

            for (int i = 0; i < splitContent.Length; i++)
            {
                if (i > completedWords) returnString += "</color>";
                returnString +=  " " + splitContent[i];
            }
        }

        currentLine = lyricLine;
        return returnString;
    }
}
