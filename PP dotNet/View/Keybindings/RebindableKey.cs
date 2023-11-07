﻿namespace PP_dotNet.View.Keybindings;

/// <summary>
/// Represents a <see cref="ConsoleKey"/> that can later be mapped to an <see cref="Action"/>.
/// </summary>
public class RebindableKey : UnboundKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RebindableKey"/> class.
    /// </summary>
    /// <param name="key"> The console key that can later be bound to an <see cref="Action"/>. </param>
    /// <param name="displayText"> A text representation of this <see cref="RebindableKey"/> to display on the user interface. </param>
    public RebindableKey(ConsoleKey key, string displayText) : base(key, displayText) { }

    /// <summary>
    /// Binds this key to an <see cref="Action"/>.
    /// </summary>
    /// <param name="action"> The action to bind this key to. </param>
    /// <returns> A new instance of the <see cref="Keybinding"/> class that has the same display text as this key. </returns>
    public Keybinding Bind(Action action) => new(action, Key, ToString());
}