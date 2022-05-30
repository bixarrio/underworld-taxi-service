using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UTS.Control;

public class GameTimer : MonoBehaviour
{
    #region Properties and Fields

    [SerializeField] TextMeshProUGUI _gameTimeText;
    [SerializeField] GameObject _gameOverObject;
    [SerializeField] bool _endlessMode = false;

    private float _currentTime = 300f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (_endlessMode) _currentTime = 0f;
    }

    private void Update()
    {
        if (_endlessMode) _currentTime += Time.deltaTime;
        else _currentTime -= Time.deltaTime;
        _currentTime = Mathf.Max(0f, _currentTime);

        if (!_endlessMode && _currentTime <= 0f)
        {
            _gameOverObject.SetActive(true);
            FindObjectOfType<Movement>().SetActive(false);
        }

        _gameTimeText.text = GetFormattedTime(_currentTime);
    }

    #endregion

    #region Public Methods

    public static GameTimer Get()
        => FindObjectOfType<GameTimer>();

    public void AddTime(float timeToAdd)
    {
        // TODO: Fanfare
        _currentTime += timeToAdd;
    }

    #endregion

    #region Private Methods

    private string GetFormattedTime(float time)
    {
        var span = System.TimeSpan.FromSeconds(time);
        if (span.Hours > 0) return span.ToString("h\\:mm\\:ss");
        if (span.Minutes > 0) return span.ToString("m\\:ss");
        return span.ToString("s\\.ff");
    }

    #endregion
}
