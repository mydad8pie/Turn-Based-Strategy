using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    private GameObject healthBarBackground;
    private GameObject healthBarFill;
    private GameObject healthBarText;

    private float barWidth = 2f;
    private float barHeight = 0.3f;
    private float barYOffset = 2f;

    void Start()
    {
        currentHealth = maxHealth;
        CreateHealthBar();
        UpdateHealthBar();
    }

    void CreateHealthBar()
    {
        // Create background (red bar)
        healthBarBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        healthBarBackground.name = "HealthBarBackground";
        healthBarBackground.transform.SetParent(transform);
        healthBarBackground.transform.localPosition = new Vector3(0, barYOffset, 0);
        healthBarBackground.transform.localScale = new Vector3(barWidth, barHeight, 0.1f);
        healthBarBackground.GetComponent<Renderer>().material.color = Color.red;

        // Remove collider so it doesnt interfere with raycasts
        Destroy(healthBarBackground.GetComponent<Collider>());

        // Create fill (green bar)
        healthBarFill = GameObject.CreatePrimitive(PrimitiveType.Cube);
        healthBarFill.name = "HealthBarFill";
        healthBarFill.transform.SetParent(transform);
        healthBarFill.transform.localPosition = new Vector3(0, barYOffset + 0.01f, 0);
        healthBarFill.transform.localScale = new Vector3(barWidth, barHeight, 0.1f);
        healthBarFill.GetComponent<Renderer>().material.color = Color.green;

        // Remove collider
        Destroy(healthBarFill.GetComponent<Collider>());

        // Create text object for health numbers
        healthBarText = new GameObject("HealthBarText");
        healthBarText.transform.SetParent(transform);
        healthBarText.transform.localPosition = new Vector3(0, barYOffset + 0.4f, 0);

        // Add TextMesh for the number display
        TextMesh textMesh = healthBarText.AddComponent<TextMesh>();
        textMesh.text = currentHealth + "/" + maxHealth;
        textMesh.fontSize = 35;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.characterSize = 0.6f;
    }

    void LateUpdate()
    {
        // Make health bar always face the camera
        if (Camera.main != null)
        {
            healthBarBackground.transform.rotation = Camera.main.transform.rotation;
            healthBarFill.transform.rotation = Camera.main.transform.rotation;
            healthBarText.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        Debug.Log(gameObject.name + " took " + damage + " damage. Health: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // Scale the green fill bar based on current health percentage
            float healthPercent = (float)currentHealth / maxHealth;
            healthBarFill.transform.localScale = new Vector3(
                barWidth * healthPercent,
                barHeight,
                0.1f
            );

            // Keep the fill bar anchored to the left side
            healthBarFill.transform.localPosition = new Vector3(
                -(barWidth / 2) * (1 - healthPercent),
                barYOffset + 0.01f,
                0
            );
        }

        if (healthBarText != null)
        {
            TextMesh textMesh = healthBarText.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = currentHealth + "/" + maxHealth;
            }
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }
}