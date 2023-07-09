using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SetOrderOnAwake : MonoBehaviour
{
    private SortingGroup m_sortingGroup;
    private void Awake()
    {
        m_sortingGroup = GetComponent<SortingGroup>();
        m_sortingGroup.sortingOrder = -(int)(transform.position.z*30);
    }
}
