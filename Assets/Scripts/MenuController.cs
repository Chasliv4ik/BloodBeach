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

    private void Start()
    {
        Instance = this;
        NetworkManagerCustom.Instance.SetupMainMenuButtons(ButtonHost, ButtonJoin);
    }

	public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}
