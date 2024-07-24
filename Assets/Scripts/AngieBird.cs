using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngieBird : MonoBehaviour
{
    [SerializeField] private AudioClip _hitClip;
    //a rigid body 2d
    private Rigidbody2D _birdBody;
    //get the circlecollider
    private CircleCollider2D _birdCollider;
    private bool _hasBeenLaunched;
    private bool _shouldFaceVelocityDir;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        //get the rigid body 2d component
        _birdBody = GetComponent<Rigidbody2D>();
        _birdCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        _birdBody.isKinematic = true;
        _birdCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if(_hasBeenLaunched && _shouldFaceVelocityDir)
        {
            transform.right = _birdBody.velocity;
        }
    }

    public void LaunchBird(Vector2 direction, float force) 
    {
        //add force to the rigid body
        _birdBody.isKinematic = false;
        _birdCollider.enabled = true;

        _birdBody.AddForce(direction * force, ForceMode2D.Impulse);

        _hasBeenLaunched = true;
        _shouldFaceVelocityDir = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        _shouldFaceVelocityDir = false;
        SoundManager.Instance.PlayClip(_hitClip, _audioSource);
        Destroy(this);
    }
}
