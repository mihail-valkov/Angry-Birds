using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHandler : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image[] _icons;
    [SerializeField] private Color _usedColor;

    public void UseShot(int shotNumber)
    {
        _icons[shotNumber - 1].color = _usedColor;
    }
}
