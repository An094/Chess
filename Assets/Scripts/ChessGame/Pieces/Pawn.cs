using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public int firstMove;
    private Piece enPassantPiece;
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        float range = hasMoved ? 1 : 2;
        for (int i = 1; i <= range; i++)
        {
            Vector2Int nextCoords = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatedAreOnBoard(nextCoords))
            {
                break;
            }
            if (piece == null)
            {
                TryToAddMove(nextCoords);
            }
            else if (piece.IsFromSameTeam(this))
            {
                break;
            }
        }

        Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y) };
        for (int i = 0; i < takeDirections.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + takeDirections[i];
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if(!board.CheckIfCoordinatedAreOnBoard(nextCoords))
            {
                continue;
            }
            if(piece != null && !piece.IsFromSameTeam(this))
            {
                TryToAddMove(nextCoords);
            }
        }
        //Check en passant
        Vector2Int[] enpassant = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(-1, 0) };
        for(int i = 0; i < enpassant.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + enpassant[i];
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatedAreOnBoard(nextCoords) || piece == null)
            {
                continue;
            }
            else
            {
                if((piece is Pawn) && (board.lastMovedPiece == piece) && (!IsFromSameTeam(piece)))
                {
                    int firstMove = (piece as Pawn).firstMove;
                    if(firstMove == 2)
                    {
                        enPassantPiece = piece;
                        Vector2Int coords = occupiedSquare + takeDirections[i];
                        TryToAddMove(coords);
                        break;
                    }
                }
            }
        }


        return availableMoves;
    }

    public override void MovePiece(Vector2Int coords)
    {
        if(!hasMoved)
        {
            firstMove = team == TeamColor.White ? coords.y - 1 : 6 - coords.y;
        }
        if(CheckEnPassant(coords))
        {
            EnPassant();
        }
        base.MovePiece(coords);
        CheckPromotion();
    }

    private bool CheckEnPassant(Vector2Int coords)
    {
        if(coords.x != occupiedSquare.x && coords.y != occupiedSquare.y)
        {
            return true;
        }
        return false;
    }

    private void CheckPromotion()
    {
        int endOfBoardYCoord = team == TeamColor.White ? Board.BOARD_SIZE - 1 : 0;
        if (occupiedSquare.y == endOfBoardYCoord)
            board.PromotePiece(this);
    }

    private void EnPassant()
    {
        board.EnPassant(this, enPassantPiece);
    }
}
