using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIController _uIController;
    [SerializeField] private TimeController _timeController;
    // Start is called before the first frame update

    private void Awake()
    {
        EventManager.OnGameOver.AddListener(GameOver);
    }
    void Start()
    {
        _timeController.SetPauseOn();
        _uIController.HideGamePanel();
        _uIController.ShowStartPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        print("game contr met play");
        _uIController.HideStartPanel();
        _uIController.ShowGamePanel();
        _uIController.HideGameOverPanel();
        _timeController.SetPauseOff();

    }

    public void Exit()
    {
        print("game contr met exit");
        _uIController.HideGamePanel();
        _uIController.ShowStartPanel();
        _uIController.HideGameOverPanel();
        _timeController.SetPauseOn();
    }

    public void GameOver()
    {
        _uIController.ShowGameOverPanel();
        _uIController.HideGamePanel();
        _uIController.HideStartPanel();
        _timeController.SetPauseOn();
    }

    public void Restart()
    {
        SceneManager.LoadScene("MiniGame");
    }

    public void Debug()
    {
        _timeController.SetPauseOn();
        _uIController.HideGamePanel();
        _uIController.HideStartPanel();
        _uIController.HideGameOverPanel();
    }

    public void OpenStore()
    {
        _timeController.SetPauseOn();
        _uIController.ShowStorePanel();
    }
    public void CloseStore()
    {
        _uIController.HideStorePanel();
    }

    private void Settings()
    {

    }
}
