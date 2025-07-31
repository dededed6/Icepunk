using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Standing : MonoBehaviour
{
    [Header("Standing Display")]
    public GameObject 스탠딩;
    public GameObject mainCamera;
    public TextMeshProUGUI 내용;
    public TextMeshProUGUI 이름;
    public GameObject 이름칸;
    public Image 배경;
    public GameObject 백설기;
    public GameObject 강도;
    public GameObject 휠체어남;

    public Sprite 폐허;

    private GameObject target = null;

    void Start()
    {
        // Subscribe to events
        GameEvents.OnDisplay += HandleDisplay;
        GameEvents.OnHide += HandleHide;
        GameEvents.OnStanding += HandleStanding;
        GameEvents.onCameraMove += HandleCameraMove;
        GameEvents.onText += HandleText;
        GameEvents.onName += HandleName;
        GameEvents.onNameSpace += HandleNameSpace;
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        GameEvents.OnDisplay -= HandleDisplay;
        GameEvents.OnHide -= HandleHide;
        GameEvents.onCameraMove -= HandleCameraMove;
        GameEvents.onText -= HandleText;
        GameEvents.onName -= HandleName;
        GameEvents.onNameSpace -= HandleNameSpace;
    }

    private void HandleDisplay(string amount)
    {
        if (string.Equals("백설기", amount)) {
                백설기.SetActive(true);
        }
        if (string.Equals("강도", amount)) {
                강도.SetActive(true);
        }
        if (string.Equals("휠체어남", amount)) {
                휠체어남.SetActive(true);
        }
    }

    private void HandleHide(string amount)
    {
        if (string.Equals("백설기", amount)) {
                백설기.SetActive(false);
        }
        if (string.Equals("강도", amount)) {
                강도.SetActive(false);
        }
        if (string.Equals("휠체어남", amount)) {
                휠체어남.SetActive(false);
        }
    }

    private void HandleStanding(string bg)
    {
        스탠딩.SetActive(true);
        배경.sprite = 폐허;
    }

    private void HandleText(string txt)
    {
        내용.text = txt;
    }

    private void HandleName(string nm)
    {
        이름.text = nm;
    }

    private void HandleNameSpace(bool on)
    {
        이름칸.SetActive(on);
    }

    private void HandleCameraMove(string amount)
    {
        if (string.Equals("백설기", amount))
        {
            target = 백설기;
        }
        if (string.Equals("강도", amount))
        {
            target = 강도;
        }
        if (string.Equals("휠체어남", amount))
        {
            target = 휠체어남;
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 카메라좌표 = Vector3.Lerp(mainCamera.transform.position, target.transform.position, 0.1f);
            카메라좌표.y += 0.2f;
            카메라좌표.z = -10;
            mainCamera.transform.position = 카메라좌표;
        }
    }
}
