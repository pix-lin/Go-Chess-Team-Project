using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // 접속 시도
        PhotonNetwork.AutomaticallySyncScene = true; // 마스터 클라이언트 씬 따라감
    }

    public override void OnConnected() // 접속됐을 경우 Callback
    {
        base.OnConnected();
        Debug.Log($"연결");
    }

    public override void OnConnectedToMaster() // 마스터 서버 접속 시 Callback
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터연결");
        PhotonNetwork.JoinLobby(); // 로비 들어가기
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 성공적으로 입장했습니다.");
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        
        // MyRoom이라는 Room만 생성되므로 여러 매칭은 동시에 불가능함
        // 테스트 단계에서만
        PhotonNetwork.JoinOrCreateRoom("MyRoom", options, TypedLobby.Default);

        // 빈 방 자동으로 찾아주고 없으면 만들어 줌
        // 나중에 랜덤 방 필요 시
        // PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: options);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장. 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // 마스터 클라이언트만 씬 로드 호출
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("PhotonGameTest"); // 씬이름 수정 예정
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // 더 이상 입장 못 하게
            PhotonNetwork.LoadLevel("PhotonGameTest"); // 씬이름 수정 예정
        }
    }
}