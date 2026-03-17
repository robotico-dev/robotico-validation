using BenchmarkDotNet.Attributes;
using Robotico.Validation;

namespace Robotico.Validation.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class ValidationBenchmarks
{
    private static readonly IRule<int> RulePositive = Rule.When<int>(x => x > 0, "Value", "Must be positive");
    private static readonly IRule<int> RuleLessThan100 = Rule.When<int>(x => x < 100, "Value", "Must be less than 100");
    private static readonly IRule<int>[] Rules = [RulePositive, RuleLessThan100];

    [Benchmark(Baseline = true)]
    public Robotico.Result.Result ValidateAll_Success()
    {
        return Rule.ValidateAll(42, Rules);
    }

    [Benchmark]
    public Robotico.Result.Result ValidateAll_FailFirst()
    {
        return Rule.ValidateAll(-1, Rules);
    }

    [Benchmark]
    public Robotico.Result.Result SingleRule_Pass()
    {
        return RulePositive.Validate(10);
    }

    [Benchmark]
    public Robotico.Result.Result SingleRule_Fail()
    {
        return RulePositive.Validate(-10);
    }
}
