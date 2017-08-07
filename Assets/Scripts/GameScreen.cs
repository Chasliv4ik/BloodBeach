using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public static GameScreen Instance;

    [SerializeField] private Image PanelPreperation;
    [SerializeField] private Image PanelGameInProgress;

    [Serializable]
    public class PlayerUi
    {
        public Toggle ToggleReady;
        public Text TextScore;
    }

    public List<PlayerUi> PlayerUis;


	private void Awake ()
	{
	    Instance = this;
	}

    private void Start()
    {
        
    }

    public void SetupReadyButtons()
    {
        int localPlayerIndex = GameObject.Find("Local Player")
            .GetComponent<NetworkBehaviour>()
            .connectionToServer.connectionId;

        PlayerUis[localPlayerIndex].ToggleReady.interactable = true;
    }

    public void SendLocalPlayerReadiness(bool value)
    {
        GameObject.Find("Local Player").GetComponent<PlayerController>().Cmd_SetPlayerReadyness(value);
    }

    public void SetPlayerReadiness(int connectionId, bool isReady)
    {
        //Debug.Log("SetPlayerReadiness");
        //PlayerUis[connectionId].ToggleReady.isOn = isReady;
        PlayerUis[connectionId].ToggleReady.Set(isReady, false);

    }

    public void StartGame()
    {
        NetworkManagerCustom.Instance.GameIsRunning = true;

        PanelPreperation.gameObject.SetActive(false);
        PanelGameInProgress.gameObject.SetActive(true);
    }
}

public static class UISetExtensions
{
    static MethodInfo toggleSetMethod;

    static UISetExtensions()
    {
        MethodInfo[] methods = typeof(Toggle).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        for (var i = 0; i < methods.Length; i++)
        {
            if (methods[i].Name == "Set" && methods[i].GetParameters().Length == 2)
            {
                toggleSetMethod = methods[i];
                break;
            }
        }
    }
    public static void Set(this Toggle instance, bool value, bool sendCallback)
    {
        toggleSetMethod.Invoke(instance, new object[] { value, sendCallback });
    }
}
