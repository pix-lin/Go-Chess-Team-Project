using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Fusion;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour, IPlayerJoined
{
    public bool keySwitched; //Player and UI input mode transition key [true: Player / false: UI]
    public int team; //1: Black / 2: White

    [Header("Settings")]
    public int boardSize = 20;
    public float startPos = 0.2465f; //왼쪽 아래(원점) 좌표
    public float spacingOffset = 0.0275f;
    //시작(-0.2465, 0, -0.2465) +-0.0275

    public GameObject playerPrefab;
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;
    public GameObject currentStone;

    [Header("State")]
    [Networked] public int currentTurn { get; set; } = 1; //Black(1), Whilte(2)
    [Networked] public bool isGameOver { get; set; }
    public struct GoChessBoard : INetworkStruct
    {
        public int IsPlaced; //0: 빈칸, 1: Black, 2: White
        public Vector3 Position;
    }

    [Networked, Capacity(400)] //20 x 20
    public NetworkArray<GoChessBoard> BoardArray => default;

    private InputSystem_Actions inputSystemActions;
    private int boardX = 9, boardY = 9; //center of board(20x20)
    private bool moveInputPressed;

    public override void Spawned()
    {
        if(Object.HasStateAuthority)
        {
            currentTurn = 1;
        }
    }

    public void OnEnable()
    {
        if (inputSystemActions == null)
            inputSystemActions = new InputSystem_Actions();

        inputSystemActions.Player.Enable();
    }

    public void OnDisable()
    {
        inputSystemActions.Player.Disable();
        inputSystemActions.UI.Disable();
    }

    private void Update()
    {
        if (Object == null || !Object.IsValid || !Object.HasInputAuthority || isGameOver)
            return;

        if (inputSystemActions == null)
            return;

        HandGridMovement();

        if(inputSystemActions.Player.Jump.WasPressedThisFrame())
        {
            RPC_RequestPlaceStone(boardX, boardY, team);
        }

        UpdateCursorVisual();

    }

    private void HandGridMovement()
    {
        Vector2 move = inputSystemActions.Player.Move.ReadValue<Vector2>();

        if(move.magnitude > 0.5f && !moveInputPressed)
        {
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
                boardX = Mathf.Clamp(boardX + (move.x > 0 ? 1 : -1), 0, boardSize - 1);
            else
                boardY = Mathf.Clamp(boardY + (move.y > 0 ? 1 : -1), 0, boardSize - 1);

            moveInputPressed = true;
        }

        else if(move.magnitude < 0.1f)
        {
            moveInputPressed = false;
        }
    }

    private void UpdateCursorVisual()
    {
        if (currentStone != null)
            currentStone.transform.localPosition = GetWorldPosition(boardX, boardY);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(startPos + (x * spacingOffset), 0.05f, startPos + (y * spacingOffset));
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestPlaceStone(int x, int y, int playerTeam)
    {
        //Check Turn
        if (currentTurn != playerTeam)
            return;

        int index = (y * boardSize) + x;
        if (BoardArray[index].IsPlaced != 0)
            return;

        //place stone
        var data = BoardArray[index];
        data.IsPlaced = playerTeam;
        BoardArray.Set(index, data);
    }

    public void PlaceStone(int x, int y, int team)
    {
        int index = (y * 20) + x; // 2차원 좌표를 1차원 인덱스로 변환
        var data = BoardArray[index];
        data.IsPlaced = team;
        BoardArray.Set(index, data); // 값을 변경한 후 Set을 호출해야 동기화됨

        //spawn stone
        Runner.Spawn(team == 1 ? blackStonePrefab : whiteStonePrefab, GetWorldPosition(x, y), Quaternion.identity);

        if(CheckWin(x, y, team))
        {
            Debug.Log($"Team {team} Winds");
            isGameOver = true;
            return;
        }

        //turn
        currentTurn = (team == 1) ? 2 : 1;
    }

    private bool CheckWin(int x, int y, int team)
    {
        //4방향(가로, 세로, 대각선\, 대각선/)
        int[,] dirs = { { 1, 0 }, { 0, 1 }, { 1, 1 }, { 1, -1 } };

        for (int i = 0; i < 4; i++)
        {
            int count = 1;
            //양방향 탐색
            count += CountInDirection(x, y, dirs[i, 0], dirs[i, 1], team);
            count += CountInDirection(x, y, -dirs[i, 0], -dirs[i, 1], team);

            if (count >= 5) return true;
        }
        return false;
    }

    private int CountInDirection(int x, int y, int dx, int dy, int team)
    {
        int count = 0;
        int nx = x + dx;
        int ny = y + dy;

        while(nx > 0 && nx < boardSize && ny > 0 && ny < boardSize)
        {
            if (BoardArray[(ny * boardSize) + nx].IsPlaced == team)
            {
                count++;
                nx += dx;
                ny += dy;
            }
            else break;
        }
        return count;
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // 팀 할당 로직 (예: 먼저 들어오면 Black)
            // 여기서는 단순화를 위해 인스펙터에서 설정하거나 별도 로직 필요
            Runner.Spawn(playerPrefab);
        }
    }

}