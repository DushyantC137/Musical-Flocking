using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockBehaviour : MonoBehaviour
{
    public Vector3 baseRotation;

    [Range(-20, 20)]
    public float maxSpeed = 1f;

    [Range(.1f, .5f)]
    public float maxForce = .03f;

    [Range(1, 10)]
    public float neighborhoodRadius = 3f;

    [Range(0, 3)]
    public float separationAmount = 1f;

    [Range(0, 3)]
    public float cohesionAmount = 1f;

    [Range(0, 3)]
    public float alignmentAmount = 1f;

    public Vector2 acceleration;
    public Vector2 velocity;
    [Range(-10f,10f)]
    public float SpeedMultiplier;
    public bool isMusicalFlock;
    public bool isBuffer;
    [SerializeField]private float timerT;
    float t = 0;
    [SerializeField] private float AmplitudeLimiter;
    private Vector2 Position
    {
        get
        {
            return gameObject.transform.position;
        }
        set
        {
            gameObject.transform.position = value;
        }
    }

    private void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        t = 0;
    }

    private void Update()
    {
        var boidColliders = Physics2D.OverlapCircleAll(Position, neighborhoodRadius);
        var boids = boidColliders.Select(o => o.GetComponent<FlockBehaviour>()).ToList();
        boids.Remove(this);

        Flock(boids);
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
        WrapAround();
        if (t > timerT)
        {
            ChangeWithMusic();
        }
        else t += Time.deltaTime;
    }
    private void ChangeWithMusic()
    {
        if (isMusicalFlock)
        {
            if (isBuffer)
            {
                if (AudioVisuals._AmplitudeBuffer > AmplitudeLimiter) 
                {
                    maxSpeed = AudioVisuals._AmplitudeBuffer * SpeedMultiplier; 
                }
                else
                {
                    maxSpeed = AudioVisuals._AmplitudeBuffer * -SpeedMultiplier;
                }

            }
            else 
            {
                if (AudioVisuals._AmplitudeBuffer > AmplitudeLimiter)
                {
                    maxSpeed = AudioVisuals._Amplitude * SpeedMultiplier;
                }
                else
                {
                    maxSpeed = AudioVisuals._Amplitude * -SpeedMultiplier;

                }
            }
    }
    }
    private void Flock(IEnumerable<FlockBehaviour> boids)
    {
        var alignment = Alignment(boids);
        var separation = Separation(boids);
        var cohesion = Cohesion(boids);

        acceleration = alignmentAmount * alignment + cohesionAmount * cohesion + separationAmount * separation;
    }

    public void UpdateVelocity()
    {
        velocity += acceleration;
        velocity = LimitMagnitude(velocity, maxSpeed);
    }

    private void UpdatePosition()
    {
        Position += velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
    }

    private Vector2 Alignment(IEnumerable<FlockBehaviour> boids)
    {
        var velocity = Vector2.zero;
        if (!boids.Any()) return velocity;

        foreach (var boid in boids)
        {
            velocity += boid.velocity;
        }
        velocity /= boids.Count();

        var steer = Steer(velocity.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Cohesion(IEnumerable<FlockBehaviour> boids)
    {
        if (!boids.Any()) return Vector2.zero;

        var sumPositions = Vector2.zero;
        foreach (var boid in boids)
        {
            sumPositions += boid.Position;
        }
        var average = sumPositions / boids.Count();
        var direction = average - Position;

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Separation(IEnumerable<FlockBehaviour> boids)
    {
        var direction = Vector2.zero;
        boids = boids.Where(o => DistanceTo(o) <= neighborhoodRadius / 2);
        if (!boids.Any()) return direction;

        foreach (var boid in boids)
        {
            var difference = Position - boid.Position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= boids.Count();

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - velocity;
        steer = LimitMagnitude(steer, maxForce);

        return steer;
    }

    private float DistanceTo(FlockBehaviour boid)
    {
        return Vector3.Distance(boid.transform.position, Position);
    }

    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }

    private void WrapAround()
    {
        if (Position.x < -9) Position = new Vector2(9, Position.y);
        if (Position.y < -5.43f) Position = new Vector2(Position.x, 5.43f);
        if (Position.x > 9) Position = new Vector2(-9, Position.y);
        if (Position.y > 5.43f) Position = new Vector2(Position.x, -5.43f);
    }
}