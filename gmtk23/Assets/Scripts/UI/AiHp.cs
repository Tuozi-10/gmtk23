using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AiHp : MonoBehaviour
    {
        [SerializeField] private Image m_fillImage;
        [SerializeField] private CanvasGroup m_canvasGroup;

        private void Update()
        {
            transform.localScale = new Vector3(transform.parent.localScale.x * 0.1f, 0.1f, 0.1f);
        }

        public void HpRatio(float hpRatio)
        {
            m_fillImage.DOFillAmount(hpRatio, 0.33f);
            m_canvasGroup.alpha = hpRatio >= 0.99f ? 0 : 1f;
        }
    }
}
