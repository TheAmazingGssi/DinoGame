using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject attackIcon;
    [SerializeField] private GameObject moveStick;
    [SerializeField] private GameObject blockIcon;

    public void ShowAttackIcon(bool v) { if (attackIcon) attackIcon.SetActive(v); }
    public void ShowMoveStick(bool v)  { if (moveStick)  moveStick.SetActive(v); }
    public void ShowBlockIcon(bool v)  { if (blockIcon)  blockIcon.SetActive(v); }

    public void HideAll()
    {
        if (attackIcon) attackIcon.SetActive(false);
        if (moveStick)  moveStick.SetActive(false);
        if (blockIcon)  blockIcon.SetActive(false);
    }
}