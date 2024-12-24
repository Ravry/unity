using UnityEngine;
using TMPro;
using DG.Tweening;

public class DebugCanvas : MonoBehaviour
{
    [Header("Panel Options")]
    [SerializeField] private RectTransform panel_D;
    [SerializeField] private Vector2 offscreenPos, onscreenPos;
    [SerializeField] private float duration = .5f;


    [Header("Panel Content")]
    [SerializeField] private TMP_Text playerVelocity_D;
    [SerializeField] private TMP_Text playerGrounded_D;
    [SerializeField] private TMP_Text playerState_D;


    private bool debugMode = false;
    
    void Start() {
        panel_D.anchoredPosition = offscreenPos;
    }

    private void SlideIn()
    {
        panel_D.DOAnchorPos(onscreenPos, duration).SetEase(Ease.OutBounce);
    }

    private void SlideOut()
    {
        panel_D.DOAnchorPos(offscreenPos, duration).SetEase(Ease.InOutQuad);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            debugMode = !debugMode;
            if (debugMode)
                SlideIn();
            else 
                SlideOut();
        }

        playerVelocity_D.text = PlayerStateMachine.instance.velocity.ToString();
        playerGrounded_D.text = PlayerStateMachine.instance.grounded.ToString();
        playerState_D.text = PlayerStateMachine.instance.GetCurrentState().ToString();
    } 
}