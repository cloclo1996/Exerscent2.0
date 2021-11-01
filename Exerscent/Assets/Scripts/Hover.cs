using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pointerEnter(GameObject caller) {
        caller.transform.DOScale(new Vector3(.8f, .8f, .8f), .15f).SetEase(Ease.OutBack);
    }

    public void pointerLeave(GameObject caller) {
        caller.transform.DOScale(new Vector3(.7f, .7f, .7f), .15f).SetEase(Ease.OutSine);
    }
}
