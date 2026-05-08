using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public int boardSize = 20;
    public float startPos = -0.2465f; // 왼쪽 아래(원점) 좌표
    public float spacingOffset = 0.0275f;

    [Header("Prefabs")]
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;

    [Header("State")]
    public int currentTurn = 1;       // 1: Black, 2: White
    public bool isGameOver = false;
    public int stonesThisTurn = 0;
    public bool firstTurnDone = false;

    // 보드 상태 (모든 클라이언트가 동일하게 유지)
    private int[,] board;             // 0: 빈칸, 1: Black, 2: White

    // 팀 분배 (ActorNumber → team)
    private const string TEAM_KEY = "team";

    void Awake()
    {
        Instance = this;
        board = new int[boardSize, boardSize];
    }

    void Start()
    {
        // 마스터만 팀 분배
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[GM] 마스터 - 팀 분배 시작");
            AssignTeams();
        }
    }

    private void AssignTeams()
    {
        int teamCounter = 1;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            Hashtable props = new Hashtable { { TEAM_KEY, teamCounter } };
            p.SetCustomProperties(props);
            teamCounter++;
        }
    }

    // 내 팀 가져오기 (어디서든 호출 가능)
    public int GetMyTeam()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(TEAM_KEY, out object t))
            return (int)t;
        return 0;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(startPos + (x * spacingOffset), 0.05f,
                           startPos + (y * spacingOffset));
    }

    public bool IsCellEmpty(int x, int y)
    {
        return board[x, y] == 0;
    }

    // ===== 클라이언트 → 마스터: 돌 놓기 요청 =====
    [PunRPC]
    public void RPC_RequestPlaceStone(int x, int y, int playerTeam)
    {
        // 마스터만 검증/처리
        if (!PhotonNetwork.IsMasterClient) return;
        if (isGameOver) return;
        if (currentTurn != playerTeam) return;
        if (board[x, y] != 0) return;

        // 검증 통과 → 모든 클라에 적용 RPC
        photonView.RPC(nameof(RPC_PlaceStone), RpcTarget.All, x, y, playerTeam);
    }

    // ===== 마스터 → 모두: 실제 돌 배치 =====
    [PunRPC]
    public void RPC_PlaceStone(int x, int y, int team)
    {
        board[x, y] = team;

        GameObject prefab = (team == 1) ? blackStonePrefab : whiteStonePrefab;
        Instantiate(prefab, GetWorldPosition(x, y), Quaternion.identity);

        // 마스터만 승패/턴 진행 결정
        if (!PhotonNetwork.IsMasterClient) return;

        if (CheckWin(x, y, team))
        {
            photonView.RPC(nameof(RPC_GameOver), RpcTarget.All, team);
            return;
        }

        // 턴 진행 로직
        bool isFirstBlackTurn = (team == 1 && !firstTurnDone);
        int stonesRequired = isFirstBlackTurn ? 1 : 2;

        int newStonesThisTurn = stonesThisTurn + 1;
        int newTurn = currentTurn;
        bool newFirstTurnDone = firstTurnDone;

        if (newStonesThisTurn >= stonesRequired)
        {
            newStonesThisTurn = 0;
            if (isFirstBlackTurn) newFirstTurnDone = true;
            newTurn = (team == 1) ? 2 : 1;
        }

        photonView.RPC(nameof(RPC_AdvanceTurn), RpcTarget.All,
                       newTurn, newStonesThisTurn, newFirstTurnDone);
    }

    [PunRPC]
    public void RPC_AdvanceTurn(int newTurn, int newStonesThisTurn, bool newFirstTurnDone)
    {
        currentTurn = newTurn;
        stonesThisTurn = newStonesThisTurn;
        firstTurnDone = newFirstTurnDone;
    }

    [PunRPC]
    public void RPC_GameOver(int winnerTeam)
    {
        isGameOver = true;
        Debug.Log($"Team {winnerTeam} Wins!");
        // TODO: 결과 UI 표시
    }

    // ===== 승리 판정 =====
    private bool CheckWin(int x, int y, int team)
    {
        int[,] dirs = { { 1, 0 }, { 0, 1 }, { 1, 1 }, { 1, -1 } };

        for (int i = 0; i < 4; i++)
        {
            int count = 1;
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

        while (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize)
        {
            if (board[nx, ny] == team)
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