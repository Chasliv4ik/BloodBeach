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
        Connected,
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

        public PlayerInfo(NetworkConnection connection)
        {
            Connection = connection;
            State = PlayerState.Connecting;
        }

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
    public List<PlayerInfo> PlayersInfo;
    public int GameCode;
    public bool GameIsRunning = false;

    private void Start()
    {
        Instance = this;
        PlayersInfo = new List<PlayerInfo>();
    }

    #region Creating/Joining Match

    public void StartupHost()
    {
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
                MenuController.Instance.SetStatus("Match Exists");
            }
            else
            {
                MenuController.Instance.SetStatus("Match doesn't exists. Creating");
                singleton.matchMaker.CreateMatch(GameCode.ToString(), 2, true, "", "", "", 0, 1, OnMatchCreate);
            }
        }
        else
        {
            MenuController.Instance.SetStatus("Create match error");
        }
    }

    public void JoinGame()
    {
        singleton.StartMatchMaker();
        singleton.matchMaker.ListMatches(0, 20, "", false, 0, 1, OnJoinMatchList);
        SetMatchNamePort();
    }

    private void OnJoinMatchList(bool success, string extendedinfo, List<MatchInfoSnapshot> responsedata)
    {
        if (success)
        {
            if (responsedata.Count != 0)
            {
                MenuController.Instance.SetStatus("Joining match");
                singleton.matchMaker.JoinMatch(responsedata[0].networkId, "", "", "", 0, 1, OnMatchJoined);
            }
            else
            {
                MenuController.Instance.SetStatus("No matches found");
            }
        }
        else
        {
            MenuController.Instance.SetStatus("Join match error");
        }
    }

    #endregion


    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        //PlayerInfo connectingPlayer = new PlayerInfo(conn); 
        PlayersInfo.Add(new PlayerInfo(conn));
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);

        GameObject newPlayer = conn.playerControllers[0].gameObject;
        PlayerInfo newPlayerInfo = PlayersInfo.Find(player => player.Connection == conn);
        newPlayerInfo.PlayerTransform = newPlayer.transform;
        newPlayerInfo.State = PlayerState.Connected;
    }

    public void SetPlayerReadiness(NetworkConnection conn, bool isReady)
    {
        PlayersInfo.Find(player => player.Connection == conn).IsReady = isReady;
        CheckAllPlayersReady();

    }

    private void CheckAllPlayersReady()
    {
        bool isEverybodyReady = true;

        foreach (var player in PlayersInfo)
        {
            if (!player.IsReady)
            {
                isEverybodyReady = false;
                break;
            }
        }

        if (isEverybodyReady && !GameIsRunning)
        {
            Debug.Log("EVERYBODY REAAAADY LOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOL");
            GameIsRunning = true;

            MatchInfo currentMatchInfo = singleton.matchInfo;
            singleton.matchMaker.SetMatchAttributes(singleton.matchInfo.networkId, false, currentMatchInfo.domain, (success, info) => { });

            foreach (var player in PlayersInfo)
            {
                player.PlayerTransform.gameObject.GetComponent<PlayerController>().Rpc_StartGame();
            }
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

    public void ResetConnections()
    {
        singleton.StopMatchMaker();
        singleton.StopHost();
        singleton.StopClient();
        NetworkServer.Reset();
    }
}
