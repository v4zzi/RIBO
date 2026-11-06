using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movimiento Horizontal")]
    public float moveSpeed = 8f;

    [Header("Configuración de Salto y Gravedad")]
    public float jumpForce = 12f;          // Fuerza inicial del salto
    public float customGravity = 35f;      // Gravedad propia aplicada en el script
    public float fallMultiplier = 2.5f;    // Multiplicador para que la caída sea más rápida y precisa

    [Header("Coyote Time y Jump Buffer")]
    public float coyoteTime = 0.15f;       // Tiempo de gracia al salir de una plataforma
    private float coyoteTimeCounter;

    public float jumpBufferTime = 0.15f;   // Tiempo de gracia antes de tocar el suelo para registrar el salto
    private float jumpBufferCounter;

    [Header("Detección de Suelo")]
    public Transform groundCheck;          // Objeto vacío posicionado en los pies del jugador
    public float checkRadius = 0.2f;       // Radio del círculo de detección
    public LayerMask groundLayer;          // Capa asignada al piso
    private bool isGrounded;

    private float verticalVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Desactivamos la gravedad nativa del Rigidbody2D para controlarla por completo en el script
        rb.gravityScale = 0f;
    }

    void Update()
    {
        // 1. Movimiento Horizontal (A/D o Flechas)
        float moveInput = Input.GetAxisRaw("Horizontal");
        float horizontalVelocity = moveInput * moveSpeed;

        // 2. Detección de Suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 3. Actualizar Coyote Time Counter
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // 4. Actualizar Jump Buffer Counter (Registrar si presionas saltar antes de tocar el suelo)
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 5. Lógica de Salto (Se ejecuta si hay Coyote Time y Jump Buffer activos al mismo tiempo)
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            verticalVelocity = jumpForce;
            coyoteTimeCounter = 0f; // Consumimos el coyote time
            jumpBufferCounter = 0f; // Consumimos el búfer para evitar saltos repetidos
        }

        // 6. Aplicación de Gravedad Personalizada
        if (isGrounded && verticalVelocity <= 0f)
        {
            verticalVelocity = -2f; // Pequeña fuerza constante hacia abajo para mantener al jugador pegado al suelo
        }
        else
        {
            // Si el jugador está cayendo, aplicamos la gravedad multiplicada para que se sienta con peso
            float currentGravity = customGravity;
            if (verticalVelocity < 0)
            {
                currentGravity *= fallMultiplier;
            }

            verticalVelocity -= currentGravity * Time.deltaTime;
        }

        // 7. Aplicar la velocidad resultante al Rigidbody2D (Usando linearVelocity para Unity 6)
        rb.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    // Dibuja una esfera roja en la escena para ver el punto de detección de suelo
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}