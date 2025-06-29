using UnityEngine;
using Mirror;
using Unity.Mathematics;
using TMPro;

public class FireScript : NetworkBehaviour
{
    [SerializeField] GameObject gameCamera = null;
    [SerializeField] LayerMask playerMask = new LayerMask();
    [SerializeField] GameObject damageTextParent = null;
    [SerializeField] HealthScript healthScript = null;
    private float lastShotTime = 0f;
    private float waitForSecondsBetweenShots = 0.2f;

    [SerializeField] GameObject roundOverPanel = null;
    [SerializeField] TMP_Text winnerText = null;

    
    void Update()
    {
        if (!isLocalPlayer) { return; }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (lastShotTime == 0 ||
            lastShotTime + waitForSecondsBetweenShots < Time.time)
            {
                lastShotTime = Time.time;
                if (Physics.Raycast(gameCamera.transform.position,
                gameCamera.transform.forward, out RaycastHit hit, playerMask))
                {
                    if (hit.collider.TryGetComponent<HealthScript>
                    (out HealthScript playerHealthScript))
                    {
                        if (playerHealthScript.GetHealth() - 25 <= 0)
                        {
                            roundOverPanel.SetActive(true);
                            winnerText.text = "You win";
                            RoundOver();
                        }
                        if (playerHealthScript.GetHealth() <= 0)
                        {
                            return;
                        }

                        GameObject newTextParent = Instantiate(damageTextParent, hit.point, Quaternion.identity);
                        newTextParent.GetComponentInChildren<DamageTextScript>().GetCalled(25, gameCamera);

                        if (isServer)
                        {
                            ServerHit(25, playerHealthScript);
                            return;
                        }
                        CmdHit(25, playerHealthScript);
                    }
                }
            }

        }
    }

    [Command]
    private void CmdHit(float damage, HealthScript playerHealthScript)
    {
        ServerHit(damage, playerHealthScript);
    }

    [Server]
    private void ServerHit(float damage, HealthScript playerHealthScript)
    {
        playerHealthScript.GetDamage(damage);
    }

    void RoundOver()
    {
        Invoke(nameof(BeginNewRound), 5f);
    }
    void BeginNewRound()
    {
        healthScript.BeginNewRound();
    }
}
