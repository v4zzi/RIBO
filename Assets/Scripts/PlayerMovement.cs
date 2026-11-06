using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ==========================================
    // 1. COMPONENTES Y VARIABLES DE REFERENCIA
    // ==========================================
    private Rigidbody2D rb;

    [Header("Movimiento Horizontal")]
    [Tooltip("Velocidad de desplazamiento lateral del personaje.")]
    public float moveSpeed = 8f;

    [Header("Configuración de Salto y Gravedad")]
    [Tooltip("Fuerza inicial aplicada hacia arriba al saltar.")]
    public float jumpForce = 12f;
    [Tooltip("Gravedad personalizada aplicada por código en lugar de la nativa de Unity.")]
    public float customGravity = 35f;
    [Tooltip("Multiplicador de gravedad al caer para que el descenso sea más rápido y con peso.")]
    public float fallMultiplier = 2.5f;

    [Header("Coyote Time y Jump Buffer (Sensibilidad de Salto)")]
    [Tooltip("Tiempo de gracia (en segundos) para saltar justo después de salir de una plataforma.")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Tooltip("Tiempo de gracia (en segundos) para registrar un salto antes de tocar el suelo.")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Detección de Suelo")]
    [Tooltip("Objeto vacío posicionado en los pies del jugador para comprobar colisión.")]
    public Transform groundCheck;
    [Tooltip("Radio del círculo de detección de suelo.")]
    public float checkRadius = 0.2f;
    [Tooltip("Capa asignada para identificar las superficies del piso.")]
    public LayerMask groundLayer;
    private bool isGrounded;

    // Velocidad vertical interna calculada por el script
    private float verticalVelocity;

    // ==========================================
    // 2. INICIALIZACIÓN
    // ==========================================
    void Start()
    {
        // Obtenemos el componente Rigidbody2D del objeto
        rb = GetComponent<Rigidbody2D>();

        // Desactivamos la gravedad nativa del motor para controlarla totalmente desde el script
        rb.gravityScale = 0f;
    }

    // ==========================================
    // 3. CICLO PRINCIPAL (UPDATE)
    // ==========================================
    void Update()
    {
        // A. Procesar Movimiento Horizontal
        float moveInput = Input.GetAxisRaw("Horizontal");
        float horizontalVelocity = moveInput * moveSpeed;

        // B. Comprobar Detección de Suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // C. Actualizar Temporizador de Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Se reinicia mientras esté pisando el suelo
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Cuenta regresiva al estar en el aire
        }

        // D. Actualizar Temporizador de Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime; // Registra la intención de saltar
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // E. Ejecución del Salto (Si hay Coyote Time y Jump Buffer activos)
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            verticalVelocity = jumpForce;
            coyoteTimeCounter = 0f; // Consumimos el coyote time
            jumpBufferCounter = 0f; // Consumimos el búfer para evitar saltos dobles
        }

        // F. Cálculo y Aplicación de Gravedad Personalizada
        if (isGrounded && verticalVelocity <= 0f)
        {
            verticalVelocity = -2f; // Fuerza constante ligera para mantener al jugador pegado al suelo
        }
        else
        {
            // Seleccionamos la gravedad base o aplicamos el multiplicador de caída rápida
            float currentGravity = customGravity;
            if (verticalVelocity < 0)
            {
                currentGravity *= fallMultiplier;
            }

            verticalVelocity -= currentGravity * Time.deltaTime;
        }

        // G. Aplicar velocidad final al Rigidbody2D (Optimizado para Unity 6 con linearVelocity)
        rb.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    // ==========================================
    // 4. MÉTODOS DE APOYO Y DEBUG VISUAL
    // ==========================================
    // Dibuja una esfera roja en la escena (Editor de Unity) para verificar el área de los pies
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}