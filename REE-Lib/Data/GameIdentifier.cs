using ReeLib.Common;

namespace ReeLib;

public readonly partial struct GameIdentifier
{
    public readonly GameNameHash hash;
    public readonly string name;

    public GameName GameEnum => Enum.TryParse<GameName>(name, out var value) ? value : GameName.unknown;

    public GameIdentifier(string name)
    {
        this.name = name;
        hash = (GameNameHash)MurMur3HashUtils.GetHash(name);
    }

    public GameIdentifier(string name, GameNameHash hash)
    {
        this.name = name;
        this.hash = hash;
    }

    public static readonly GameIdentifier Unknown = new GameIdentifier(string.Empty, 0);

    public static implicit operator GameIdentifier(string name) => new GameIdentifier(name);
    public static implicit operator GameNameHash(GameIdentifier id) => id.hash;
    public static implicit operator GameName(GameIdentifier id) => id.GameEnum;
    public static implicit operator int(GameIdentifier id) => (int)id.hash;

    public override int GetHashCode() => (int)hash;
    public override bool Equals(object? obj) => obj is GameIdentifier id && id.hash == hash;
    public static bool operator ==(GameIdentifier left, GameIdentifier right) => left.hash == right.hash;
    public static bool operator !=(GameIdentifier left, GameIdentifier right) => left.hash != right.hash;
    public override string ToString() => !string.IsNullOrEmpty(name) ? name : hash.ToString();
}
