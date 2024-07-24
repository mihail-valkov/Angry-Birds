using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float levelEndWaitTime = 5f;
    [SerializeField] private GameObject _afterGameWonObject;
    [SerializeField] private GameObject _afterGameFailedObject;
    [SerializeField] private SlingshotHandler _slingshotHandler;
    [SerializeField] private UnityEngine.UI.Image _nextLevelImage;

    public int MaxNumberOfShots = 3;

    private int _usedNumberOfShots;

    private static GameManager _instance;

    private IconHandler _iconHandler;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;

        _iconHandler = FindObjectOfType<IconHandler>();
        _nextLevelImage.enabled = false;
    }

    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);
        
        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        return (_usedNumberOfShots < MaxNumberOfShots);
    }

    public void CheckForLastShot()
    {
        if (_usedNumberOfShots == MaxNumberOfShots)
        {
            //check waittime through a coroutine
            StartCoroutine(WaitForLevelEndOnLastShot());
        }
    }

    private IEnumerator WaitForLevelEndOnLastShot()
    {
        yield return new WaitForSeconds(levelEndWaitTime);

        //check if all baddies are dead
        
        if (!AreThereNoBaddies())
        {
            RestartLevel();
        }
    }

    private IEnumerator WaitForLevelEndOnBaddieKilled()
    {
        yield return new WaitForSeconds(0.3f);

        //check if all baddies are dead
        if (AreThereNoBaddies())
        {
            WinLevel();
        }
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting Level, remaining baddies:  " + FindObjectsOfType<Baddie>().Length.ToString());

        DOTween.Clear(true);
        _usedNumberOfShots = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Debug.Log("Loading next level");
        DOTween.Clear(true);
        _usedNumberOfShots = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void WinLevel()
    {
        Debug.Log("Level Complete");
        _afterGameWonObject.SetActive(true);
        _slingshotHandler.enabled = false;

        int currentScreenIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings - 1;

        if (currentScreenIndex < maxLevels)
        {
            _nextLevelImage.enabled = true;
        }
    }

    //Method to check for win if all baddies are dead
    public bool AreThereNoBaddies()
    {
        Debug.Log($"Baddies left: {FindObjectsOfType<Baddie>().Length}");
        return FindObjectsOfType<Baddie>().Length == 0;
    }

    public void BaddieKilled()
    {
        StartCoroutine(WaitForLevelEndOnBaddieKilled());
    }
}
