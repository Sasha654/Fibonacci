using JetBrains.Annotations;

namespace PT.Fibonacci.Contract;

[PublicAPI]
public record FibNumRequest(int CalculationId, long Value);
