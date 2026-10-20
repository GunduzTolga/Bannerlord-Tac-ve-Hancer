# DLX Hair Layers - API for other mods (1.1+)

Static class `HairAddons.Api` in `HairAddons.dll`. Call it through reflection, so your mod does not need a hard dependency.

```csharp
var api = Type.GetType("HairAddons.Api, HairAddons");   // null if DLX Hair Layers is not loaded
if (api != null)
{
    int version = (int)api.GetField("Version").GetValue(null);                                  // 1
    string layers = (string)api.GetMethod("GetLayers").Invoke(null, new object[] { character });
    api.GetMethod("SetLayers").Invoke(null, new object[] { hero, layers });
}
```

| Member | Description |
|---|---|
| `int Version` | API version (static readonly field). |
| `string GetLayers(object character)` | Layers as a string token. If the face editor is currently open for this character, returns the editor's live state. `null` = no record, `""` = no layers. |
| `bool SetLayers(object character, string layers)` | Stores layers. `null` or `""` removes them. |
| `bool Transfer(object from, object to)` | Copies layers from one character to another and removes them from the first. |
| `bool RemoveLayers(object character)` | Removes the record (e.g. before you unregister a temporary character). |

`character` can be a `Hero` or a `CharacterObject`.
- A hero's own `CharacterObject` (or the `Hero`) -> the hero's layers.
- Any other `CharacterObject` (temporary editor characters, troop types) -> layers of that character, keyed by its `StringId`.
- A copy made with `CharacterObject.CreateFrom(hero.CharacterObject)` is NOT treated as the hero.

The layers string is opaque - store it as is (it only contains letters, digits and `_ : ; | ~ -`, safe for XML attributes and for `BodyProperties.FromString`, which ignores unknown attributes).

## Layers inside a BodyProperties string

In the face editor, Ctrl+C adds `hair_layers="<layers>"` to the copied `<BodyProperties ... />` string, and Ctrl+V applies it. If you store appearance strings, you can keep the layers there too: `BodyProperties.FromString` ignores unknown attributes. The value is the same token `GetLayers` returns.

## Example: custom editor with a temporary character

1. Open the vanilla `BodyGeneratorView` with your temporary `CharacterObject`. The Hair Layers panel appears on the Hair tab.
2. When the player presses Done, DLX Hair Layers saves the layers for that character (before your confirm callback runs).
3. Before you unregister the temporary character: `layers = GetLayers(tempCharacter)`, store `layers` with your data, then `RemoveLayers(tempCharacter)`.
4. After you create the real hero: `SetLayers(hero, layers)`.

Records of characters that no longer exist are removed automatically when the game is saved.
