using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class SlingshotHandler : MonoBehaviour
{
    
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transforms")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance = 5f;
    [SerializeField] private float _shotForce = 9f;
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;

    [Header("Camera")]
    [SerializeField] private CameraManager _cameraManager;


    [Header("Bird")]
    [SerializeField] private AngieBird _angieBirdPrefab;
    [SerializeField] private float _angieBirdPositionOffset = .33f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip[] _elasticReleasedClips;
    
    private AngieBird _spawnedAngieBird;
    private Vector2 _slingShotLinesPosition;
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _birdOnSlingshot;

    private bool _clickedWithinSlingshotArea;
    private AudioSource _audioSource;

    //Spawn a bird
    private void SpawnAngieBird()
    {
        _elasticTransform.DOComplete();

        SetLines(_idlePosition.position); 

        //calculate the normalized position, taking into account the position offset
        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angieBirdPositionOffset;

        _spawnedAngieBird = Instantiate(_angieBirdPrefab, spawnPosition, Quaternion.identity);
        //also rotate the bird in the direction of the slingshot
        _spawnedAngieBird.transform.right = dir;

        _birdOnSlingshot = true;
    }

    //position and rotate the bird
    private void PositionAndRotateAngieBird()
    {
        _spawnedAngieBird.transform.position = _slingShotLinesPosition + _directionNormalized * _angieBirdPositionOffset;
        _spawnedAngieBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);

        SpawnAngieBird();

        _cameraManager.SwitchToIdleCam();
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;
        SpawnAngieBird();
    }

    // Update is called once per frame
    private void Update()
    { 
        // check if the mouse is pressed within the slingshot area
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingshotArea() && _birdOnSlingshot)
        {
            _clickedWithinSlingshotArea = true;

            if (_birdOnSlingshot)
            {
                SoundManager.Instance.PlayClip(_elasticPulledClip, _audioSource);
                _cameraManager.SwitchToFollowCam(_spawnedAngieBird.transform);
            }
        }

        if (InputManager.IsLeftMouseButtonPressed && _clickedWithinSlingshotArea)
        {
            DrawSlingshot();
            PositionAndRotateAngieBird();
        }

        // check if the mouse is released 
        if (InputManager.WasLeftMouseButtonReleased  && _clickedWithinSlingshotArea && _birdOnSlingshot)
        {
            _clickedWithinSlingshotArea = false;

            _spawnedAngieBird.LaunchBird(_direction, _shotForce);

            SoundManager.Instance.PlayRandomClip(_elasticReleasedClips, _audioSource);

            GameManager.Instance.UseShot();
            
            _birdOnSlingshot = false;

            //SetLines(_centerPosition.position);
            AnimateSlingshot();

            if (GameManager.Instance.HasEnoughShots())
            {
                StartCoroutine(SpawnAngieBirdAfterTime());
            }
        }
    }

    private void DrawSlingshot()
    {
        // detect the position of the mouse, then change the position of the line renderers accordingly
        Vector3 position = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        // clamp the position of the line renderers to the max distance, use clampmagnitude
        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(position - _centerPosition.position, _maxDistance);

        SetLines(_slingShotLinesPosition);
        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }

    private void AnimateSlingshot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);
        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = Math.Min(2f, dist / _elasticDivider);
        Debug.Log($"AnimateSlingshot for {time} seconds at {dist}, {dist / _elasticDivider}");

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);

        StartCoroutine(AnimateSlingshotLines(_elasticTransform, time));
    }

    //a coroutine to animate the slingshot
    private IEnumerator AnimateSlingshotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }

}
