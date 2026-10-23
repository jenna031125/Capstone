using UnityEngine;
using Yarn.Unity;

public class NPCManager : MonoBehaviour
{
    [Header("NPC References")]
    [SerializeField] private GameObject _butlerObject; // 집사 GameObject

    [Header("Teleport Waypoints")]
    [SerializeField] private Transform _livingRoomPoint; // 거실 위치 (현관 진입 후)
    [SerializeField] private Transform _bedroomPoint;   // 앨리스 방 위치
    [SerializeField] private Transform _hallwayPoint;   // 집사가 사라질 복도 위치

    private static NPCManager _instance;

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    // 1. 집사를 특정 위치로 이동시키는 Yarn 커맨드
    [YarnCommand("move_butler")]
    public static void MoveButler(string locationName)
    {
        if (_instance == null || _instance._butlerObject == null) return;

        if (locationName == "LivingRoom" && _instance._livingRoomPoint != null)
        {
            _instance._butlerObject.transform.position = _instance._livingRoomPoint.position;
        }
        else if (locationName == "Bedroom" && _instance._bedroomPoint != null)
        {
            _instance._butlerObject.transform.position = _instance._bedroomPoint.position;
        }
        else if (locationName == "Hallway" && _instance._hallwayPoint != null)
        {
            _instance._butlerObject.transform.position = _instance._hallwayPoint.position;
        }
    }
}