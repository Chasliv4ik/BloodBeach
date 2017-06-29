using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetworkManagerCustom : NetworkManager {

    public enum PlayerState
    {
        Connecting,
        Playing,
        Dead
    }

    [Serializable]
    public class PlayerInfo
    {
        public NetworkConnection Connection;
        public string Name;
        public PlayerState State;
        public Transform PlayerTransform;
        public bool IsReady;


        public PlayerInfo(NetworkConnection connection, string name, PlayerState state, Transform playerTransform, bool isReady)
        {
            Connection = connection;
            Name = name;
            State = state;
            PlayerTransform = playerTransform;
            IsReady = isReady;
        }

    }

    public static NetworkManagerCustom Instance;
    public List<PlayerInfo> PlayerInfos;
    public int GameCode;

    private void Start()
    {
        Instance = this;
        PlayerInfos = new List<PlayerInfo>();
    }

    //On Button Create Host
    public void StartupHost()
    {
        //singleton.StartMatchMaker();
        GameCode = Random.Range(1000000, 10000000);
        singleton.StartMatchMaker();
        singleton.matchMaker.ListMatches(0, 20, GameCode.ToString(), false, 0, 1, OnCreateMatchList);

       
    }

    public virtual void OnCreateMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
    {
        if (success)
        {
            if (responseData.Count != 0) //Checking if there is any matches with generated name
            {
                //Match exists
            }
            else
            {
                //Match doesn't exists, creating it
                singleton.matchMaker.CreateMatch(GameCode.ToString(), 2, true, "", "", "", 0, 1, OnMatchCreate);
            }
        }
        else
        {
            //Error while searching for match
        }
    }

    public void JoinGame()
    {
        singleton.StartMatchMaker();
        singleton.matchMaker.ListMatches(0, 20, matchName, false, 0, 1, OnJoinMatchList);
        SetMatchNamePort();
    }

    private void OnJoinMatchList(bool success, string extendedinfo, List<MatchInfoSnapshot> responsedata)
    {
        if (success)
        {
            if (responsedata.Count > 0)
            {
                //Match found. Connecting
                singleton.matchMaker.JoinMatch(responsedata[0].networkId, "", "", "", 0, 1, OnMatchJoined);
            }
            else
            {
                //Match not found
            }
        }
        else
        {
            //Error while searching for match
        }
    }

    private void SetMatchNamePort()
    {
        string ipAddress = matchName;
        singleton.networkAddress = ipAddress;
        singleton.networkPort = 7777;
    }

    public void SetupMainMenuButtons(Button host, Button join)
    {
        host.onClick.RemoveAllListeners();
        host.onClick.AddListener(StartupHost);

        join.onClick.RemoveAllListeners();
        join.onClick.AddListener(JoinGame);
    }

}
