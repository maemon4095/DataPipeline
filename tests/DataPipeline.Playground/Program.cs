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

//複数経路の分岐と合併で表現できる。バイパスはメインになる経路とサブ経路を与える
class BypassNode<TIn, TOut> : IPipelineInput<TIn>, IPipelineOutput<TOut>
{
    private BypassNode(IEqualityComparer<TIn> comparer, IPipelineInput<TIn> input, ImmutableArray<IPipelineInput<TOut>> followings)
    {
        this.input = input;
        this.followings = followings;
        this.cache = new Dictionary<TIn, TOut>(comparer);
    }

    readonly IPipelineInput<TIn> input;
    readonly ImmutableArray<IPipelineInput<TOut>> followings;
    readonly IDictionary<TIn, TOut> cache;

    public void Receive(TIn input)
    {
        if(this.cache.TryGetValue(input, out var result))
        {
            foreach(var following in this.followings)
            {
                following.Receive(result);
            }
        }
        else
        {
            this.input.Receive(input);
        }
    }

    public IPipelineOutput<TOut> Connected(IPipelineInput<TOut> following)
    {
        return new BypassNode<TIn, TOut>(
            this.cache.Comparer,
            this.input.Connected(following),
            this.followings.Add(following)
        );
    }
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