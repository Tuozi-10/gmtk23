using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay;
using src.Extensions;
using src.Singletons;
using UnityEngine;

public class TesMortManager : MonoSingleton<TesMortManager>
{
    private CanvasGroup m_canvasgroup;
    
    // Start is called before the first frame update
    void Start()
    {
        m_canvasgroup = GetComponent<CanvasGroup>();
        m_canvasgroup.alpha = 0;
        m_canvasgroup.NoInteractions();
    }


    public static void Respawn()
    {
        instance.m_canvasgroup.DOFade(1, 0.35f).OnComplete(PlayerController.instance.Respawn);
        instance.m_canvasgroup.DOFade(0, 0.35f).SetDelay(0.65f);
    }
    
}
