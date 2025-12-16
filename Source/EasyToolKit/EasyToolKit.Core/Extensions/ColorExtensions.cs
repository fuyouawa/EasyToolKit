using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides extension methods for Unity's Color struct to enable fluent API operations
    /// for color manipulation and modification.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Sets the red component of the color and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="r">The new red value (0-1)</param>
        /// <returns>The modified color with updated red component</returns>
        public static Color SetR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        /// <summary>
        /// Sets the green component of the color and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="g">The new green value (0-1)</param>
        /// <returns>The modified color with updated green component</returns>
        public static Color SetG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        /// <summary>
        /// Sets the blue component of the color and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="b">The new blue value (0-1)</param>
        /// <returns>The modified color with updated blue component</returns>
        public static Color SetB(this Color color, float b)
        {
            color.b = b;
            return color;
        }

        /// <summary>
        /// Sets the alpha component of the color and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="a">The new alpha value (0-1)</param>
        /// <returns>The modified color with updated alpha component</returns>
        public static Color SetA(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        /// <summary>
        /// Multiplies the red component by the specified ratio and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="ratio">The multiplication ratio</param>
        /// <returns>The modified color with red component multiplied</returns>
        public static Color MulR(this Color color, float ratio)
        {
            color.r *= ratio;
            return color;
        }

        /// <summary>
        /// Multiplies the green component by the specified ratio and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="ratio">The multiplication ratio</param>
        /// <returns>The modified color with green component multiplied</returns>
        public static Color MulG(this Color color, float ratio)
        {
            color.g *= ratio;
            return color;
        }

        /// <summary>
        /// Multiplies the blue component by the specified ratio and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="ratio">The multiplication ratio</param>
        /// <returns>The modified color with blue component multiplied</returns>
        public static Color MulB(this Color color, float ratio)
        {
            color.b *= ratio;
            return color;
        }

        /// <summary>
        /// Multiplies the alpha component by the specified ratio and returns the modified color for fluent chaining.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="ratio">The multiplication ratio</param>
        /// <returns>The modified color with alpha component multiplied</returns>
        public static Color MulA(this Color color, float ratio)
        {
            color.a *= ratio;
            return color;
        }

        /// <summary>
        /// Multiplies the RGB components (red, green, blue) by the specified ratio and returns the modified color for fluent chaining.
        /// The alpha component remains unchanged.
        /// </summary>
        /// <param name="color">The source color</param>
        /// <param name="ratio">The multiplication ratio for RGB components</param>
        /// <returns>The modified color with RGB components multiplied</returns>
        public static Color MulRGB(this Color color, float ratio)
        {
            color.r *= ratio;
            color.g *= ratio;
            color.b *= ratio;
            return color;
        }
    }
}
