using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks; // Needed for Task handling if using async
using Yarn.Unity; // Added Yarn Spinner namespace

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundary;
    CinemachineConfiner confiner;
    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPosition;
    [SerializeField] float additivePos = 4f;

    enum Direction { Up, Down, Left, Right, Teleport }

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.gameObject);
        }
    }

    public async Task FadeTransition(GameObject player)
    {
        if (ScreenFader.instance != null)
        {
            await ScreenFader.instance.FadeOut();
        }

        if (confiner != null && mapBoundary != null)
        {
            confiner.m_BoundingShape2D = mapBoundary;
        }

        UpdatePlayerPosition(player);

        if (ScreenFader.instance != null)
        {
            await ScreenFader.instance.FadeIn();
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if (direction == Direction.Teleport)
        {
            if (teleportTargetPosition != null)
            {
                player.transform.position = teleportTargetPosition.position;
            }
            return;
        }

        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += additivePos;
                break;
            case Direction.Down:
                newPos.y -= additivePos;
                break;
            case Direction.Right:
                newPos.x += additivePos;
                break;
            case Direction.Left:
                newPos.x -= additivePos;
                break;
        }

        player.transform.position = newPos;
    }

    // --- YARN SPINNER COMMAND ---
    // Usage in Yarn: <<teleport_player Mansion_Entrance>>
    [YarnCommand("teleport_player")]
    public static async Task TeleportPlayerCommand(string transitionObjectName)
    {
        // Find the specific MapTransition object in the scene by name
        GameObject transitionObj = GameObject.Find(transitionObjectName);
        GameObject player = GameObject.FindWithTag("Player");

        if (transitionObj != null && player != null)
        {
            MapTransition mapTransition = transitionObj.GetComponent<MapTransition>();
            if (mapTransition != null)
            {
                await mapTransition.FadeTransition(player);
            }
        }
        else
        {
            Debug.LogWarning($"[MapTransition] Could not find transition object named '{transitionObjectName}' or Player object.");
        }
    }
}