using SharpGDX.Shims;
using SharpGDX.Graphics;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Utils;
using SharpGDX.Graphics.G2D;
using SharpGDX.Files;
using SharpGDX.Utils.Reflect;

namespace SharpGDX.Scenes.Scene2D.UI;

/** A skin stores resources for UI widgets to use (texture regions, ninepatches, fonts, colors, etc). Resources are named and can
 * be looked up by name and type. Resources can be described in JSON. Skin provides useful conversions, such as allowing access to
 * regions in the atlas as ninepatches, sprites, drawables, etc. The get* methods return an instance of the object in the skin.
 * The new* methods return a copy of an instance in the skin.
 * <p>
 * See the <a href="https://libgdx.com/wiki/graphics/2d/scene2d/skin">documentation</a> for more.
 * @author Nathan Sweet */
public class Skin : IDisposable
{
    ObjectMap<Type, ObjectMap<String, Object>> resources = new();
    TextureAtlas atlas;
    float _scale = 1;

    private readonly ObjectMap<String, Type> jsonClassTags = new(defaultTagClasses.Length);


    /** Creates an empty skin. */
    public Skin()
    {
        foreach (Type c in defaultTagClasses)
            jsonClassTags.put(c.Name, c);
    }

    /** Creates a skin containing the resources in the specified skin JSON file. If a file in the same directory with a ".atlas"
     * extension exists, it is loaded as a {@link TextureAtlas} and the texture regions added to the skin. The atlas is
     * automatically disposed when the skin is disposed. */
    public Skin(FileHandle skinFile) : this()
    {
        FileHandle atlasFile = skinFile.sibling(skinFile.nameWithoutExtension() + ".atlas");
        if (atlasFile.exists())
        {
            atlas = new TextureAtlas(atlasFile);
            addRegions(atlas);
        }

        load(skinFile);
    }

    /** Creates a skin containing the resources in the specified skin JSON file and the texture regions from the specified atlas.
     * The atlas is automatically disposed when the skin is disposed. */
    public Skin(FileHandle skinFile, TextureAtlas atlas)
    {
        this.atlas = atlas;
        addRegions(atlas);
        load(skinFile);
    }

    /** Creates a skin containing the texture regions from the specified atlas. The atlas is automatically disposed when the skin
     * is disposed. */
    public Skin(TextureAtlas atlas)
    {
        this.atlas = atlas;
        addRegions(atlas);
    }

    /** Adds all resources in the specified skin JSON file. */
    public void load(FileHandle skinFile)
    {
        try
        {
            getJsonLoader(skinFile).fromJson(typeof(Skin), skinFile);
        }
        catch (SerializationException ex)
        {
            throw new SerializationException("Error reading file: " + skinFile, ex);
        }
    }

    /** Adds all named texture regions from the atlas. The atlas will not be automatically disposed when the skin is disposed. */
    public void addRegions(TextureAtlas atlas)
    {
        Array<TextureAtlas.AtlasRegion> regions = atlas.getRegions();
        for (int i = 0, n = regions.size; i < n; i++)
        {
            TextureAtlas.AtlasRegion region = regions.Get(i);
            String name = region.name;
            if (region.index != -1)
            {
                name += "_" + region.index;
            }

            add(name, region, typeof(TextureRegion));
        }
    }

    public void add(String name, Object resource)
    {
        add(name, resource, resource.GetType());
    }

    public void add(String name, Object resource, Type type)
    {
        if (name == null) throw new IllegalArgumentException("name cannot be null.");
        if (resource == null) throw new IllegalArgumentException("resource cannot be null.");
        ObjectMap<String, Object> typeResources = resources.get(type);
        if (typeResources == null)
        {
            typeResources = new(type == typeof(TextureRegion) || type == typeof(IDrawable) || type == typeof(Sprite)
                ? 256
                : 64);
            resources.put(type, typeResources);
        }

        typeResources.put(name, resource);
    }

    public void remove(String name, Type type)
    {
        if (name == null) throw new IllegalArgumentException("name cannot be null.");
        ObjectMap<String, Object> typeResources = resources.get(type);
        typeResources.remove(name);
    }

    // TODO: Really implement this for generics:
    public T get<T>(Type type)
    {
        return (T)get(type);
    }

    // TODO: Really implement this for generics:
    public T get<T>(string name, Type type)
    {
        return (T)get(name, type);
    }

    /** Returns a resource named "default" for the specified type.
         * @throws GdxRuntimeException if the resource was not found. */
    public object get(Type type)
    {
        return get("default", type);
    }

    /** Returns a named resource of the specified type.
     * @throws GdxRuntimeException if the resource was not found. */
    public object get(String name, Type type)
    {
        if (name == null) throw new IllegalArgumentException("name cannot be null.");
        if (type == null) throw new IllegalArgumentException("type cannot be null.");

        // TODO: These originally cast to T, should we verify type and throw exception? -RP
        if (type == typeof(IDrawable)) return getDrawable(name);
        if (type == typeof(TextureRegion)) return getRegion(name);
        if (type == typeof(NinePatch)) return getPatch(name);
        if (type == typeof(Sprite)) return getSprite(name);

        ObjectMap<String, Object> typeResources = resources.get(type);
        if (typeResources == null)
        {
            throw new GdxRuntimeException("No " + type.Name + " registered with name: " + name);
        }

        Object resource = typeResources.get(name);
        if (resource == null) throw new GdxRuntimeException("No " + type.Name + " registered with name: " + name);

// TODO: This originally cast to T, should we verify type and throw exception? -RP
        return resource;
    }

    /** Returns a named resource of the specified type.
     * @return null if not found. */
    public object? optional(String name, Type type)
    {
        if (name == null) throw new IllegalArgumentException("name cannot be null.");
        if (type == null) throw new IllegalArgumentException("type cannot be null.");
        ObjectMap<String, Object> typeResources = resources.get(type);
        if (typeResources == null) return null;
        // TODO: This originally cast to T, should we verify type and throw exception? -RP
        return typeResources.get(name);
    }

    public bool has(String name, Type type)
    {
        ObjectMap<String, Object> typeResources = resources.get(type);
        if (typeResources == null) return false;
        return typeResources.containsKey(name);
    }

    /** Returns the name to resource mapping for the specified type, or null if no resources of that type exist. */
    public ObjectMap<string, object>? getAll(Type type)
    {
        // TODO: This was originally typed. -RP
        return (ObjectMap<string, object>)resources.get(type);
    }

    public Color getColor(String name)
    {
        return (Color)get(name, typeof(Color));
    }

    public BitmapFont getFont(String name)
    {
        return (BitmapFont)get(name, typeof(BitmapFont));
    }

    /** Returns a registered texture region. If no region is found but a texture exists with the name, a region is created from the
     * texture and stored in the skin. */
    public TextureRegion getRegion(String name)
    {
        TextureRegion region = (TextureRegion?)optional(name, typeof(TextureRegion));
        if (region != null) return region;

        Texture texture = (Texture?)optional(name, typeof(Texture));
        if (texture == null) throw new GdxRuntimeException("No TextureRegion or Texture registered with name: " + name);
        region = new TextureRegion(texture);
        add(name, region, typeof(TextureRegion));
        return region;
    }

    /** @return an array with the {@link TextureRegion} that have an index != -1, or null if none are found. */
    public Array<TextureRegion>? getRegions(String regionName)
    {
        Array<TextureRegion> regions = null;
        int i = 0;
        TextureRegion region = (TextureRegion?)optional(regionName + "_" + (i++), typeof(TextureRegion));
        if (region != null)
        {
            regions = new Array<TextureRegion>();
            while (region != null)
            {
                regions.Add(region);
                region = (TextureRegion?)optional(regionName + "_" + (i++), typeof(TextureRegion));
            }
        }

        return regions;
    }

    /** Returns a registered tiled drawable. If no tiled drawable is found but a region exists with the name, a tiled drawable is
     * created from the region and stored in the skin. */
    public TiledDrawable getTiledDrawable(String name)
    {
        TiledDrawable tiled = (TiledDrawable?)optional(name, typeof(TiledDrawable));
        if (tiled != null) return tiled;

        tiled = new TiledDrawable(getRegion(name));
        tiled.setName(name);
        if (_scale != 1)
        {
            scale(tiled);
            tiled.setScale(_scale);
        }

        add(name, tiled, typeof(TiledDrawable));
        return tiled;
    }

    /** Returns a registered ninepatch. If no ninepatch is found but a region exists with the name, a ninepatch is created from the
     * region and stored in the skin. If the region is an {@link AtlasRegion} then its split {@link AtlasRegion#values} are used,
     * otherwise the ninepatch will have the region as the center patch. */
    public NinePatch getPatch(String name)
    {
        NinePatch patch = (NinePatch?)optional(name, typeof(NinePatch));
        if (patch != null) return patch;

        try
        {
            TextureRegion region = getRegion(name);
            if (region is TextureAtlas.AtlasRegion)
            {
                int[] splits = ((TextureAtlas.AtlasRegion)region).findValue("split");
                if (splits != null)
                {
                    patch = new NinePatch(region, splits[0], splits[1], splits[2], splits[3]);
                    int[] pads = ((TextureAtlas.AtlasRegion)region).findValue("pad");
                    if (pads != null) patch.setPadding(pads[0], pads[1], pads[2], pads[3]);
                }
            }

            if (patch == null) patch = new NinePatch(region);
            if (_scale != 1) patch.scale(_scale, _scale);
            add(name, patch, typeof(NinePatch));
            return patch;
        }
        catch (GdxRuntimeException ex)
        {
            throw new GdxRuntimeException("No NinePatch, TextureRegion, or Texture registered with name: " + name);
        }
    }

    /** Returns a registered sprite. If no sprite is found but a region exists with the name, a sprite is created from the region
     * and stored in the skin. If the region is an {@link AtlasRegion} then an {@link AtlasSprite} is used if the region has been
     * whitespace stripped or packed rotated 90 degrees. */
    public Sprite getSprite(String name)
    {
        Sprite sprite = (Sprite?)optional(name, typeof(Sprite));
        if (sprite != null) return sprite;

        try
        {
            TextureRegion textureRegion = getRegion(name);
            if (textureRegion is TextureAtlas.AtlasRegion)
            {
                TextureAtlas.AtlasRegion region = (TextureAtlas.AtlasRegion)textureRegion;
                if (region.rotate || region.packedWidth != region.originalWidth ||
                    region.packedHeight != region.originalHeight)
                    sprite = new TextureAtlas.AtlasSprite(region);
            }

            if (sprite == null) sprite = new Sprite(textureRegion);
            if (_scale != 1) sprite.SetSize(sprite.GetWidth() * _scale, sprite.GetHeight() * _scale);
            add(name, sprite, typeof(Sprite));
            return sprite;
        }
        catch (GdxRuntimeException ex)
        {
            throw new GdxRuntimeException("No NinePatch, TextureRegion, or Texture registered with name: " + name);
        }
    }

    /** Returns a registered drawable. If no drawable is found but a region, ninepatch, or sprite exists with the name, then the
     * appropriate drawable is created and stored in the skin. */
    public IDrawable getDrawable(String name)
    {
        IDrawable drawable = (IDrawable?)optional(name, typeof(IDrawable));
        if (drawable != null) return drawable;

        // Use texture or texture region. If it has splits, use ninepatch. If it has rotation or whitespace stripping, use sprite.
        try
        {
            TextureRegion textureRegion = getRegion(name);
            if (textureRegion is TextureAtlas.AtlasRegion)
            {
                TextureAtlas.AtlasRegion region = (TextureAtlas.AtlasRegion)textureRegion;
                if (region.findValue("split") != null)
                    drawable = new NinePatchDrawable(getPatch(name));
                else if (region.rotate || region.packedWidth != region.originalWidth ||
                         region.packedHeight != region.originalHeight)
                    drawable = new SpriteDrawable(getSprite(name));
            }

            if (drawable == null)
            {
                drawable = new TextureRegionDrawable(textureRegion);
                if (_scale != 1) scale(drawable);
            }
        }
        catch (GdxRuntimeException ignored)
        {
        }

        // Check for explicit registration of ninepatch, sprite, or tiled drawable.
        if (drawable == null)
        {
            NinePatch patch = (NinePatch?)optional(name, typeof(NinePatch));
            if (patch != null)
                drawable = new NinePatchDrawable(patch);
            else
            {
                Sprite sprite = (Sprite?)optional(name, typeof(Sprite));
                if (sprite != null)
                    drawable = new SpriteDrawable(sprite);
                else
                    throw new GdxRuntimeException(
                        "No Drawable, NinePatch, TextureRegion, Texture, or Sprite registered with name: " + name);
            }
        }

        if (drawable is BaseDrawable) ((BaseDrawable)drawable).setName(name);

        add(name, drawable, typeof(IDrawable));
        return drawable;
    }

    /** Returns the name of the specified style object, or null if it is not in the skin. This compares potentially every style
     * object in the skin of the same type as the specified style, which may be a somewhat expensive operation. */
    public String? find(Object resource)
    {
        if (resource == null) throw new IllegalArgumentException("style cannot be null.");
        ObjectMap<String, Object> typeResources = resources.get(resource.GetType());
        if (typeResources == null) return null;
        return typeResources.findKey(resource, true);
    }

    /** Returns a copy of a drawable found in the skin via {@link #getDrawable(String)}. */
    public IDrawable newDrawable(String name)
    {
        return newDrawable(getDrawable(name));
    }

    /** Returns a tinted copy of a drawable found in the skin via {@link #getDrawable(String)}. */
    public IDrawable newDrawable(String name, float r, float g, float b, float a)
    {
        return newDrawable(getDrawable(name), new Color(r, g, b, a));
    }

    /** Returns a tinted copy of a drawable found in the skin via {@link #getDrawable(String)}. */
    public IDrawable newDrawable(String name, Color tint)
    {
        return newDrawable(getDrawable(name), tint);
    }

    /** Returns a copy of the specified drawable. */
    public IDrawable newDrawable(IDrawable drawable)
    {
        if (drawable is TiledDrawable) return new TiledDrawable((TiledDrawable)drawable);
        if (drawable is TextureRegionDrawable) return new TextureRegionDrawable((TextureRegionDrawable)drawable);
        if (drawable is NinePatchDrawable) return new NinePatchDrawable((NinePatchDrawable)drawable);
        if (drawable is SpriteDrawable) return new SpriteDrawable((SpriteDrawable)drawable);
        throw new GdxRuntimeException("Unable to copy, unknown drawable type: " + drawable.GetType());
    }

    /** Returns a tinted copy of a drawable found in the skin via {@link #getDrawable(String)}. */
    public IDrawable newDrawable(IDrawable drawable, float r, float g, float b, float a)
    {
        return newDrawable(drawable, new Color(r, g, b, a));
    }

    /** Returns a tinted copy of a drawable found in the skin via {@link #getDrawable(String)}. */
    public IDrawable newDrawable(IDrawable drawable, Color tint)
    {
        IDrawable newDrawable;
        if (drawable is TextureRegionDrawable)
            newDrawable = ((TextureRegionDrawable)drawable).tint(tint);
        else if (drawable is NinePatchDrawable)
            newDrawable = ((NinePatchDrawable)drawable).tint(tint);
        else if (drawable is SpriteDrawable)
            newDrawable = ((SpriteDrawable)drawable).tint(tint);
        else
            throw new GdxRuntimeException("Unable to copy, unknown drawable type: " + drawable.GetType());

        if (newDrawable is BaseDrawable)
        {
            BaseDrawable named = (BaseDrawable)newDrawable;
            if (drawable is BaseDrawable)
                named.setName(((BaseDrawable)drawable).getName() + " (" + tint + ")");
            else
                named.setName(" (" + tint + ")");
        }

        return newDrawable;
    }

    /** Scales the drawable's {@link Drawable#getLeftWidth()}, {@link Drawable#getRightWidth()},
     * {@link Drawable#getBottomHeight()}, {@link Drawable#getTopHeight()}, {@link Drawable#getMinWidth()}, and
     * {@link Drawable#getMinHeight()}. */
    public void scale(IDrawable drawble)
    {
        drawble.setLeftWidth(drawble.getLeftWidth() * _scale);
        drawble.setRightWidth(drawble.getRightWidth() * _scale);
        drawble.setBottomHeight(drawble.getBottomHeight() * _scale);
        drawble.setTopHeight(drawble.getTopHeight() * _scale);
        drawble.setMinWidth(drawble.getMinWidth() * _scale);
        drawble.setMinHeight(drawble.getMinHeight() * _scale);
    }

    /** The scale used to size drawables created by this skin.
     * <p>
     * This can be useful when scaling an entire UI (eg with a stage's viewport) then using an atlas with images whose resolution
     * matches the UI scale. The skin can then be scaled the opposite amount so that the larger or smaller images are drawn at the
     * original size. For example, if the UI is scaled 2x, the atlas would have images that are twice the size, then the skin's
     * scale would be set to 0.5. */
    public void setScale(float scale)
    {
        this._scale = scale;
    }

    /** Sets the style on the actor to disabled or enabled. This is done by appending "-disabled" to the style name when enabled is
     * false, and removing "-disabled" from the style name when enabled is true. A method named "getStyle" is called the actor via
     * reflection and the name of that style is found in the skin. If the actor doesn't have a "getStyle" method or the style was
     * not found in the skin, no exception is thrown and the actor is left unchanged. */
    public void setEnabled(Actor actor, bool enabled)
    {
        // Get current style.
        Method method = findMethod(actor.GetType(), "getStyle");
        if (method == null) return;
        Object style;
        try
        {
            style = method.invoke(actor);
        }
        catch (Exception ignored)
        {
            return;
        }

        // Determine new style.
        String name = find(style);
        if (name == null) return;
        name = name.Replace("-disabled", "") + (enabled ? "" : "-disabled");
        style = get(name, style.GetType());
        // Set new style.
        method = findMethod(actor.GetType(), "setStyle");
        if (method == null) return;
        try
        {
            method.invoke(actor, style);
        }
        catch (Exception ignored)
        {
        }
    }

    /** Returns the {@link TextureAtlas} passed to this skin constructor, or null. */
    public TextureAtlas? getAtlas()
    {
        return atlas;
    }

    /** Disposes the {@link TextureAtlas} and all {@link Disposable} resources in the skin. */
    public void Dispose()
    {
        if (atlas != null) atlas.Dispose();
        foreach (ObjectMap<String, Object> entry in resources.values())
        {
            foreach (Object resource in entry.values())
                if (resource is IDisposable)
                    ((IDisposable)resource).Dispose();
        }
    }

    private class SkinJson : Json
    {
        private readonly Skin _skin;
        static private readonly String parentFieldName = "parent";

        public SkinJson(Skin skin)
        {
            _skin = skin;
        }

        public override object readValue(Type type, Type elementType, JsonValue jsonData)
        {
            // If the JSON is a string but the type is not, look up the actual value by name.
            // TODO: if (jsonData != null && jsonData.isString() && !ClassReflection.isAssignableFrom(CharSequence.class, type))
            if (jsonData != null && jsonData.isString() && !ClassReflection.isAssignableFrom(typeof(string), type))
                return _skin.get(jsonData.asString(), type);
            return base.readValue(type, elementType, jsonData);
        }

        protected override bool ignoreUnknownField(Type type, String fieldName)
        {
            return fieldName.Equals(parentFieldName);
        }

        public override void readFields(Object obj, JsonValue jsonMap)
        {
            if (jsonMap.has(parentFieldName))
            {
                string parentName = (string)readValue(parentFieldName, typeof(string), jsonMap);
                Type parentType = obj.GetType();
                while (true)
                {
                    try
                    {
                        copyFields(_skin.get(parentName, parentType), obj);
                        break;
                    }
                    catch (GdxRuntimeException ex)
                    {
                        // Parent resource doesn't exist.
                        parentType = parentType.BaseType; // Try resource for super class.
                        if (parentType == typeof(object))
                        {
                            SerializationException se = new SerializationException(
                                "Unable to find parent resource with name: " + parentName);
                            se.addTrace(jsonMap._child.trace());
                            throw se;
                        }
                    }
                }
            }

            base.readFields(obj, jsonMap);
        }
    }

    private class ReadOnlySkinSerializer : Json.ReadOnlySerializer
    {
        private readonly Skin _skin;

        public ReadOnlySkinSerializer(Skin skin)
        {
            _skin = skin;
        }

        public override Skin read(Json json, JsonValue typeToValueMap, Type ignored)
        {
            for (JsonValue valueMap = typeToValueMap._child; valueMap != null; valueMap = valueMap._next)
            {
                try
                {
                    Type type = json.getClass(valueMap.name());
                    if (type == null) type = ClassReflection.forName(valueMap.name());
                    readNamedObjects(json, type, valueMap);
                }
                catch (ReflectionException ex)
                {
                    throw new SerializationException(ex);
                }
            }

            return _skin;
        }

        private void readNamedObjects(Json json, Type type, JsonValue valueMap)
        {
            Type addType = type == typeof(TintedDrawable) ? typeof(IDrawable) : type;
            for (JsonValue valueEntry = valueMap._child; valueEntry != null; valueEntry = valueEntry._next)
            {
                Object obj = json.readValue(type, valueEntry);
                if (obj == null) continue;
                try
                {
                    _skin.add(valueEntry._name, obj, addType);
                    if (addType != typeof(IDrawable) && ClassReflection.isAssignableFrom(typeof(IDrawable), addType))
                        _skin.add(valueEntry._name, obj, typeof(IDrawable));
                }
                catch (Exception ex)
                {
                    throw new SerializationException(
                        "Error reading " + ClassReflection.getSimpleName(type) + ": " + valueEntry.name, ex);
                }
            }
        }
    }

    private class ReadOnlyBitmapFontSerializer : Json.ReadOnlySerializer
    {
        private readonly Skin _skin;
        private readonly FileHandle _skinFile;

        public ReadOnlyBitmapFontSerializer(Skin skin, FileHandle skinFile)
        {
            _skin = skin;
            _skinFile = skinFile;
        }

        public override BitmapFont read(Json json, JsonValue jsonData, Type type)
        {
            String path = (string)json.readValue("file", typeof(string), jsonData);
            float scaledSize = (float)json.readValue("scaledSize", typeof(float), -1f, jsonData);
            Boolean flip = (bool)json.readValue("flip", typeof(bool), false, jsonData);
            Boolean markupEnabled = (bool)json.readValue("markupEnabled", typeof(bool), false, jsonData);
            Boolean useIntegerPositions = (bool)json.readValue("useIntegerPositions", typeof(bool), true, jsonData);

            FileHandle fontFile = _skinFile.parent().child(path);
            if (!fontFile.exists()) fontFile = GDX.Files.Internal(path);
            if (!fontFile.exists()) throw new SerializationException("Font file not found: " + fontFile);

            // Use a region with the same name as the font, else use a PNG file in the same directory as the FNT file.
            String regionName = fontFile.nameWithoutExtension();
            try
            {
                BitmapFont font;
                Array<TextureRegion> regions = _skin.getRegions(regionName);
                if (regions != null)
                    font = new BitmapFont(new BitmapFont.BitmapFontData(fontFile, flip), regions, true);
                else
                {
                    TextureRegion region = (TextureRegion?)_skin.optional(regionName, typeof(TextureRegion));
                    if (region != null)
                        font = new BitmapFont(fontFile, region, flip);
                    else
                    {
                        FileHandle imageFile = fontFile.parent().child(regionName + ".png");
                        if (imageFile.exists())
                            font = new BitmapFont(fontFile, imageFile, flip);
                        else
                            font = new BitmapFont(fontFile, flip);
                    }
                }

                font.getData().markupEnabled = markupEnabled;
                font.setUseIntegerPositions(useIntegerPositions);
                // Scaled size is the desired cap height to scale the font to.
                if (scaledSize != -1) font.getData().setScale(scaledSize / font.getCapHeight());
                return font;
            }
            catch (RuntimeException ex)
            {
                throw new SerializationException("Error loading bitmap font: " + fontFile, ex);
            }
        }
    }

    private class ReadOnlyColorSerializer : Json.ReadOnlySerializer
    {
        private readonly Skin _skin;

        public ReadOnlyColorSerializer(Skin skin)
        {
            _skin = skin;
        }

        public override Color read(Json json, JsonValue jsonData, Type type)
        {
            if (jsonData.isString()) return (Color)_skin.get(jsonData.asString(), typeof(Color));
            String hex = (string)json.readValue("hex", typeof(string), (String)null, jsonData);
            if (hex != null) return Color.ValueOf(hex);
            float r = (float)json.readValue("r", typeof(float), 0f, jsonData);
            float g = (float)json.readValue("g", typeof(float), 0f, jsonData);
            float b = (float)json.readValue("b", typeof(float), 0f, jsonData);
            float a = (float)json.readValue("a", typeof(float), 1f, jsonData);
            return new Color(r, g, b, a);
        }
    }

    private class ReadOnlyTintedDrawableSerializer : Json.ReadOnlySerializer
    {
        private readonly Skin _skin;

        public ReadOnlyTintedDrawableSerializer(Skin skin)
        {
            _skin = skin;
        }

        public override object read(Json json, JsonValue jsonData, Type type)
        {
            String name = (string)json.readValue("name", typeof(string), jsonData);
            Color color = (Color)json.readValue("color", typeof(Color), jsonData);
            if (color == null) throw new SerializationException("TintedDrawable missing color: " + jsonData);
            IDrawable drawable = _skin.newDrawable(name, color);
            if (drawable is BaseDrawable)
            {

                BaseDrawable named = (BaseDrawable)drawable;
                named.setName(jsonData.name + " (" + name + ", " + color + ")");
            }

            return drawable;
        }
    }

    protected Json getJsonLoader(FileHandle skinFile)
    {
        Skin skin = this;

        Json json = new SkinJson(this);
        json.setTypeName(null);
        json.setUsePrototypes(false);

        json.setSerializer(typeof(Skin), new ReadOnlySkinSerializer(this));

        json.setSerializer(typeof(BitmapFont), new ReadOnlyBitmapFontSerializer(this, skinFile));

        json.setSerializer(typeof(Color), new ReadOnlyColorSerializer(this));

        json.setSerializer(typeof(TintedDrawable), new ReadOnlyTintedDrawableSerializer(this));

        foreach (var entry in jsonClassTags)
            json.addClassTag(entry.key, entry.value);

        return json;
    }

    /** Returns a map of {@link Json#addClassTag(String, Class) class tags} that will be used when loading skin JSON. The map can
     * be modified before calling {@link #load(FileHandle)}. By default the map is populated with the simple class names of libGDX
     * classes commonly used in skins. */
    public ObjectMap<String, Type> getJsonClassTags()
    {
        return jsonClassTags;
    }

    static private readonly Type[] defaultTagClasses =
    {
        typeof(BitmapFont), typeof(Color), typeof(TintedDrawable), typeof(NinePatchDrawable),
        typeof(SpriteDrawable), typeof(TextureRegionDrawable), typeof(TiledDrawable), typeof(Button.ButtonStyle),
        typeof(CheckBox.CheckBoxStyle), typeof(ImageButton.ImageButtonStyle),
        typeof(ImageTextButton.ImageTextButtonStyle),
        typeof(Label.LabelStyle),
        // TODO: typeof(List.ListStyle), 
        typeof(ProgressBar.ProgressBarStyle), typeof(ScrollPane.ScrollPaneStyle),
        // TODO: typeof(SelectBox.SelectBoxStyle), 
        typeof(Slider.SliderStyle), typeof(SplitPane.SplitPaneStyle), typeof(TextButton.TextButtonStyle),
        typeof(TextField.TextFieldStyle), typeof(TextTooltip.TextTooltipStyle),
        typeof(Touchpad.TouchpadStyle), // TODO: Tree.TreeStyle.class,
        typeof(Window.WindowStyle)
    };

    static private Method? findMethod(Type type, String name)
    {
        Method[] methods = ClassReflection.getMethods(type);
        for (int i = 0, n = methods.Length; i < n; i++)
        {
            Method method = methods[i];
            if (method.getName().Equals(name)) return method;
        }

        return null;
    }

    /** @author Nathan Sweet */
    public class TintedDrawable
    {
        public String name;
        public Color color;
    }
}