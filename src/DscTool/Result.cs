namespace DscTool;

public readonly record struct TagOk<T>(T Value);

public readonly record struct TagError<T>(T Value);

public static class ResultTagger
{
    public static TagOk<T> Ok<T>(T value) => new(value);
    public static TagError<T> Error<T>(T value) => new(value);
}

public readonly record struct Result<TOk, TError>
{
    private readonly TagOk<TOk>? _tagOk;
    private readonly TagError<TError>? _tagError;

    private Result(TagOk<TOk>? tagOk, TagError<TError>? tagError)
    {
        _tagOk = tagOk; _tagError = tagError;
    }

    public bool IsEmpty => this == default;
    public static implicit operator Result<TOk, TError>(TagOk<TOk> tagOk) => new(tagOk, default);
    public static implicit operator Result<TOk, TError>(TagError<TError> tagError) => new(default, tagError);

    public TOut Match<TOut>(Func<TOk, TOut> caseOfOk, Func<TError, TOut> caseOfError)
    {
        Guard.IsNotNull(caseOfOk);
        Guard.IsNotNull(caseOfError);

        if (_tagOk.HasValue) { return caseOfOk(_tagOk.Value.Value); }
        if (_tagError.HasValue) { return caseOfError(_tagError.Value.Value); }

        return ThrowHelper.ThrowInvalidDataException<TOut>($"{nameof(Match)} failed: This {nameof(Result<,>)} instance is empty.");
    }
}

