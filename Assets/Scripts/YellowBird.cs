using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class YellowBird : Bird
{
    

    public UnityAction OnBirdDestroyed = delegate { };
    public UnityAction<Bird> OnBirdShot = delegate { };

    private BirdState _state;
    public BirdState State => _state;

    private float _minVelocity = 0.05f;
    private bool _flagDestroy = false;

    // Start is called before the first frame update
    //mematikan fungsi physics dan collider burung
    void Start()
    {
        Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Collider.enabled = false;
        _state = BirdState.Idle;
    }
    private void FixedUpdate()
    {
        if (_state == BirdState.Idle && Rigidbody.velocity.sqrMagnitude >= _minVelocity)
        {
            _state = BirdState.Thrown;
        }
        if ((_state == BirdState.Thrown || _state == BirdState.HitSomething) &&
            Rigidbody.velocity.sqrMagnitude < _minVelocity &&
            !_flagDestroy)
        {
            //hancurkan gameobject setelah 2 detik
            //jika kecepatannya sudah kurang dari batas min
            _flagDestroy = true;
            StartCoroutine(DestroyAfter(2));
        }
    }
    private IEnumerator DestroyAfter(float second)
    {
        yield return new WaitForSeconds(second);
        Destroy(gameObject);
    }
    //untuk inisialisasi posisi dan mengubah parent dari game object burung
    public void MoveTo(Vector2 target, GameObject parent)
    {
        gameObject.transform.SetParent(parent.transform);
        gameObject.transform.position = target;
    }
    //untuk melemparkan burung dengan arah, jarak tali yang ditarik, dan kec. awal
    public void Shoot(Vector2 velocity, float distance, float speed)
    {
        Collider.enabled = true;
        Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Rigidbody.velocity = velocity * speed * distance;
        OnBirdShot(this);
    }
    void OnDestroy()
    {
        if (_state == BirdState.Thrown || _state == BirdState.HitSomething)
            OnBirdDestroyed();
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        _state = BirdState.HitSomething;
    }
    
    public float _boostForce = 100;
    public bool _hasBoost = false;

    public void Boost()
    {
        if (State == BirdState.Thrown && !_hasBoost)
        {
            Rigidbody.AddForce(Rigidbody.velocity * _boostForce);
            _hasBoost = true;
        }
    }

    public override void OnTap()
    {
        Boost();
    }
}