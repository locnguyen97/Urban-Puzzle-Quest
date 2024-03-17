using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TouchPoint : MonoBehaviour
{

    public int id;
    [SerializeField] private List<Sprite> listWt;

    private bool isSelected = false;

    private void OnEnable()
    {
        List<int> l = new List<int>();
        if( GameManager.Instance.currentIndex == 1)
            l = new List<int>(){1,2};
        else if( GameManager.Instance.currentIndex == 2)
            l = new List<int>(){0,2};
        else
            l = new List<int>(){0,1,2};
        
        id = l[Random.Range(0, l.Count)];
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = listWt[id];

    }

    public void SetCollected()
    {
        if(isSelected) return;
        isSelected = true;
        transform.localScale = Vector3.one*0.35f;
        GetComponent<BoxCollider2D>().enabled = false;
    }
    public void SetUnCollected()
    {
        isSelected = false;
        transform.localScale = Vector3.one*0.3f;
        GetComponent<BoxCollider2D>().enabled = true;
    }
}