using UnityEngine;

public class RaycastTester : MonoBehaviour
{
    [SerializeField] private float distance = 5f;

    private void Update()
    {
        Transform t = transform;

        // Draw a visible ray in the Scene view
        Debug.DrawRay(t.position, t.forward * distance, Color.red);

        if (Physics.Raycast(t.position, t.forward, out RaycastHit hit, distance))
        {
            Debug.Log($"[RaycastTester] Hit: {hit.collider.name}");
        }
        else
        {
            Debug.Log("[RaycastTester] Raycast hit nothing");
        }
    }
}
