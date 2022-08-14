using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] Transform spriteHoldingChild;
    [SerializeField] float mirrorTimer;
    [SerializeField] float enemyMovementSpeed;
    [SerializeField] float floatingSpeed;
    [SerializeField] float floatingAmount;
    [SerializeField] Sprite enemySprite1;
    [SerializeField] Sprite enemySprite2;
    [SerializeField] Sprite enemySprite3;
    [SerializeField] Sprite cauldronEnemySprite;
    [SerializeField] Sprite powerupSprite;
    [SerializeField] int cauldronSpawnChancePercentage = 10;
    [SerializeField] int powerupSpawnChancePercentage = 25;
    [SerializeField] float cauldronEnemyKnockbackForce = 10f;
    [SerializeField] float swordKnockbackForce = 10f;
    [SerializeField] float killWeaponContribution = 7.5f;

    private SpriteRenderer spriteRenderer;
    private Transform player;
    private int spriteRandomizer = 0;
    private int mirrorRandomizer = 0;
    private int cauldronRandomizer = 0;
    private int powerupRandomizer = 0;
    private bool isPowerup = false;
    private bool isCauldron = false;
    private bool rigidbodyExists = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = spriteHoldingChild.gameObject.GetComponent<SpriteRenderer>();
        spriteRandomizer = Random.Range(0, 3);
        mirrorRandomizer = Random.Range(0, 2);
        cauldronRandomizer = Random.Range(0, cauldronSpawnChancePercentage);
        powerupRandomizer = Random.Range(0, powerupSpawnChancePercentage);
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    private void OnEnable()
    {
        RandomizeSprite();
        StartCoroutine(MirrorSpriteRendererWithDelay());
    }

    private void HandleMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, enemyMovementSpeed * Time.deltaTime);
    }

    private void HandleAnimation()
    {
        float floatingLimiter = Mathf.PingPong(Time.time * floatingSpeed, floatingAmount);
        spriteHoldingChild.localPosition = new Vector3(0, floatingLimiter, 0);
    }

    IEnumerator MirrorSpriteRendererWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(mirrorTimer);

            if (!spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    IEnumerator RemoveRigidbodyWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        //Change enemy to normal
        isCauldron = false;

        //Remove rigidbody
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        GetComponent<BoxCollider2D>().isTrigger = true;
        rigidbodyExists = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Weapon" && isCauldron && !isPowerup)
        {
            //Physics
            GetComponent<Rigidbody2D>().AddForceAtPosition(GetComponent<Rigidbody2D>().velocity * cauldronEnemyKnockbackForce, collision.transform.position);
            Rigidbody2D swordRb = collision.gameObject.GetComponent<Rigidbody2D>();
            swordRb.AddForceAtPosition(-swordRb.velocity * swordKnockbackForce, collision.transform.position);

            switch (spriteRandomizer)
            {
                case 0:
                    spriteRenderer.sprite = enemySprite1;
                    break;
                case 1:
                    spriteRenderer.sprite = enemySprite2;
                    break;
                case 2:
                    spriteRenderer.sprite = enemySprite3;
                    break;
            }

            StartCoroutine(RemoveRigidbodyWithDelay());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Normal Enemy
        if (collision.tag == "Weapon" && !isCauldron && !isPowerup)
        {
            GetComponent<PooledObject>().Finish();
            WeaponManager.killCounter = WeaponManager.killCounter + killWeaponContribution;
            return;
        }

        //HP Enemy
        else if (collision.tag == "Weapon" && !isCauldron && isPowerup)
        {
            spriteRenderer.sprite = powerupSprite;
            PlayerController.GainHP();
            isPowerup = false;
            GetComponent<PooledObject>().Finish();
            WeaponManager.killCounter = WeaponManager.killCounter + killWeaponContribution;
            return;
        }

        //Player Getting Hit
        if (collision.tag == "Player")
        {
            PlayerController.LoseHP();
            GetComponent<PooledObject>().Finish();
            return;
        }
    }

    private void RandomizeSprite()
    {
        switch (spriteRandomizer)
        {
            case 0:
                spriteRenderer.sprite = enemySprite1;
                break;
            case 1:
                spriteRenderer.sprite = enemySprite2;
                break;
            case 2:
                spriteRenderer.sprite = enemySprite3;
                break;
        }

        //Chance for the enemies to be shielded
        if (cauldronRandomizer == 0)
        {
            spriteRenderer.sprite = cauldronEnemySprite;
            isCauldron = true;

            //Add rigidbody to cauldron enemies to activate knockback effects
            if (!rigidbodyExists)
            {
                gameObject.AddComponent<Rigidbody2D>();
                GetComponent<Rigidbody2D>().freezeRotation = true;
                GetComponent<BoxCollider2D>().isTrigger = false;
                rigidbodyExists = true;
            }
        }
        //Chance for the enemies to be powerup provided
        else if (powerupRandomizer == 0)
        {
            spriteRenderer.sprite = powerupSprite;
            isPowerup = true;
        }

        switch (mirrorRandomizer)
        {
            case 0:
                spriteRenderer.flipX = true;
                break;
            case 1:
                spriteRenderer.flipX = false;
                break;
        }
    }
}
