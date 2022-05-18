// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;

Console.WriteLine("Hello, World!");


struct Pipeline<TIn, TOut>
{

}

interface IPipelineInput<TIn>
{
    public void Receive(TIn input);
}

interface IPipelineOutput<TOut>
{
    public IPipelineOutput<TOut> Connected(IPipelineInput<TOut> following);
}


//バイパスになる．キャッシュはバイパスが定数関数であるパターン．バイパスと考えるとその後続の結果を知らないといけない．
class CacheNode<TIn, TOut> : IPipelineInput<TIn>, IPipelineOutput<TOut>
{
    public CacheNode(IEnumerable<(IPipelineInput<TOut>, IImmutableDictionary<TIn, TOut>)> followings)
    {
        this.followings = followings.ToImmutableArray();
    }
    readonly ImmutableArray<(IPipelineInput<TOut>, IImmutableDictionary<TIn, TOut>)> followings;
    public void Receive(TIn input)
    {
        foreach (var (following, dictionary) in this.followings.AsSpan())
        {
            
        }
    }
    public IPipelineOutput<TOut> Connected(IPipelineInput<TOut> following) => throw new NotImplementedException();
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