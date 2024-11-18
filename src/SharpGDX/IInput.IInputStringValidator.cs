namespace SharpGDX;

public partial interface IInput
{
    interface IInputStringValidator
    {
        /// <summary>
        ///     Validates if the string is acceptable.
        /// </summary>
        /// <param name="toCheck">The string that should be validated.</param>
        /// <returns>true, if the string is acceptable, false if not.</returns>
        public bool Validate(string toCheck);
    }
}