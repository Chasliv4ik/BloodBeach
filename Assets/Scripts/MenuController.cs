using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    [SerializeField] public Button ButtonHost;
    [SerializeField] public Button ButtonJoin;
    [SerializeField] public Text MultiplayerStatus;

    private void Start()
    {
        Instance = this;
        NetworkManagerCustom.Instance.SetupMainMenuButtons(ButtonHost, ButtonJoin);
    }

	public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetStatus(string statusMessage)
    {
        MultiplayerStatus.text = statusMessage;
        Debug.Log(statusMessage);
    }
}
