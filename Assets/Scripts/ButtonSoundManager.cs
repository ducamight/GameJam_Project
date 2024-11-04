using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioClip hoverButtonSound;       // Âm thanh khi di chuột qua nút
    private AudioSource audioSource;         // AudioSource để phát âm thanh

    void Awake()
    {
        // Tự động thêm AudioSource nếu chưa có
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;    // Không phát âm thanh khi khởi động

        // Tìm tất cả Button trong Canvas
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            // Thêm sự kiện hover và click cho mỗi button
            AddSoundEvents(button);
        }
    }

    // Hàm thêm sự kiện âm thanh cho button
    private void AddSoundEvents(Button button)
    {
        // Tạo sự kiện hover (OnPointerEnter) cho button
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Sự kiện hover (OnPointerEnter)
        EventTrigger.Entry hoverEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        hoverEntry.callback.AddListener((data) => PlayHoverSound());
        trigger.triggers.Add(hoverEntry);

        // Sự kiện click (OnPointerClick)
    }

    // Hàm phát âm thanh hover
    private void PlayHoverSound()
    {
        if (hoverButtonSound != null)
        {
            audioSource.PlayOneShot(hoverButtonSound);
        }
    }
}
