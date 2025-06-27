using UnityEngine;
using Mirror;
using Unity.Mathematics;

public class FireScript : NetworkBehaviour
{
    [SerializeField] GameObject gameCamera = null;
    [SerializeField] LayerMask playerMask = new LayerMask();
    [SerializeField] GameObject damageTextParent = null;

    private float lastShotTime = 0f;
    private float waitForSecondsBetweenShots = 0.2f;
    void Update()
    {
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
                        if (playerHealthScript.GetHealth() <= 0) return;

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
}
