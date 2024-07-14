using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public RectTransform indicatorRect, fillRect;
    public Vector3 startPos, targetPos;
    public float currentDistance, maxDistance;
    public SimpleProjectile projObject;
    // Update is called once per frame
    void Update()
    {
        if (projObject.gameObject.activeSelf)
        {
            currentDistance = Vector3.Distance(projObject.transform.position, targetPos);
            fillRect.localScale = new Vector3((1 - currentDistance / maxDistance), (1 - currentDistance / maxDistance), 1);
            if (fillRect.localScale.x >= 0.97 || fillRect.localScale.x < 0 || currentDistance <= 0.03)
            {
                gameObject.SetActive(false);
            }
        }
        else gameObject.SetActive(false);
    }
    public void SetIndicator(SimpleProjectile proj, Transform targetPos)
    {
        projObject = proj;
        this.startPos = proj.transform.position;
        this.targetPos = targetPos.position;
        transform.position = targetPos.position;
        maxDistance = Vector3.Distance(this.startPos, this.targetPos);
        gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        fillRect.localScale = new Vector3(0,0,1);
    }
    private void OnEnable()
    {
        fillRect.localScale = new Vector3(0,0,1);
    }
}
