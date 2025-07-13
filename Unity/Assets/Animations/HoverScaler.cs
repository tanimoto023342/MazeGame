
using UnityEngine;
using UnityEngine.EventSystems; // �}�E�X�C�x���g�����m���邽�߂ɕK�v

public class HoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // �A�j���[�V�����̑��x�i�������قǑ����j
    [SerializeField] private float animationSpeed = 10f;

    // �z�o�[���̃X�P�[��
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);

    private Vector3 originalScale; // ���̃X�P�[����ۑ�
    private Vector3 targetScale;   // �ڕW�Ƃ���X�P�[��

    void Awake()
    {
        originalScale = transform.localScale; // �{�^���̌��݂̃X�P�[�����L�^
        targetScale = originalScale;         // �ŏ��͌��̃X�P�[�����ڕW
    }

    void Update()
    {
        // ���݂̃X�P�[����ڕW�X�P�[���ɏ��X�ɋ߂Â���
        // ����ɂ��A���炩�Ȋg��E�k���A�j���[�V�����ɂȂ�܂�
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // �}�E�X�J�[�\�����{�^���ɏ������
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale; // �ڕW�X�P�[�����g���̃X�P�[���ɐݒ�
    }

    // �}�E�X�J�[�\�����{�^�����痣�ꂽ��
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale; // �ڕW�X�P�[�������̃X�P�[���ɖ߂�
    }
}