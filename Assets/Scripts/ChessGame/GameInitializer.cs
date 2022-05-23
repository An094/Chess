using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Game mode dependent objects")]
    [SerializeField] private SingleplayerChessGameController singleplayerChessGameControllerPrefab;
    [SerializeField] private MultiplayerChessGameController multiplayerChessGameControllerPrefab;
    [SerializeField] private SingleplayerBoard singleplayerBoardPrefab;
    [SerializeField] private MultiplayerBoard multiplayerBoardPrefab;

    [Header("Scene references")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private ChessUIManager uIManager;
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private CameraSetup cameraSetup;

    public void CreateMultiplayerBoard()
    {
        //if(networkManager.IsRoomFull())
        {
            PhotonNetwork.Instantiate(multiplayerBoardPrefab.name, boardAnchor.position, boardAnchor.rotation);

        }
    }

    public void CreateSingleplayerBoard()
    {
        Instantiate(singleplayerBoardPrefab, boardAnchor);
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerBoard board = FindObjectOfType<MultiplayerBoard>();
        if(board)
        {
            MultiplayerChessGameController controller = Instantiate(multiplayerChessGameControllerPrefab);
            controller.SetDependencies(uIManager, board, cameraSetup);
            controller.CreatePlayer();
            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);
            board.SetDependencies(controller);
        }
    }

    public void InitializeSingleplayerController()
    {
        SingleplayerBoard board = FindObjectOfType<SingleplayerBoard>();
        if(board)
        {
            SingleplayerChessGameController controller = Instantiate(singleplayerChessGameControllerPrefab);
            controller.SetDependencies(uIManager, board, cameraSetup);
            controller.CreatePlayer();
            board.SetDependencies(controller);
            controller.StartNewGame();
        }
    }
}
