using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerChessGameController : ChessGameController
{
    public override bool CanPerformMove()
    {
        if(!IsGameInProgress())
        {
            return false;
        }
        return true;
    }

    public override void TryToStartCurrentGame()
    {
        SetGameState(GameState.Play);
    }

    protected override void SetGameState(GameState state)
    {
        this.state = state;
    }
}
