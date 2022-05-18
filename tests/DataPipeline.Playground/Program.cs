// See https://aka.ms/new-console-template for more information
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


//バイパスになる．キャッシュはバイパスが定数関数であるパターン．バイパスと考えるとその後続の結果を知らないといけない．
class CacheNode<TIn, TOut> : IPipelineInput<TIn>, IPipelineOutput<TOut>
{
    public CacheNode(Pipeline<TIn> following, IEqualityComparer<TIn> comparer)
    {
        this.following = following; 
        this.dictionary = new Dictionary<TIn, TOut>(comparer);
    }
    readonly Pipeline<TIn> following;
    readonly IDictionary<TIn, TOut> dictionary;
    public void Receive(TIn input)
    {

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