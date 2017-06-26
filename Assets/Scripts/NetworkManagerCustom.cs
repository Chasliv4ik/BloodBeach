using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    private void Start()
    {
        Instance = this;
        PlayerInfos = new List<PlayerInfo>();
    }

    //On Button Create Host
    public void StartupHost()
    {
        
    }

}
