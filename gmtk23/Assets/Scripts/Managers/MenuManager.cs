using DG.Tweening;
using src.Extensions;
using src.Singletons;
using UnityEngine;

namespace Managers
{
   [RequireComponent(typeof(CanvasGroup))]
   public class MenuManager : MonoSingleton<MenuManager>
   {
      private CanvasGroup m_canvasGroup;
      [SerializeField] private float fadeDuration = 0.75f;
      [SerializeField] private float openSideDuration = 0.65f;
      [SerializeField] private RectTransform m_menuSide;
      
      private void Start()
      {
         m_canvasGroup = GetComponent<CanvasGroup>();
      }
      
      public void PressButtonPlay()
      {
         GameManager.instance.LoadScene(GameManager.Scenes.Game);
         HideMenu();
      }
      
      public void ShowMenu()
      {
         m_canvasGroup.DOKill();
         m_canvasGroup.DOFade(1,fadeDuration).OnComplete(m_canvasGroup.EnableInteractions);
      }
        
      public void HideMenu()
      {
         m_canvasGroup.DOKill();
         m_canvasGroup.NoInteractions();
         m_canvasGroup.DOFade(0,fadeDuration);
      }

      private bool m_opened = true;
      
      public void ShowMenuSide()
      {
         m_opened = !m_opened;
         m_menuSide.DOKill();
         m_menuSide.DOAnchorPosX( m_opened ? -400 : -50,openSideDuration).SetEase(m_opened ? Ease.InBack : Ease.OutBack);
      }

   }
}
