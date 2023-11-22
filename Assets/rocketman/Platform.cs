using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float bouncyAmount;

    private void OnCollisionEnter(Collision collision)
    {
        BirdOwnerAssigner assigner = collision.transform.GetComponentInParent<BirdOwnerAssigner>();
        if (assigner != null)
        {
            if (!assigner.isMine)
            {
                Debug.Log("benim deil returnn");
                return;
            }
        }
        if (collision.transform.TryGetComponent(out Rigidbody rb))
        {
            rb.GetComponent<BallController>().ChangeShape(BallController.Shape.Ball);
            // Get the point of collision and the collision surface's normal
            ContactPoint contactPoint = collision.contacts[0];
            Vector3 contactNormal = contactPoint.normal;
            Debug.Log(rb.velocity);
            // Reverse the movement direction and apply force to the object
            float forceMagnitude = rb.velocity.magnitude; // Magnitude of force based on the object's velocity
            Vector3 oppositeForce = bouncyAmount * forceMagnitude * -contactNormal;
            oppositeForce.y = Mathf.Clamp(oppositeForce.y, 30f, 60f);
            rb.AddForce(oppositeForce, ForceMode.Impulse);
        }
    }
}
