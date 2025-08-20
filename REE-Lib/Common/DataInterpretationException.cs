namespace ReeLib
{
    /// <summary>
    /// Thrown when an expected assumption as to how the data for a file format works is found to be false.
    /// </summary>
    public class DataInterpretationException(string message = "Found unexpected data while reading file") : Exception(message)
    {
    }
}
