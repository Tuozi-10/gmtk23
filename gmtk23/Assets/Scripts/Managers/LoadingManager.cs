using System.Collections;
using DG.Tweening;
using src.Extensions;
using src.Singletons;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingManager : MonoSingleton<LoadingManager>
    {
        [SerializeField] private float durationLoading = 1.5f;
        [SerializeField] private float fadeDuration = 0.75f;

        private CanvasGroup m_canvasGroup;
        
        private void Start()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
           StartCoroutine(DelayLoadingScreen());
        }

        IEnumerator DelayLoadingScreen()
        {
            yield return new WaitForSeconds(durationLoading);
            GameManager.instance.LoadScene(GameManager.Scenes.Menu);
            HideLoadingScreen();
        }

        public void ShowLoadingScreen()
        {
            m_canvasGroup.DOKill();
            m_canvasGroup.DOFade(1,fadeDuration).OnComplete(m_canvasGroup.EnableInteractions);
        }
        
        public void HideLoadingScreen()
        {
            m_canvasGroup.DOKill();
            m_canvasGroup.NoInteractions();
            m_canvasGroup.DOFade(0,fadeDuration);
        }
        
    }
}
