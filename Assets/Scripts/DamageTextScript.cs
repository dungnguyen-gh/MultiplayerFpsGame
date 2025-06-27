using UnityEngine;
using TMPro;

public class DamageTextScript : MonoBehaviour
{
    GameObject cameraGo;
    void DestroyText()
    {
        Destroy(gameObject);
    }
    public void GetCalled(float damage, GameObject camera)
    {
        GetComponent<TMP_Text>().text = damage.ToString();
        cameraGo = camera;
    }
    private void LateUpdate()
    {
        if (cameraGo != null)
        {
            transform.LookAt(transform.position + cameraGo.transform.rotation * Vector3.forward, cameraGo.transform.rotation * Vector3.up);
        }
    }
}
