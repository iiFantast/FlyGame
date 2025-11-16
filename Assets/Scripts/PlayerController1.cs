using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController1 : MonoBehaviour
{
    public float thrustForce = 15f; // Сила тяги (ускорение)
    public float maxSpeed = 8f; // Максимальная скорость
    public float rotationSpeed = 150f;
    public float drag = 2f; // Скорость торможения (чем меньше, тем дольше скользит)
    public int maxHealth = 10;
    private int currentHealth;
    private float thrustInput;
    private float rotationInput;
    private Vector2 movement;
    private Rigidbody2D rb;

    public TextMeshProUGUI healthText;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    
    // Для респавна
    private Vector3 startPosition;
    private Quaternion startRotation;
    public float respawnDelay = 2f; // Задержка перед возрождением
    private bool isDead = false;

    // Компоненты для включения/выключения
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        
        // Сохраняем стартовую позицию
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Movement").Enable();
    }

    public void OnMove(InputValue value)
    {
        if (isDead) return; // Блокируем управление во время смерти
        
        Vector2 input = value.Get<Vector2>();
        thrustInput = input.y;
        rotationInput = input.x;
    }

    public void OnFire(InputValue value)
    {
        if (isDead) return; // Блокируем стрельбу во время смерти
        
        if (Time.time >= nextFireTime)
        {
            if (firePoint != null && bulletPrefab != null)
            {
                Instantiate(bulletPrefab, firePoint.position, transform.rotation);
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return; // Блокируем физику во время смерти
        
        // Поворот самолёта
        float rotation = -rotationInput * rotationSpeed * Time.fixedDeltaTime;
        rb.rotation += rotation;

        // Применяем силу для плавного ускорения
        if (thrustInput > 0)
        {
            Vector2 thrustDirection = transform.right * thrustInput;
            rb.AddForce(thrustDirection * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, drag * Time.fixedDeltaTime);
        }

        rb.angularVelocity = 0f;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hazard"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Не получаем урон, если уже мертвы
        
        currentHealth -= damage;
        UpdateHealthUI();
        Debug.Log("Player1 took damage! Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText !=null)
        {
            healthText.text = "Helath: + " + currentHealth;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player1 is dead!");
        
        // Уведомляем GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("Player1");
        }
        
        // Делаем игрока невидимым и неактивным
        spriteRenderer.enabled = false;
        playerCollider.enabled = false;
        rb.linearVelocity = Vector2.zero;
        thrustInput = 0;
        rotationInput = 0;
        
        // Запускаем респавн через задержку
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        Debug.Log("Player1 respawned!");
        
        // Восстанавливаем позицию и здоровье
        transform.position = startPosition;
        transform.rotation = startRotation;
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        // Включаем обратно компоненты
        spriteRenderer.enabled = true;
        playerCollider.enabled = true;
        isDead = false;
        
        // Сбрасываем скорость
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}