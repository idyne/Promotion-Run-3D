using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FateGames;
public class LevitatingText : MonoBehaviour, IPooledObject
{
    [SerializeField] private bool promotion = false;
    [SerializeField] private float height = 6;
    [SerializeField] private float time = 2;
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void SetColor(Color newColor)
    {
        text.color = newColor;
    }
    public void OnObjectSpawn()
    {
        transform.localScale = Vector3.one;
        LeanTween.cancel(gameObject);
        transform.LeanMoveY(transform.position.y + height, time).setOnComplete(() =>
        {
            if (promotion)
            {
                transform.LeanRotateZ(10, 0.2f).setLoopPingPong(1).setOnComplete(() =>
                {
                    LeanTween.delayedCall(0.2f, () =>
                    {
                        transform.LeanMoveY(transform.position.y - height, time / 2).setOnComplete(() => { gameObject.SetActive(false); });
                    });
                });
            }
            else
            {
                transform.LeanScale(Vector3.zero, 0.2f).setOnComplete(() => { gameObject.SetActive(false); });

            }
        });
    }
}
