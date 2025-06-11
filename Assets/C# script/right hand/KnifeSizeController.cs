using UnityEngine;
using UnityEngine.XR;

public class KnifeSizeController : MonoBehaviour
{
    public Vector3[] sizeLevels = new Vector3[5] {
        new Vector3(-0.05f, 0.05f, 0.05f),    // a: 最小
        new Vector3(-0.075f, 0.075f, 0.075f), // b
        new Vector3(-0.1f, 0.1f, 0.1f),       // c: 默认
        new Vector3(-0.125f, 0.125f, 0.125f), // d
        new Vector3(-0.15f, 0.15f, 0.15f)     // e: 最大
    };


    private int currentIndex = 2;  // 初始为 c
    private bool triggerDownLastFrame = false;
    private bool gripDownLastFrame = false;

    void Update()
    {
        InputDevice deviceR = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (deviceR.isValid)
        {
            // 右手扳机键增大一档
            if (deviceR.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
            {
                if (triggerPressed && !triggerDownLastFrame)
                {
                    IncreaseSize();
                }
                triggerDownLastFrame = triggerPressed;
            }

            // 右手 Grip 键缩小一档
            if (deviceR.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
            {
                if (gripPressed && !gripDownLastFrame)
                {
                    DecreaseSize();
                }
                gripDownLastFrame = gripPressed;
            }
        }
    }

    private void IncreaseSize()
    {
        if (currentIndex < sizeLevels.Length - 1)
        {
            currentIndex++;
            ApplySize();
            Debug.Log($"[XR] Knife size increased to level {(char)('a' + currentIndex)}");
        }
        else
        {
            Debug.Log("[XR] Already at maximum size (e)");
        }
    }

    private void DecreaseSize()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ApplySize();
            Debug.Log($"[XR] Knife size decreased to level {(char)('a' + currentIndex)}");
        }
        else
        {
            Debug.Log("[XR] Already at minimum size (a)");
        }
    }

    private void ApplySize()
    {
        transform.localScale = sizeLevels[currentIndex];
    }
}
