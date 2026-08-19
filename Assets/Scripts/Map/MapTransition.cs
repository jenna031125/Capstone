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

    // --- YARN SPINNER COMMAND WITH DELAY ---
    // Usage in Yarn: <<teleport_with_delay TransitionObjectName HoldDurationSeconds>>
    // Example: <<teleport_with_delay Mansion_Entrance_Transition 2.5>>
    [YarnCommand("teleport_with_delay")]
    public static async Task TeleportWithDelayCommand(string transitionObjectName, float holdBlackSeconds)
    {
        GameObject transitionObj = GameObject.Find(transitionObjectName);
        GameObject player = GameObject.FindWithTag("Player");

        if (transitionObj != null && player != null)
        {
            MapTransition mapTransition = transitionObj.GetComponent<MapTransition>();
            if (mapTransition != null)
            {
                // 1. Fade to Black
                if (ScreenFader.instance != null)
                {
                    await ScreenFader.instance.FadeOut();
                }

                // 2. Wait while screen is completely black (play SFX during this window!)
                int delayMilliseconds = Mathf.RoundToInt(holdBlackSeconds * 1000f);
                await Task.Delay(delayMilliseconds);

                // 3. Teleport & Update Camera Confiner behind the black screen
                if (mapTransition.confiner != null && mapTransition.mapBoundary != null)
                {
                    mapTransition.confiner.m_BoundingShape2D = mapTransition.mapBoundary;
                }
                mapTransition.UpdatePlayerPosition(player);

                // 4. Fade back in
                if (ScreenFader.instance != null)
                {
                    await ScreenFader.instance.FadeIn();
                }
            }
        }
        else
        {
            Debug.LogWarning($"[MapTransition] Could not find transition object '{transitionObjectName}' or Player.");
        }
    }
}