using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System.Collections;

public class PlayerController : MonoBehaviourPun
{
    [Header("Cursor")]
    public GameObject cursorStone;

    private InputSystem_Actions inputSystemActions;
    private int boardX = 9, boardY = 9;
    private bool moveInputPressed;
    private int myTeam;
    private bool isReady = false;

    IEnumerator Start()
    {
        Debug.Log($"[PC] Start 호출. IsMine={photonView.IsMine}");

        if (!photonView.IsMine)
        {
            if (cursorStone != null) cursorStone.SetActive(false);
            enabled = false;
            yield break;
        }

        // GameManager가 씬에 등장할 때까지 대기
        while (GameManager.Instance == null)
            yield return null;

        // 팀 분배가 끝날 때까지 대기 (CustomProperties 업데이트 기다림)
        while (GameManager.Instance.GetMyTeam() == 0)
            yield return null;

        myTeam = GameManager.Instance.GetMyTeam();
        isReady = true;
        Debug.Log($"[PC] 준비 완료. myTeam = {myTeam}");
    }

    void OnEnable()
    {
        if (inputSystemActions == null)
            inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.Enable();
    }

    void OnDisable()
    {
        if (inputSystemActions != null)
            inputSystemActions.Player.Disable();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (!isReady) return; // ← 팀 정해지기 전엔 입력 무시
        if (GameManager.Instance.isGameOver) return;

        HandleGridMovement();
        UpdateCursorVisual();

        if (inputSystemActions.Player.Jump.WasPressedThisFrame())
        {
            Debug.Log($"[PC] Jump 눌림. currentTurn={GameManager.Instance.currentTurn}, myTeam={myTeam}");

            if (GameManager.Instance.currentTurn != myTeam)
            {
                Debug.Log("[PC] 내 턴이 아님");
                return;
            }
            if (!GameManager.Instance.IsCellEmpty(boardX, boardY))
            {
                Debug.Log("[PC] 이미 돌이 놓인 자리");
                return;
            }

            Debug.Log($"[PC] 마스터에 RPC 요청: ({boardX}, {boardY})");
            GameManager.Instance.photonView.RPC(
                "RPC_RequestPlaceStone",
                RpcTarget.MasterClient,
                boardX, boardY, myTeam
            );
        }
    }

    private void HandleGridMovement()
    {
        Vector2 move = inputSystemActions.Player.Move.ReadValue<Vector2>();
        int boardSize = GameManager.Instance.boardSize;

        if (move.magnitude > 0.5f && !moveInputPressed)
        {
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
                boardX = Mathf.Clamp(boardX + (move.x > 0 ? 1 : -1), 0, boardSize - 1);
            else
                boardY = Mathf.Clamp(boardY + (move.y > 0 ? 1 : -1), 0, boardSize - 1);

            moveInputPressed = true;
        }
        else if (move.magnitude < 0.1f)
        {
            moveInputPressed = false;
        }
    }

    private void UpdateCursorVisual()
    {
        if (cursorStone != null)
            cursorStone.transform.position =  // localPosition → position
                GameManager.Instance.GetWorldPosition(boardX, boardY);
    }
}