using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MultiplayerBoard : Board
{
    private PhotonView photonView;

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }
    public override void SelectPieceMoved(Vector2 coords)
    {
        photonView.RPC(nameof(RPC_OnSelectedPieceMoved), RpcTarget.AllBuffered, new object[] { coords });
    }

    public override void SetSelectedPiece(Vector2 coords)
    {
        photonView.RPC(nameof(RPC_OnSetSelectedPiece), RpcTarget.AllBuffered, new object[] { coords });

    }

    [PunRPC]
    private void RPC_OnSelectedPieceMoved(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSelectedPieceMoved(intCoords);
    }

    [PunRPC]
    private void RPC_OnSetSelectedPiece(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSetSelectedPiece(intCoords);
    }
}
