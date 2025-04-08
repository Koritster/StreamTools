using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;          // El slider de la barra de vida
    public Image fillImage;        // La imagen del relleno (Fill)

    /// <summary>
    /// Asigna vida actual y máxima, y actualiza el color del fill.
    /// </summary>
    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (slider == null || fillImage == null) return;

        slider.maxValue = maxHealth;
        slider.value = currentHealth;

        float percent = (float)currentHealth / maxHealth;

        // Cambia el color según el porcentaje
        if (percent >= 0.7f)
        {
            fillImage.color = Color.green;
        }
        else if (percent >= 0.3f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
}
