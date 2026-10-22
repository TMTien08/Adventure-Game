using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public float speed = 3;
    Rigidbody2D rb;
    Vector2 move;

    //Các biến số liên quan đến hệ thống y tế
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    private int currentHealth;

    //Các biến số liên quan đến khả năng bất khả xâm phạm tạm thời
    public float timeInvincible = 2f;
    private bool isInvincible;
    private float damageCooldown;

    //Các biến liên quan đến animation
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public InputAction LaunchAction;

    public InputAction TalkAction;

    NonPlayerCharacter lastNonPlayerCharacter;

    private AudioSource audioSource;
    public AudioClip playerHitClip;
    public AudioClip launch;

    public ParticleSystem pickUpParticleEffect;
    public ParticleSystem hitParticleEffect;

    public event Action OnTalkedToNPC;
    void Start()
    {
        MoveAction.Enable();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        LaunchAction.Enable();
        TalkAction.Enable();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
        if (LaunchAction.WasPressedThisFrame())
        {
            Launch();
        }
        RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, moveDirection, 1.4f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NonPlayerCharacter NPC = hit.collider.GetComponent<NonPlayerCharacter>();
            NPC.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = NPC;
            FindFriend();
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
            }
        }
    }
    void FixedUpdate()
    {
        Vector2 position = (Vector2)rb.position + move * speed * Time.deltaTime;
        rb.MovePosition(position);
    }
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            Instantiate(hitParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            PlaySound(playerHitClip);
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }
        else
        {
            Instantiate(pickUpParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        
    }
    void Launch()
    {
        PlaySound(launch);
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
    }
    void FindFriend()
    {
        if (TalkAction.WasPressedThisFrame())
        {
            OnTalkedToNPC?.Invoke();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
}
