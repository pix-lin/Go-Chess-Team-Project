using UnityEngine;
using Photon.Pun;

public class GamePlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    void Start()
    {
        // 이미 룸에 들어와 있으면 바로 스폰
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
        else
        {
            // 아직 룸에 안 들어왔으면 콜백 대기
            Debug.LogWarning("아직 룸에 입장하지 않음. 룸 입장 후 스폰됩니다.");
        }
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        int myTeam = GameManager.Instance.GetMyTeam();
        float z = (myTeam == 1) ? -1f : 1f;

        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, z), Quaternion.identity);
    }
}