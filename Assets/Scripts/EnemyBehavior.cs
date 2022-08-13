using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] float enemyMovementSpeed = 1.0f;
    Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, enemyMovementSpeed * Time.deltaTime);
    }
}