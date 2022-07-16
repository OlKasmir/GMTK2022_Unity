using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions {
  /// <summary>
  /// Darkens a color by percentage
  /// </summary>
  public static Color Darken(this Color color, float perc) {
    color.r *= 1 - perc;
    color.g *= 1 - perc;
    color.b *= 1 - perc;
    return color;
  }

  /// <summary>
  /// Lightens a color by percentage
  /// </summary>
  public static Color Lighten(this Color color, float perc) {
    color.r *= 1 + perc;
    color.g *= 1 + perc;
    color.b *= 1 + perc;
    return color;
  }

  /// <summary>
  /// Returns a color in html format
  /// </summary>
  public static string GetHtmlFormat(this Color color) {
    return ColorUtility.ToHtmlStringRGBA(color);
  }

  /// <summary>
  /// Returns the text enclosed by color html tags.
  /// </summary>
  /// <param name="text">The text to be colored</param>
  /// <returns>Colored input text</returns>
  public static string ColorString(this Color color, string text) {
    return string.Format("<color=#{0}>{1}</color>", color.GetHtmlFormat(), text);
  }

  /// <summary>
  /// Returns the prefix of the html color tag including the specified color
  /// </summary>
  public static string GetHtmlPrefix(this Color color) {
    return string.Format("<color=#{0}>", color.GetHtmlFormat());
  }
}