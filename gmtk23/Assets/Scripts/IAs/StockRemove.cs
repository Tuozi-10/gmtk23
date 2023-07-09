using System.Collections.Generic;
using DG.Tweening;
using IAs;
using UnityEngine;
using UnityEngine.UI;

public class StockRemove : MonoBehaviour {
    [SerializeField] private Transform canvas = null;
    [SerializeField] private Transform stocksParent = null;
    [SerializeField] private List<Image> stocksImg = new();
    private int currentlyActivatedStock = 0;

    private void Update() {
        canvas.transform.eulerAngles = new Vector3(90f, 0, -180f);
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
            GetComponent<AI>().RemoveRandomItem();
            ResetStock();
        }
    }

    /// <summary>
    /// Reset the stocks
    /// </summary>
    private void ResetStock() {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.25f);
        foreach (var img in stocksImg) {
            seq.Append(img.DOColor(new Color(70, 70, 70), 0)); 
        }
        currentlyActivatedStock = 0;
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
