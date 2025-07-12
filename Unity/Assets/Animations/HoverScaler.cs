
using UnityEngine;
using UnityEngine.EventSystems; // マウスイベントを検知するために必要

public class HoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // アニメーションの速度（小さいほど速い）
    [SerializeField] private float animationSpeed = 10f;

    // ホバー時のスケール
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);

    private Vector3 originalScale; // 元のスケールを保存
    private Vector3 targetScale;   // 目標とするスケール

    void Awake()
    {
        originalScale = transform.localScale; // ボタンの現在のスケールを記録
        targetScale = originalScale;         // 最初は元のスケールが目標
    }

    void Update()
    {
        // 現在のスケールを目標スケールに徐々に近づける
        // これにより、滑らかな拡大・縮小アニメーションになります
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // マウスカーソルがボタンに乗った時
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale; // 目標スケールを拡大後のスケールに設定
    }

    // マウスカーソルがボタンから離れた時
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale; // 目標スケールを元のスケールに戻す
    }
}