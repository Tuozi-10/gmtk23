using src.Singletons;
using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager> {
    [SerializeField] private float slowMoTimeScale = 0.1f;
    [SerializeField] private float decelerationScale = 0.1f;
    private float targetTimeScale = 1;
    private float startFixedDeltaTime = 0;

    private void Start() {
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update() {
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, decelerationScale);
        Time.fixedDeltaTime = startFixedDeltaTime * Time.timeScale;
    }
    
    public void StartSlowMotion() => targetTimeScale = slowMoTimeScale;
    public void EndSlowMotion() => targetTimeScale = 1;
}
