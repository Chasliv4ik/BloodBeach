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

    private AsyncOperation _loadAsyncOperation;
    private void Start()
    {
        Instance = this;
        NetworkManagerCustom.Instance.SetupMainMenuButtons(ButtonHost, ButtonJoin);
    }

	public void LoadScene(string sceneName)
    {
        _loadAsyncOperation = SceneManager.LoadSceneAsync(sceneName);
       
    }

    void Update() 
    {
        if(_loadAsyncOperation !=null)
        MultiplayerStatus.text = (_loadAsyncOperation.progress*100f).ToString();
    }
    

    public void SetStatus(string statusMessage)
    {
        MultiplayerStatus.text = statusMessage;
        Debug.Log(statusMessage);
    }
}
