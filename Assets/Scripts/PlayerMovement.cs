using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Player Stats")]
    public float movementSpeed = 5f;
    public int attackPower = 10;
    public int defense = 5;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Try to find Animator on this object first
        anim = GetComponent<Animator>();

        // If not found, search in children
        if (anim == null)
            anim = GetComponentInChildren<Animator>(true); // true = include inactive

        Debug.Log("Animator found: " + (anim != null ? anim.gameObject.name : "NULL"));
    }

    void Update()
    {
        if (anim == null)
        {
            Debug.LogError("Animator is NULL! Check your Toto object.");
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;

        bool isMoving = moveX != 0 || moveY != 0;
        anim.SetInteger("speed", isMoving ? 1 : 0);

        // Only change direction when moving so idle faces last direction
        if (isMoving)
        {
            if (moveY < 0) anim.SetInteger("direction", 0);      // S = Down
            else if (moveY > 0) anim.SetInteger("direction", 1); // W = Up
            else if (moveX > 0) anim.SetInteger("direction", 2); // D = Right
            else if (moveX < 0) anim.SetInteger("direction", 3); // A = Left
        }

        Debug.Log($"isMoving: {isMoving} | speed: {anim.GetInteger("speed")} | direction: {anim.GetInteger("direction")} | animObject: {anim.gameObject.name}");
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput * movementSpeed;
        }
    }

    /// <summary>
    /// Handles dynamic stat boosts when math equations are solved!
    /// </summary>
    public void ApplyCombatBoost(string operationType, int boostAmount)
    {
        operationType = operationType.ToLower();

        if (operationType == "attack")
        {
            attackPower += boostAmount;
            Debug.Log($"[BOOST] ATK increased by {boostAmount}! New ATK: {attackPower}");
        }
        else if (operationType == "defense")
        {
            defense += boostAmount;
            Debug.Log($"[BOOST] DEF increased by {boostAmount}! New DEF: {defense}");
        }
        else if (operationType == "movement" || operationType == "speed")
        {
            movementSpeed += (boostAmount * 0.1f);
            Debug.Log($"[BOOST] MOV increased by {boostAmount}! New Speed: {movementSpeed}");
        }
    }
}