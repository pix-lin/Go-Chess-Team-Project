using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Fusion;
using Unity.VisualScripting;

// 플레이어 개별 컨트롤러
// 각 플레이어마다 하나씩 스폰되며, 자기 캐릭터의 입력 처리 및 커서 시각화를 담당
// 게임 전역 상태(보드, 턴 등)는 GameManager에서 관리하므로 여기서는 다루지 않음
public class PlayerController : NetworkBehaviour
{
    public bool keySwitched; 

    [Networked] public int team { get; set; } //1: Black / 2: White
                                               // 이 플레이어의 팀 번호 (1: 흑돌 / 2: 백돌)
                                               // [Networked]로 선언되어 모든 피어에 자동 동기화됨
                                               // GameManager가 스폰 직후 직접 할당함

    public GameObject currentStone; // 커서 역할을 하는 돌 오브젝트 (보드 위에서 위치를 미리 보여주는 용도)
    private GameManager gameManager;
    private InputSystem_Actions inputSystemActions;
    private int boardX = 9, boardY = 9; //center of board(20x20)
    private bool moveInputPressed; // 한 번 누르면 한 칸만 이동하도록, 연속 발동 방지용

    // 네트워크 오브젝트가 스폰될 때 호출됨 (Unity의 Start와 비슷한 역할)
    public override void Spawned()
    {
        // 씬에 있는 GameManager를 찾아서 참조 저장
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogError("GameManager not found in scene!"); // 씬에 GameManager가 없으면 에러 출력
    }

    // 입력 시스템 초기화 및 활성화
    public void OnEnable()
    {
        if (inputSystemActions == null)
            inputSystemActions = new InputSystem_Actions(); // 입력 액션 객체가 없으면 새로 생성

        inputSystemActions.Player.Enable(); // 플레이어 입력 맵 활성화
    }

    // 입력 시스템 정리
    public void OnDisable()
    {
        inputSystemActions.Player.Disable(); // 플레이어 입력 맵 비활성화
        inputSystemActions.UI.Disable();     // UI 입력 맵 비활성화
    }

    private void Update()
    {
        // 입력 권한이 있는 클라이언트(=내 캐릭터)만 입력을 처리
        // Object가 유효하지 않거나, 내 캐릭터가 아니면 아무것도 하지 않음
        if (Object == null || !Object.IsValid || !Object.HasInputAuthority) return;

        // GameManager가 없거나 게임이 끝난 상태면 입력 무시
        if (gameManager == null || gameManager.isGameOver) return;

        // 입력 액션이 초기화 안 되어 있으면 무시
        if (inputSystemActions == null) return;

        HandGridMovement(); // 방향 입력으로 커서 이동 처리

        // Jump 키(돌 놓기 키)가 이번 프레임에 눌렸으면
        if (inputSystemActions.Player.Jump.WasPressedThisFrame())
        {
            // GameManager에게 돌 배치 요청 RPC를 보냄
            // 실제 검증과 처리는 StateAuthority(=호스트)의 GameManager에서 이루어짐
            gameManager.RPC_RequestPlaceStone(boardX, boardY, team);
        }

        UpdateCursorVisual(); // 커서 위치를 보드 좌표에 맞게 시각적으로 업데이트
    }

    // 방향 입력을 받아서 커서를 한 칸씩 이동시키는 함수
    private void HandGridMovement()
    {
        // 현재 입력된 이동 벡터 (스틱/WASD/방향키)
        Vector2 move = inputSystemActions.Player.Move.ReadValue<Vector2>();
        int boardSize = gameManager.boardSize; // GameManager에서 보드 크기 가져옴

        // 입력 강도가 임계값(0.5)을 넘었고, 아직 처리되지 않은 입력이면
        if (move.magnitude > 0.5f && !moveInputPressed)
        {
            // x축 입력이 더 크면 좌우 이동, 아니면 상하 이동
            // (대각선 입력 시 더 큰 축 하나만 처리하여 정확히 한 칸씩 이동)
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
                // x가 양수면 +1, 음수면 -1, 보드 범위(0 ~ boardSize-1)로 제한
                boardX = Mathf.Clamp(boardX + (move.x > 0 ? 1 : -1), 0, boardSize - 1);
            else
                // y축도 동일한 방식
                boardY = Mathf.Clamp(boardY + (move.y > 0 ? 1 : -1), 0, boardSize - 1);

            moveInputPressed = true; // 처리됐다고 표시 (다음 입력 전까지 또 처리되지 않게)
        }
        // 입력이 거의 없을 때(스틱이 중앙으로 돌아왔을 때) 플래그 해제
        // -> 다음 입력을 받을 준비
        else if (move.magnitude < 0.1f)
        {
            moveInputPressed = false;
        }
    }

    // 커서(currentStone)의 시각적 위치를 현재 boardX, boardY에 맞게 갱신
    private void UpdateCursorVisual()
    {
        if (currentStone != null)
            // GameManager의 좌표 변환 함수를 통해 보드 좌표 -> 월드 좌표 변환
            currentStone.transform.localPosition = gameManager.GetWorldPosition(boardX, boardY);
    }
}