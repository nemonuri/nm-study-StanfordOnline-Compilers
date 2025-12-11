using System.Diagnostics;

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
    private readonly int _index;

    private const int IndexOK = 1;
    private readonly TagOk<TOk> _tagOk;

    private const int IndexError = 2;
    private readonly TagError<TError> _tagError;

    private Result(int index, TagOk<TOk> tagOk, TagError<TError> tagError)
    {
        _index = index;
        _tagOk = tagOk; _tagError = tagError;
    }

    public bool IsInvalid => !((IndexOK <= _index) && (_index <= IndexError));
    public bool IsOk => _index == IndexOK;
    public bool IsError => _index == IndexError;

    [return: NotNull]
    public TOk GetOk()
    {
        Guard.IsTrue(IsOk);
        Debug.Assert(_tagOk.Value is not null);
        return _tagOk.Value!;
    }

    public TError GetError()
    {
        Guard.IsTrue(IsError);
        return _tagError.Value;
    }


    public static implicit operator Result<TOk, TError>(TagOk<TOk> tagOk) => new(IndexOK, tagOk, default);
    public static implicit operator Result<TOk, TError>(TagError<TError> tagError) => new(IndexError, default, tagError);

    public TOut Match<TOut>(Func<TOk, TOut> caseOfOk, Func<TError, TOut> caseOfError)
    {
        Guard.IsNotNull(caseOfOk);
        Guard.IsNotNull(caseOfError);

        if (_index == IndexOK) { return caseOfOk(_tagOk.Value); }
        if (_index == IndexError) { return caseOfError(_tagError.Value); }

        return ThrowHelper.ThrowInvalidDataException<TOut>($"{nameof(Match)} failed: This {nameof(Result<,>)} instance is invalid.");
    }
}

