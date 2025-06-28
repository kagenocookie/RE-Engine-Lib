using ReeLib.Common;

namespace ReeLib;

public readonly struct GameIdentifier
{
    public readonly GameNameHash hash;
    public readonly string name;

    public GameName GameEnum => Enum.TryParse<GameName>(name, out var value) ? value : GameName.unknown;

    public GameIdentifier(GameNameHash hash)
    {
        this.hash = hash;
        name = string.Empty;
    }

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

    public static implicit operator GameIdentifier(string name) => new GameIdentifier(name);
    public static implicit operator GameNameHash(GameIdentifier id) => id.hash;
    public static implicit operator GameName(GameIdentifier id) => id.GameEnum;
    public static implicit operator int(GameIdentifier id) => (int)id.hash;

    public override int GetHashCode() => (int)hash;
    public override bool Equals(object? obj) => obj is GameIdentifier id && id.hash == hash;
    public static bool operator ==(GameIdentifier left, GameIdentifier right) => left.hash == right.hash;
    public static bool operator !=(GameIdentifier left, GameIdentifier right) => left.hash != right.hash;
}
