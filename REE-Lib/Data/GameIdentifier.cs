using ReeLib.Common;

namespace ReeLib;

public readonly struct GameIdentifier
{
    public readonly int hash;
    public readonly string name;

    public GameIdentifier(int hash)
    {
        this.hash = hash;
        name = string.Empty;
    }

    public GameIdentifier(string name)
    {
        this.name = name;
        hash = (int)MurMur3HashUtils.GetHash(name);
    }

    public GameIdentifier(string name, int hash)
    {
        this.name = name;
        this.hash = hash;
    }

    public static implicit operator int(GameIdentifier id) => id.hash;

    public override int GetHashCode() => hash;
    public override bool Equals(object? obj) => obj is GameIdentifier id && id.hash == hash;
    public static bool operator ==(GameIdentifier left, GameIdentifier right) => left.hash == right.hash;
    public static bool operator !=(GameIdentifier left, GameIdentifier right) => left.hash != right.hash;
}
