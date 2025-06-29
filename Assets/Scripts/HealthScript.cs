using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class HealthScript : NetworkBehaviour
{
    [SyncVar(hook = nameof(HealthValueChange))] private float healthValue = 100f;
    [SerializeField] TMP_Text healthTxt = null;
    [SerializeField] Slider healthSlider = null;

    [SerializeField] Animator tpsAnimator = null;
    [SerializeField] GameObject mainFpsCamera = null, afterDeathCamera = null, tpsModelMesh = null;
    [SerializeField] Movement movementScript = null;
    [SerializeField] CharacterController controller = null;

    Vector3 startPosition = Vector3.zero;
    void Start()
    {
        if (!isLocalPlayer) return;

        startPosition = transform.position;
        healthTxt.text = healthValue.ToString();
        healthSlider.value = healthValue;
    }
    public void NewRoundCall()
    {
        CmdMaxHealth();
    }
    [Command]
    void CmdMaxHealth()
    {
        ServerMaxHealth();
    }

    [Server]
    void ServerMaxHealth()
    {
        healthValue = 100f;
    }
    [Server]
    public void GetDamage(float damage)
    {
        healthValue = Mathf.Max(0f, healthValue - damage);
    }
    void HealthValueChange(float oldHealth, float newHealth)
    {
        if (!isLocalPlayer) { return; }

        healthTxt.text = healthValue.ToString();
        healthSlider.value = healthValue;
        if (newHealth <= 0)
        {
            print("die");

            afterDeathCamera.SetActive(true);
            mainFpsCamera.SetActive(false);
            tpsModelMesh.SetActive(true);
            
            tpsAnimator.SetBool("isWalking", false);
            tpsAnimator.SetBool("dead", true);

            Invoke(nameof(BeginNewRound), 5f);
        }
    }
    public float GetHealth()
    {
        return healthValue;
    }
    public void BeginNewRound()
    {
        NewRoundCall();
        
        movementScript.enabled = false;
        controller.enabled = false;

        transform.position = startPosition;

        mainFpsCamera.SetActive(true);
        afterDeathCamera.SetActive(false);

        tpsModelMesh.SetActive(false);

        tpsAnimator.SetBool("isWalking", false);
        tpsAnimator.SetBool("dead", false);  

        movementScript.enabled = true;
        controller.enabled = true;
    }
}
