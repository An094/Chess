using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public abstract class Board : MonoBehaviour
{
    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;

    private Piece[,] grid;
    private Piece selectedPiece;
    public Piece lastMovedPiece;
    private ChessGameController chessController;
    private SquareSelectorCreator squareSelector;

    public const int BOARD_SIZE = 8;

    public abstract void SelectPieceMoved(Vector2 coords);   
    public abstract void SetSelectedPiece(Vector2 coords);   

    protected virtual void Awake()
    {
        squareSelector = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    public void SetDependencies(ChessGameController chessController)
    {
        //squareSelector = GetComponent<SquareSelectorCreator>();
        this.chessController = chessController;
    }

    private void CreateGrid()
    {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE/2;
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE/2;
        return new Vector2Int(x, y);
    }

    public void OnGameRestarted()
    {
        selectedPiece = null;
        CreateGrid();
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        if(!chessController || !chessController.CanPerformMove())
        {
            return;
        }
        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);
        if(selectedPiece)
        {
            if(piece != null && selectedPiece == piece)
            {
                DeselectPiece();
            }
            else if(piece != null && selectedPiece != piece && chessController.IsTeamTurnActive(piece.team))
            {
                SelectPiece(coords);
            }
            else if(selectedPiece.CanMoveTo(coords))
            {
                SelectPieceMoved(coords);
            }
        }
        else
        {
            if(piece != null && chessController.IsTeamTurnActive(piece.team))
            {
                SelectPiece(coords);
            }
        }
    }

    public void PromotePiece(Piece piece)
    {
        TakePiece(piece);
        chessController.CreatePieceAndInitialize(piece.occupiedSquare, piece.team, typeof(Queen));
    }

    public void OnSelectedPieceMoved(Vector2Int coords)
    {
        TryToTakeOppositePiece(coords);
        UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
        selectedPiece.MovePiece(coords);
        DeselectPiece();
        EndTurn();
    }
    public void OnSetSelectedPiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        selectedPiece = piece;
    }

    private void TryToTakeOppositePiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        if(piece != null && !selectedPiece.IsFromSameTeam(piece))
        {
            TakePiece(piece);
        }
    }

    private void TakePiece(Piece piece)
    {
        if(piece)
        {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessController.OnPieceRemoved(piece);
        }
    }

    public void EnPassant(Pawn pawn, Piece enPassantPiece)
    {
        TakePiece(enPassantPiece);
    }

    private void EndTurn()
    {
        chessController.EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
    }

    private void SelectPiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        chessController.RemoveMovesEnablingAttackOnPiecOfType<King>(piece);
        SetSelectedPiece(coords);
        List<Vector2Int> selection = selectedPiece.availableMoves;
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for( int i = 0; i < selection.Count; i++ )
        {
            Vector3 position = CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
            squaresData.Add(position, isSquareFree);
        }
        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectPiece()
    {
        selectedPiece = null;
        squareSelector.ClearSelection();
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if(CheckIfCoordinatedAreOnBoard(coords))
        {
            return grid[coords.x, coords.y];
        }
        return null;
    }

    public bool CheckIfCoordinatedAreOnBoard(Vector2Int coords)
    {
        if(coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
        {
            return false;
        }
        return true;
    }

    public bool HasPiece(Piece piece)
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if(grid[i,j] == piece)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if(CheckIfCoordinatedAreOnBoard(coords))
        {
            grid[coords.x, coords.y] = piece;
        }
    }
}
