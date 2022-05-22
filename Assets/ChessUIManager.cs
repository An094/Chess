using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChessUIManager : MonoBehaviour
{
    [Header("Scene Dependencies")]
    [SerializeField] private NetworkManager networkManager;

    [Header("Buttons")]
    [SerializeField] private Button blackTeamButton;
    [SerializeField] private Button whiteTeamButton;

    [Header("Text")]
    [SerializeField] private Text resultText;
    [SerializeField] private Text connectionStatusText;

    [Header("Screen GameObjects")]
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private GameObject connectScreen;
    [SerializeField] private GameObject teamSelectionScreen;
    [SerializeField] private GameObject gameModeSelectionScreen;

    [Header("Other UI")]
    [SerializeField] private Dropdown gameLevelSelection;

    public void SetConnectionStatus(string status)
    {
        connectionStatusText.text = status;
    }

    private void Awake()
    {
        gameLevelSelection.AddOptions(Enum.GetNames(typeof(ChessLevel)).ToList());
        OnGameLaunched();
    }

    private void DisableAllScreens()
    {
        gameoverScreen.SetActive(false);
        connectScreen.SetActive(false);
        teamSelectionScreen.SetActive(false);
        gameModeSelectionScreen.SetActive(false);
    }

    public void OnGameStarted()
    {
        DisableAllScreens();
        connectionStatusText.gameObject.SetActive(false);
    }

    private void OnGameLaunched()
    {
        DisableAllScreens();
        gameModeSelectionScreen.SetActive(true);
    }

    public void OnSingleplayerModeSelected()
    {
        DisableAllScreens();
    }

    public void OnMultiplayerModeSelected()
    {
        connectionStatusText.gameObject.SetActive(true);
        DisableAllScreens();
        connectScreen.SetActive(true);
    }

    public void OnConnect()
    {
        networkManager.SetPlayerLevel((ChessLevel)gameLevelSelection.value);
        networkManager.Connect();
    }

    public void ShowTeamSelectionScreen()
    {
        DisableAllScreens();
        teamSelectionScreen.SetActive(true);
    }

    public void SelectTeam(int team)
    {
        networkManager.SelectTeam(team);
    }

    public void RestrictTeamChoice(TeamColor occupiedTeam)
    {
        var buttonToDeactivate = occupiedTeam == TeamColor.White ? whiteTeamButton : blackTeamButton;
        buttonToDeactivate.interactable = false;
    }

    public void OnGameFinised(string winner)
    {
        gameoverScreen.SetActive(true);
        resultText.text = String.Format("{0} won", winner);
    }
}
