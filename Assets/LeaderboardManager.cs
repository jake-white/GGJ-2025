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
using System.Collections.Generic;
using System.Collections;

public class LeaderboardManager : SingletonBehavior<LeaderboardManager>
{
    const string LeaderboardId = "ggj-2025";
    string myId;
    public List<LeaderboardDisplay> displays;
    IEnumerator coroutine;

    override protected async void Awake()
    {
        base.Awake();
        await UnityServices.InitializeAsync();

        await SignInAnonymously();
        coroutine = CheckLeaderboard();
        StartCoroutine(coroutine);
    }

    IEnumerator CheckLeaderboard()
    {
        while(true)
        {
            GetScores();
            yield return new WaitForSeconds(30.0f);
        }
    }

    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            myId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in as: " + myId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void AddScore(float score)
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        GetScores();
    }

    public async void GetScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));

        List<string> nameList = new List<string>();
        List<string> scoreList = new List<string>();

        foreach (LeaderboardEntry entry in scoresResponse.Results)
        {
            string name = "";
            if(entry.PlayerId == myId)
            {
                name += "<color=#AB6F3E>";
            }
            name += entry.PlayerName;
            if (entry.PlayerId == myId)
            {
                name += "</color>";
            }
            nameList.Add(name);
            scoreList.Add(entry.Score.ToString("0.#"));
        }
        foreach(LeaderboardDisplay display in displays)
        {
            display.Display(nameList, scoreList);
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
