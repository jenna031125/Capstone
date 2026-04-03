using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor.Experimental.GraphView;

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

    async void FadeTransition(GameObject player)
    {
        await ScreenFader.instance.FadeOut();
        
        confiner.m_BoundingShape2D = mapBoundary;
        UpdatePlayerPosition(player);

        await ScreenFader.instance.FadeIn();
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if(direction == Direction.Teleport)
        {
            player.transform.position = teleportTargetPosition.position;

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

}
