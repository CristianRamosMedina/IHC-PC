using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;  // Asegúrate de incluir esta directiva

public class PlayerUI : MonoBehaviour
{
    public TMP_Text zoomText;  // Referencia al texto del nivel de zoom
    public Image crosshairImage;  // Referencia a la mira (crosshair)
    public Image altCrosshairImage;  // Referencia a la imagen de la segunda mira

    public Sprite alternateCrosshair;  // Sprite de la segunda mira
    public Sprite defaultCrosshair; // Mira por defecto
    public Sprite zoomed1Crosshair; // Mira para zoom x1
    public Sprite zoomed2Crosshair; // Mira para zoom x2
    public Sprite zoomed3Crosshair; // Mira para zoom x3

    private bool isAltCrosshairVisible = false;  // Controla si la segunda mira está visible

    // Actualiza el texto del zoom
    public void UpdateZoomText(int zoomIndex)
    {
        string zoomDisplay = zoomIndex switch
        {
            0 => "Zoom: x1",
            1 => "Zoom: x2",
            2 => "Zoom: x3",
            _ => "Zoom: OFF"
        };

        zoomText.text = zoomDisplay;

        // Cambiar la mira según el nivel de zoom
        switch (zoomIndex)
        {
            case 0:
                crosshairImage.sprite = zoomed1Crosshair;  // Mira por defecto
                break;
            case 1:
                crosshairImage.sprite = zoomed2Crosshair; // Mira para zoom x2
                break;
            case 2:
                crosshairImage.sprite = zoomed3Crosshair; // Mira para zoom x3
                break;
            default:
                crosshairImage.sprite = defaultCrosshair;  // Restablecer a la mira por defecto
                break;
        }
    }

    // Cambia la visibilidad de la segunda mira al presionar la barra espaciadora
    public void ToggleAltCrosshair()
    {
        if (!isAltCrosshairVisible)
        {
            // Muestra la segunda mira
            altCrosshairImage.gameObject.SetActive(true);
            altCrosshairImage.sprite = alternateCrosshair;
            isAltCrosshairVisible = true;

            // Reinicia la opacidad antes de la animación
            Color startColor = altCrosshairImage.color;
            altCrosshairImage.color = new Color(startColor.r, startColor.g, startColor.b, 1f); // Opacidad completa
            // Inicia la animación para que desaparezca después de 1 segundo
            StartCoroutine(FadeOutCrosshair());
        }
    }

    // Desaparece suavemente la segunda mira durante 1 segundo
    private IEnumerator FadeOutCrosshair()
    {
        float elapsedTime = 0f;
        float fadeDuration = 1f; // Duración del fade (1 segundo)

        Color startColor = altCrosshairImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Totalmente transparente

        // Animación para que la segunda mira desaparezca suavemente
        while (elapsedTime < fadeDuration)
        {
            altCrosshairImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        altCrosshairImage.color = endColor; // Asegura que la opacidad quede al mínimo
        altCrosshairImage.gameObject.SetActive(false); // Desactiva la mira después de la animación
        isAltCrosshairVisible = false;  // Marca la mira como no visible
    }
}