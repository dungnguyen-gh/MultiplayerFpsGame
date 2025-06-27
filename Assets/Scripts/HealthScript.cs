using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class HealthScript : NetworkBehaviour
{
    [SyncVar(hook = nameof(HealthValueChange))] private float healthValue = 100f;
    [SerializeField] TMP_Text healthTxt = null;
    [SerializeField] Slider healthSlider = null;
    void Start()
    {
        if (!isLocalPlayer) return;
        healthTxt.text = healthValue.ToString();
        healthSlider.value = healthValue;
    }

    [Server]
    public void GetDamage(float damage)
    {
        healthValue = Mathf.Max(0f, healthValue - damage);
    }
    void HealthValueChange(float oldHealth, float newHealth)
    {
        healthTxt.text = healthValue.ToString();
        healthSlider.value = healthValue;
        if (newHealth <= 0)
        {
            print("die");
        }
    }
    public float GetHealth()
    {
        return healthValue;
    }
}
