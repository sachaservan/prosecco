public enum IncrementalAlgorithmType
{   
    Merge, // calculates the FI on each batch and merges results (fast, less accurate)
    Accumulate, // continuously updates support with each new batch (slow, more accurate)
    Stream, // builds a single FP-tree and mines it at given intervals

    None // default value, prints error message
};
