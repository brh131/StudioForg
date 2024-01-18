using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class KinematicObject : MonoBehaviour
{
    //Parameters
    public float kickEpsilon;
    public float groundAngleLimit;
    public float groundedThreshold;
    public float minMove;
    //Components
    private Rigidbody2D rb;
    //Variables
    private List<RaycastHit2D> moveHits = new();
    private ContactFilter2D contactFilter;
    private ContactFilter2D groundedFilter;
    protected RaycastHit2D groundHit;
    [SerializeField] protected Vector2 velocity= Vector2.zero;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        contactFilter = new();
        contactFilter.layerMask = LayerMask.GetMask("Ground");

        groundedFilter = new();
        groundedFilter.layerMask = LayerMask.GetMask("Ground");
        groundedFilter.minNormalAngle = groundAngleLimit;
        groundedFilter.maxNormalAngle = 180f-groundAngleLimit;
        groundedFilter.useOutsideDepth = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void Move()
    {
        if (velocity.magnitude < minMove) return;
        //Cast the player collider in the direction and length of their next move. Find all colliders in the way.
        int numHits = rb.Cast(velocity, contactFilter, moveHits, distance: velocity.magnitude * Time.deltaTime);

        //Go through all colliders, taking the first one where the player is moving towards the collider.
        RaycastHit2D firstHit = new();
        foreach (RaycastHit2D hit in moveHits.Take(numHits).OrderBy(hit => hit.distance))
        {
            if (Vector2.Dot(velocity, hit.normal) < 0f)
            {
                firstHit = hit;
                break;
            }
        }
        //Note: you should maybe implement your ground check as a downward cast on the player's collider. Have some small leeway distance
        //Also you need to add a minimum move distance to avoid shaking and such.
        //Then you need to make it so that when the player is grounded their movement is along the surface of the ground (perpendicular to the normal)
        //And I guess that also means you need to get your state machine going for jumps and falling and such
        //And to do that you should research a better state machine bc you will have to add your fighting comboes to this and boy do I not want to fiddle with a bunch of switch statements
        //Is this a trello board now? I guess so.

        //U need to entirely rethink ur approach to all this. spriteshapes have a spline. When a character is grounded, snap to the spline. Move along the spline unless it gets too
        //steep.

        //Todo: check if slope is shallow enough to stand on
        Vector2 nextPos;
        if (firstHit.collider != null)
        {
            nextPos = firstHit.centroid;
            velocity -= MyUtils.Project(velocity, firstHit.normal);
            velocity += kickEpsilon * firstHit.normal;
        }
        else
        {
            nextPos = rb.position + velocity*Time.deltaTime;
        }

        rb.MovePosition(nextPos);
    }

    public bool CheckGrounded()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        int numHits = rb.Cast(Vector2.down, groundedFilter, hits, groundedThreshold);
        if(numHits == 0)
        {
            return false;
        }
        else
        {

        }
        return numHits > 0;
    }
}
