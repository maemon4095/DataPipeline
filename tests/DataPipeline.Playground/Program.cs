using System.Collections.Immutable;

Console.WriteLine("Hello, World!");


struct Pipeline<TIn>
{
    IPipelineInput<TIn> Input;
    ImmutableArray<object> Output;
}

interface IPipelineInput<TIn>
{
    public void Receive(TIn input);
}

interface IPipelineOutput<TOut>
{
    public IPipelineOutput<TOut> Connected(IPipelineInput<TOut> following);
}
interface IPipelineJoint<TIn, TOut> : IPipelineInput<TIn>, IPipelineOutput<TOut> {
    IPipelineOutput<TOut>.Connected(IPipelineInput<TIn> following) => this.Connected(following);
    new public IPipelineJoint<TIn, TOut> Connected(IPipelineInput<TOut> following);
}

//キャッシュノードは結果を受け取る必要がある。Receiveに副作用を持たせる必要がある。

class MergeNode<TOut> : IPipelineOutput<TOut>
{
    public MergeNode(ImmutableArray<IPipelineOutput<TOut>> outputs)
    {
        this.outputs = outputs;
    }

    ImmutableArray<IPipelineOutput<TOut>> outputs;

    public IPipelineOutput<TOut> Connected(IPipelineInput<TOut> following)
    {
        var builder = ImmutableArray.CreateBuilder(this.outputs.Length);
        foreach(var output in this.outputs)
        {
            builder.Add(output.Connected(following));
        }

        return new MergeNode(
            builder.ToImmutable()
        );
    }
} 

class TransformNode<TIn, TOut> : PipelineJoint<TIn, TOut>
{
    public TransformNode(Func<TIn, TOut> transformer)
    {
        this.transformer = transformer;
    }

    readonly Func<TIn, TOut> transformer;

    public override TOut Process(TIn input)
    {
        return this.transformer(input);
    }
}

class ValveNode<TIn, TOut>
{

}