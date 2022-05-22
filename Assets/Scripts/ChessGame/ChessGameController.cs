using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public abstract class ChessGameController : MonoBehaviour
{
    public enum GameState { Init, Play, Finished}
    [SerializeField] private BoardLayout startingBoardLayout;
    private Board board;
    private ChessUIManager uiManager;
    private CameraSetup cameraSetup;

    private PieceCreator pieceCreator;
    protected ChessPlayer whitePlayer;
    protected ChessPlayer blackPlayer;
    protected ChessPlayer activePlayer;
    protected GameState state;

    protected abstract void SetGameState(GameState state);
    public abstract void TryToStartCurrentGame();
    public abstract bool CanPerformMove();
    private void Awake()
    {
        pieceCreator = GetComponent<PieceCreator>();
        //SetDependencies();
        //CreatePlayer();
        
    }

    public void CreatePlayer()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    public void SetDependencies(ChessUIManager uIManager, Board board, CameraSetup cameraSetup)
    {
        this.uiManager = uIManager;
        this.board = board;
        this.cameraSetup = cameraSetup;
    }
    private void Start()
    {
        //StartNewGame();
    }

    public void StartNewGame()
    {
        uiManager.OnGameStarted();
        SetGameState(GameState.Init);
        //board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        TryToStartCurrentGame();
    }

    public void SetupCamera(TeamColor team)
    {
        cameraSetup.SetupCamera(team);
    }
    public void RestartGame()
    {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }


    public bool IsGameInProgress()
    {
        return state == GameState.Play; 
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoorsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if(CheckIfGameIsFinished())
        {
            EndGame();
        }
        else
        {
            ChangeActiveTeam();
        }
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }

    private bool CheckIfGameIsFinished()
    {
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        if(kingAttackingPieces.Length > 0)
        {
            ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

            int availableMoves = attackedKing.availableMoves.Count;
            if(availableMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing)
                    return true;
            }
        }
        return false;
    }

    private void EndGame()
    {
        Debug.Log("EndGame");
        uiManager.OnGameFinised(activePlayer.team.ToString());
        SetGameState(GameState.Finished);
    }

    private void ChangeActiveTeam()
    {
        //bool cond = activePlayer == whitePlayer;
        //if(cond)
        //{
        //    activePlayer = blackPlayer;
        //    Debug.Log("Change Active Player to black");
        //}
        //else
        //{
        //    activePlayer = whitePlayer;
        //    Debug.Log("Change Active Player to white");
        //}
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void RemoveMovesEnablingAttackOnPiecOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetOpponentToPlayer(activePlayer), piece);    
    }
}
