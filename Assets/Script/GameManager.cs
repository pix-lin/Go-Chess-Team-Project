using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour, IPlayerJoined
{
    [Networked] public int PlayerCount { get; set; } = 0;

    [Header("Settings")]
    public int boardSize = 20;
    public float startPos = 0.2465f; //왼쪽 아래(원점) 좌표
    public float spacingOffset = 0.0275f;
    //시작(-0.2465, 0, -0.2465) +-0.0275

    public GameObject playerPrefab;
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;

    [Header("State")]
    [Networked] public int currentTurn { get; set; } = 1; //Black(1), White(2)
    [Networked] public bool isGameOver { get; set; }
    [Networked] public int StonesThisTurn { get; set; } = 0; // 이번 턴에 놓은 돌 수
    [Networked] public bool FirstTurnDone { get; set; } = false; // 흑의 첫 턴(1개) 완료 여부

    public struct GoChessBoard : INetworkStruct
    {
        public int IsPlaced; //0: 빈칸, 1: Black, 2: White
        public Vector3 Position;
    }

    [Networked, Capacity(400)] //20 x 20
    public NetworkArray<GoChessBoard> BoardArray => default;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            currentTurn = 1;
            StonesThisTurn = 0;
            FirstTurnDone = false;
        }
    }

    // Player 입장시 팀 분배 문제 해결
    public void PlayerJoined(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if (PlayerCount >= 2) return; // 최대 2인
            PlayerCount++;
            var spawned = Runner.Spawn(playerPrefab, inputAuthority: player);
            spawned.GetComponent<PlayerController>().team = PlayerCount;
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(startPos + (x * spacingOffset), 0.05f, startPos + (y * spacingOffset));
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestPlaceStone(int x, int y, int playerTeam)
    {
        if (currentTurn != playerTeam)
            return;

        int index = (y * boardSize) + x;
        if (BoardArray[index].IsPlaced != 0)
            return;

        PlaceStone(x, y, playerTeam);
    }

    public void PlaceStone(int x, int y, int team)
    {
        int index = (y * boardSize) + x;
        var data = BoardArray[index];
        data.IsPlaced = team;
        BoardArray.Set(index, data);

        Runner.Spawn(team == 1 ? blackStonePrefab : whiteStonePrefab, GetWorldPosition(x, y), Quaternion.identity);

        if (CheckWin(x, y, team))
        {
            Debug.Log($"Team {team} Wins");
            isGameOver = true;
            return;
        }

        // 육목 턴 규칙: 흑의 첫 턴은 1개, 이후 모든 턴은 2개
        bool isFirstBlackTurn = team == 1 && !FirstTurnDone;
        int stonesRequired = isFirstBlackTurn ? 1 : 2;

        StonesThisTurn++;

        if (StonesThisTurn >= stonesRequired)
        {
            StonesThisTurn = 0;
            if (isFirstBlackTurn) FirstTurnDone = true;
            currentTurn = team == 1 ? 2 : 1;
        }
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

            if (count >= 6) return true;
        }
        return false;
    }

    private int CountInDirection(int x, int y, int dx, int dy, int team)
    {
        int count = 0;
        int nx = x + dx;
        int ny = y + dy;

        while (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize) // nx, ny 0 포함으로 수정 
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
}
