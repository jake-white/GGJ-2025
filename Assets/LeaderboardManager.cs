using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using System;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;
using Unity.VisualScripting;
using CollabXR;
using TMPro;

public class LeaderboardManager : SingletonBehavior<LeaderboardManager>
{
    const string LeaderboardId = "ggj-2025";
    public TextMeshProUGUI names, scores;

    override protected async void Awake()
    {
        await UnityServices.InitializeAsync();

        await SignInAnonymously();

        AddScore();

        GetScores();
    }

    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void AddScore()
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, 102);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }

    public async void GetScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));

        names.text = "";
        scores.text = "";

        foreach(LeaderboardEntry entry in scoresResponse.Results)
        {
            names.text += entry.PlayerName + "\n";
            scores.text += entry.Score + "\n";
        }
    }

    //public async void GetPaginatedScores()
    //{
    //    Offset = 10;
    //    Limit = 10;
    //    var scoresResponse =
    //        await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
    //    Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    //}

    public async void GetPlayerScore()
    {
        var scoreResponse =
            await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }

    //public async void GetVersionScores()
    //{
    //    var versionScoresResponse =
    //        await LeaderboardsService.Instance.GetVersionScoresAsync(LeaderboardId, VersionId);
    //    Debug.Log(JsonConvert.SerializeObject(versionScoresResponse));
    //}
}
