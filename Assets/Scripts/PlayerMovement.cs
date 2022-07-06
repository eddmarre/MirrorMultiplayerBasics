using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent playerAgent;

    #region Server

    [Command]
    private void CmdMove(Vector3 position)
    {
        //if not valid nav mesh position
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        playerAgent.SetDestination(hit.position);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    //client call back ensures only client will run code
    [ClientCallback]
    private void Update()
    {
        //if not owner
        if (!hasAuthority) return;
        
        if (!Input.GetMouseButton(1)) return;

        Ray screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(screenPointToRay, out RaycastHit hit, Mathf.Infinity)) return;

        CmdMove(hit.point);
    }

    #endregion
}