using UnityEngine;
using DG.Tweening;

public class ShowAndClosePanel : MonoBehaviour
{
    public RectTransform panel;
    CanvasGroup cg;
    public static ShowAndClosePanel ins;
    void Start()
    {
        cg = panel.GetComponent<CanvasGroup>();
    }
    private void Awake()
    {
        ins = this;
    }
    public void ShowPanel()
    {
        SoundManager.instance.PlayEffect_Instance(11);

        panel.transform.parent.gameObject.SetActive(true);
        panel.DOScale(1, .25f);
        cg.DOFade(1, .25f);
    }
    public void Cross()
    {
        SoundManager.instance.PlayEffect_Instance(9);

        panel.DOScale(.5f, .25f);
        cg.DOFade(0, .25f).OnComplete(() =>
        {
            //isUiActive = false;
            panel.transform.parent.gameObject.SetActive(false);
        });
    }
}
