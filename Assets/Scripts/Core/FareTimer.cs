using UnityEngine;
using TMPro;

public class FareTimer : MonoBehaviour
{
    #region Properties and Fields

    [SerializeField] TextMeshProUGUI _timerText;

    private bool _timerIsActive = false;
    private float _currentTime;

    #endregion

    #region Unity Methods

    private void Start() => SetActive(false);

    private void Update()
    {
        if (!_timerIsActive) return;
        _currentTime -= Time.deltaTime;
        _currentTime = Mathf.Max(0f, _currentTime);
        _timerText.text = GetFormattedTime(_currentTime);
    }

    #endregion

    #region Public Methods

    public void SetTimerDuration(float duration)
        => _currentTime = duration;

    public void SetActive(bool active)
    {
        _timerIsActive = active;
        _timerText.enabled = active;
    }

    public float GetCurrentTime()
        => _currentTime;

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
