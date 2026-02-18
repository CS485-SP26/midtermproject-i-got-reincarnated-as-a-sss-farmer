using UnityEngine;

public class EquippableItem : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnEquip(Transform holdPosition)
    {
        transform.SetParent(holdPosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (rb != null) {rb.isKinematic = true;}
  
        if (col != null) {col.enabled = false;}

    }

    public void OnDrop(Vector3 dropPosition)
    {
        transform.SetParent(null);
        transform.position = dropPosition;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }

        if (col != null) {col.enabled = true;}

    }
}
