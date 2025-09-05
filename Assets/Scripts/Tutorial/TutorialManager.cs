using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Drop on an empty GameObject (host/authority).
/// Assign EnemySpawner, TutorialUI, and list of PlayerTutorialProxy (your active players).
public class TutorialManager : MonoBehaviour
{
    private enum TutorialState
    {
        None,
        ShowAttack, WaitAttacks,
        ShowMove,   WaitMove,
        ShowBlock,  WaitBlock,
        Done
    }

    [Header("Refs")]
    [SerializeField] private TutorialUI ui;

    [Header("Thresholds")]
    [SerializeField] private int attacksPerPlayer = 3;
    [SerializeField] private float moveSecondsPerPlayer = 4f;
    [SerializeField] private int blocksPerPlayer = 2;

    [Header("Players")]
    [SerializeField] private List<PlayerTutorialProxy> players = new();

    private TutorialState _state = TutorialState.None;

    // Progress tracking
    private readonly Dictionary<int, int> _attacks = new();
    private readonly Dictionary<int, float> _moveSecs = new();
    private readonly Dictionary<int, int> _blocks = new();

    private void OnDisable()
    {
        foreach (var player in PlayerEntity.PlayerList)
        {
            player.gameObject.GetComponent<PlayerCombatManager>().RestoreHealthByPercent(100f);
            player.gameObject.GetComponent<MainPlayerController>().inTutorial = false;
        }
        
        UnsubscribePlayers(players);
    }

    private void Start()
    {
        foreach (var player in PlayerEntity.PlayerList)
            players.Add(player.GetComponent<PlayerTutorialProxy>());
        
        if (players.Count < 1)
        {
            Debug.LogWarning("TutorialManager: No players assigned/found, disabling self.");
            this.enabled = false;
            return;
        }
        
        SubscribePlayers(players);
        SetState(TutorialState.ShowAttack);
    }

    private void SubscribePlayers(List<PlayerTutorialProxy> list)
    {
        foreach (var p in list)
        {
            if (!p) continue;
            EnsurePlayer(p);
            p.OnAttack += HandleAttack;
            p.OnBlock  += HandleBlock;
            p.OnMoved  += HandleMoved;
        }
    }

    private void UnsubscribePlayers(List<PlayerTutorialProxy> list)
    {
        foreach (var p in list)
        {
            if (!p) continue;
            p.OnAttack -= HandleAttack;
            p.OnBlock  -= HandleBlock;
            p.OnMoved  -= HandleMoved;
        }
    }

    private void EnsurePlayer(PlayerTutorialProxy p)
    {
        if (!_attacks.ContainsKey(p.PlayerId)) _attacks[p.PlayerId] = 0;
        if (!_moveSecs.ContainsKey(p.PlayerId)) _moveSecs[p.PlayerId] = 0f;
        if (!_blocks.ContainsKey(p.PlayerId))  _blocks[p.PlayerId]  = 0;
    }

    private void SetState(TutorialState next)
    {
        _state = next;

        switch (_state)
        {
            case TutorialState.ShowAttack:
                if (ui) ui.ShowAttackIcon(true);
                SetState(TutorialState.WaitAttacks);
                break;

            case TutorialState.ShowMove:
                if (ui) { ui.ShowAttackIcon(false); ui.ShowMoveStick(true); }
                SetState(TutorialState.WaitMove);
                break;

            case TutorialState.ShowBlock:
                if (ui) { ui.ShowMoveStick(false); ui.ShowBlockIcon(true); }
                SetState(TutorialState.WaitBlock);
                break;

            case TutorialState.Done:
                if (ui) ui.HideAll();
                // Start main game here if needed, then disable this tutorial:
                this.enabled = false; // stops further logic
                break;
        }
    }

    private void TryAdvance()
    {
        if (_state == TutorialState.WaitAttacks)
        {
            if (AllPlayersMeet(pid => _attacks[pid] >= attacksPerPlayer))
                SetState(TutorialState.ShowMove);
        }
        else if (_state == TutorialState.WaitMove)
        {
            if (AllPlayersMeet(pid => _moveSecs[pid] >= moveSecondsPerPlayer))
                SetState(TutorialState.ShowBlock);
        }
        else if (_state == TutorialState.WaitBlock)
        {
            if (AllPlayersMeet(pid => _blocks[pid] >= blocksPerPlayer))
                SetState(TutorialState.Done);
        }
    }

    
    private bool AllPlayersMeet(System.Func<int, bool> predicate)
    {
        return players.Where(p => p != null).All(p => predicate(p.PlayerId));
    }


    private void HandleAttack(int playerId)
    {
        if (_state != TutorialState.WaitAttacks) return;
        _attacks[playerId]++;
        TryAdvance();
    }

    
    private void HandleMoved(int playerId, float deltaSeconds)
    {
        if (_state != TutorialState.WaitMove) return;
        _moveSecs[playerId] += deltaSeconds;
        TryAdvance();
    }

    
    private void HandleBlock(int playerId)
    {
        if (_state != TutorialState.WaitBlock) return;
        _blocks[playerId]++;
        TryAdvance();
    }

    //todo: call this when players join/leave
    public void RebuildPlayers(List<PlayerTutorialProxy> currentPlayers)
    {
        UnsubscribePlayers(players);
        players = currentPlayers ?? new List<PlayerTutorialProxy>();
        foreach (var p in players) if (p) EnsurePlayer(p);
        SubscribePlayers(players);
        TryAdvance();
    }
}
