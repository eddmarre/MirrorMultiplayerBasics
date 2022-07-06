using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TextMeshPro displayNameText;
    [SerializeField] private Renderer displayRenderer;


    //hook calls function every time the variable is changed on client
    [SyncVar(hook = nameof(HandleDisplayColorUpdate))] [SerializeField]
    private Color displayColor;

    //syncvar -syncs variable data to the server
    [SyncVar(hook = nameof(HandleDisplayNameText))] [SerializeField]
    private string displayName = "missing name";

    #region Server

    //make sure the server is the only one who access this method
    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    [Server]
    public void SetPlayerColor()
    {
        displayColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    //clients calling method on server
    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        if (newDisplayName.Length < 2 || newDisplayName.Length > 20) return;

        RpcSetDisplayName(newDisplayName);


        SetDisplayName(newDisplayName);
    }

    #endregion

    #region Client

    //hook call
    private void HandleDisplayColorUpdate(Color oldColor, Color newColor)
    {
        displayRenderer.material.SetColor("_BaseColor", newColor);
    }

    //hook call
    private void HandleDisplayNameText(string oldText, string newText)
    {
        displayNameText.text = newText;
    }

    [ContextMenu("SetMyName")]
    private void SetMyName()
    {
        CmdSetDisplayName("my new Name");
    }

    //server calling method on client
    [ClientRpc]
    private void RpcSetDisplayName(String newDisplayName)
    {
        Debug.Log($"{newDisplayName}");
    }

    #endregion
}