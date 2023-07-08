using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StockRemove : MonoBehaviour {
    [SerializeField] private Transform canvas = null;
    [SerializeField] private Transform stocksParent = null;
    [SerializeField] private List<Image> stocksImg = new();
    private int currentlyActivatedStock = 0;

    private void Update() {
        canvas.transform.eulerAngles = new Vector3(32.1f, 0, 0);
    }

    /// <summary>
    /// Apply a stock to a an enemy (which mean the player dash inside this enemy
    /// </summary>
    public void ApplyStock() {
        if (currentlyActivatedStock + 1 > stocksImg.Count) return;
        currentlyActivatedStock++;
        if (currentlyActivatedStock == 1) ActivateFirstStock();
        else {
            Sequence stockSeq = DOTween.Sequence();
            stockSeq.AppendInterval(0.25f);
            stockSeq.Append(stocksImg[currentlyActivatedStock - 1].DOColor(Color.red, 0.5f));
            stockSeq.Join(stocksImg[currentlyActivatedStock - 1].transform.DOPunchScale(new Vector3(0.6f, 0.6f, 0.6f), .5f, 1));
        }

        if (currentlyActivatedStock == stocksImg.Count) {
            Debug.Log("THE ENEMY IS STUN");
        }
    }

    /// <summary>
    /// Activate the first stock of an enemy
    /// </summary>
    private void ActivateFirstStock() {
        Sequence stockSeq = DOTween.Sequence();
        stockSeq.AppendInterval(0.25f);
        stockSeq.Append(stocksParent.DOScale(new Vector3(1, 1, 1), 0.5f));
        stockSeq.AppendInterval(0.25f);
        stockSeq.Append(stocksImg[0].DOColor(Color.red, 0.5f));
        stockSeq.Join(stocksImg[0].transform.DOPunchScale(new Vector3(0.6f, 0.6f, 0.6f), .5f, 1));
    }
}
