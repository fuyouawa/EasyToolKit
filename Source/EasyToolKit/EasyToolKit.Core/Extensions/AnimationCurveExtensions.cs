using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides extension methods for Unity's AnimationCurve class
    /// to simplify working with arbitrary time and value ranges.
    /// </summary>
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Evaluates the animation curve by remapping the input time from [0, maxTime] to [0, 1] range.
        /// This allows using the curve with arbitrary time ranges while maintaining the curve's original behavior.
        /// </summary>
        /// <param name="curve">The animation curve to evaluate</param>
        /// <param name="time">The current time value in the [0, maxTime] range</param>
        /// <param name="maxTime">The maximum time value that defines the input range</param>
        /// <returns>The evaluated curve value at the remapped time position</returns>
        public static float EvaluateWithRemap(this AnimationCurve curve, float time, float maxTime)
        {
            return curve.Evaluate(MathUtility.Remap(time, 0f, maxTime, 0f, 1f));
        }
        
        /// <summary>
        /// Evaluates the animation curve with double remapping: first remaps input time from [0, maxTime] to [0, 1],
        /// then remaps the curve output from [0, 1] to [minValue, maxValue].
        /// This allows using the curve with arbitrary time and value ranges.
        /// </summary>
        /// <param name="curve">The animation curve to evaluate</param>
        /// <param name="time">The current time value in the [0, maxTime] range</param>
        /// <param name="maxTime">The maximum time value that defines the input range</param>
        /// <param name="minValue">The minimum output value for the remapped result</param>
        /// <param name="maxValue">The maximum output value for the remapped result</param>
        /// <returns>The evaluated curve value remapped to the [minValue, maxValue] range</returns>
        /// <remarks>
        /// Note: The AnimationCurve.Evaluate result must be in the [0, 1] range for proper remapping.
        /// </remarks>
        public static float EvaluateWithRemap(this AnimationCurve curve, float time, float maxTime, float minValue, float maxValue)
        {
            return MathUtility.Remap(curve.EvaluateWithRemap(time, maxTime), 0f, 1f, minValue, maxValue);
        }
    }
}
