using SharpGDX.Files;
using SharpGDX.Utils;
using SharpGDX.Shims;

namespace SharpGDX.Graphics.G2D
{
	/** Loads images from texture atlases created by TexturePacker.<br>
 * <br>
 * A TextureAtlas must be disposed to free up the resources consumed by the backing textures.
 * @author Nathan Sweet */
public class TextureAtlas : Disposable {
	private readonly ObjectSet<Texture> textures = new (4);
	private readonly Array<AtlasRegion> regions = new ();

	/** Creates an empty atlas to which regions can be added. */
	public TextureAtlas () {
	}

	/** Loads the specified pack file using {@link FileType#Internal}, using the parent directory of the pack file to find the page
	 * images. */
	public TextureAtlas (String internalPackFile) 
	: this(Gdx.files.@internal(internalPackFile))
	{
		
	}

	/** Loads the specified pack file, using the parent directory of the pack file to find the page images. */
	public TextureAtlas (FileHandle packFile) 
	: this(packFile, packFile.parent())
	{
		
	}

	/** @param flip If true, all regions loaded will be flipped for use with a perspective where 0,0 is the upper left corner.
	 * @see #TextureAtlas(FileHandle) */
	public TextureAtlas (FileHandle packFile, bool flip) 
	: this(packFile, packFile.parent(), flip)
	{
		
	}

	public TextureAtlas (FileHandle packFile, FileHandle imagesDir) 
	: this(packFile, imagesDir, false)
	{
		
	}

	/** @param flip If true, all regions loaded will be flipped for use with a perspective where 0,0 is the upper left corner. */
	public TextureAtlas (FileHandle packFile, FileHandle imagesDir, bool flip) 
	: this(new TextureAtlasData(packFile, imagesDir, flip))
	{
		
	}

	public TextureAtlas (TextureAtlasData data) {
		load(data);
	}

	/** Adds the textures and regions from the specified texture atlas data. */
	public void load (TextureAtlasData data) {
		textures.ensureCapacity(data.pages.size);
		foreach (TextureAtlasData.Page page in data.pages) {
			if (page.texture == null) page.texture = new Texture(page.textureFile, page.format, page.useMipMaps);
			page.texture.setFilter(page.minFilter, page.magFilter);
			page.texture.setWrap(page.uWrap, page.vWrap);
			textures.add(page.texture);
		}

		regions.ensureCapacity(data.regions.size);
		foreach (TextureAtlasData.Region region in data.regions) {
			AtlasRegion atlasRegion = new AtlasRegion(region.page.texture, region.left, region.top, //
				region.rotate ? region.height : region.width, //
				region.rotate ? region.width : region.height);
			atlasRegion.index = region.index;
			atlasRegion.name = region.name;
			atlasRegion.offsetX = region.offsetX;
			atlasRegion.offsetY = region.offsetY;
			atlasRegion.originalHeight = region.originalHeight;
			atlasRegion.originalWidth = region.originalWidth;
			atlasRegion.rotate = region.rotate;
			atlasRegion.degrees = region.degrees;
			atlasRegion.names = region.names;
			atlasRegion.values = region.values;
			if (region.flip) atlasRegion.flip(false, true);
			regions.add(atlasRegion);
		}
	}

	/** Adds a region to the atlas. The specified texture will be disposed when the atlas is disposed. */
	public AtlasRegion addRegion (String name, Texture texture, int x, int y, int width, int height) {
		textures.add(texture);
		AtlasRegion region = new AtlasRegion(texture, x, y, width, height);
		region.name = name;
		regions.add(region);
		return region;
	}

	/** Adds a region to the atlas. The texture for the specified region will be disposed when the atlas is disposed. */
	public AtlasRegion addRegion (String name, TextureRegion textureRegion) {
		textures.add(textureRegion.texture);
		AtlasRegion region = new AtlasRegion(textureRegion);
		region.name = name;
		regions.add(region);
		return region;
	}

	/** Returns all regions in the atlas. */
	public Array<AtlasRegion> getRegions () {
		return regions;
	}

	/** Returns the first region found with the specified name. This method uses string comparison to find the region, so the
	 * result should be cached rather than calling this method multiple times. */
	public AtlasRegion? findRegion (String name) {
		for (int i = 0, n = regions.size; i < n; i++)
			if (regions.get(i).name.Equals(name)) return regions.get(i);
		return null;
	}

	/** Returns the first region found with the specified name and index. This method uses string comparison to find the region, so
	 * the result should be cached rather than calling this method multiple times. */
	public AtlasRegion? findRegion (String name, int index) {
		for (int i = 0, n = regions.size; i < n; i++) {
			AtlasRegion region = regions.get(i);
			if (!region.name.Equals(name)) continue;
			if (region.index != index) continue;
			return region;
		}
		return null;
	}

	/** Returns all regions with the specified name, ordered by smallest to largest {@link AtlasRegion#index index}. This method
	 * uses string comparison to find the regions, so the result should be cached rather than calling this method multiple
	 * times. */
	public Array<AtlasRegion> findRegions (String name) {
		Array<AtlasRegion> matched = new (typeof(AtlasRegion));
		for (int i = 0, n = regions.size; i < n; i++) {
			AtlasRegion region = regions.get(i);
			if (region.name.Equals(name)) matched.add(new AtlasRegion(region));
		}
		return matched;
	}

	/** Returns all regions in the atlas as sprites. This method creates a new sprite for each region, so the result should be
	 * stored rather than calling this method multiple times.
	 * @see #createSprite(String) */
	public Array<Sprite> createSprites () {
		var sprites = new Array<Sprite>(true, regions.size, typeof(Sprite));
		for (int i = 0, n = regions.size; i < n; i++)
			sprites.add(newSprite(regions.get(i)));
		return sprites;
	}

	/** Returns the first region found with the specified name as a sprite. If whitespace was stripped from the region when it was
	 * packed, the sprite is automatically positioned as if whitespace had not been stripped. This method uses string comparison to
	 * find the region and constructs a new sprite, so the result should be cached rather than calling this method multiple
	 * times. */
	public  Sprite? createSprite (String name) {
		for (int i = 0, n = regions.size; i < n; i++)
			if (regions.get(i).name.Equals(name)) return newSprite(regions.get(i));
		return null;
	}

	/** Returns the first region found with the specified name and index as a sprite. This method uses string comparison to find
	 * the region and constructs a new sprite, so the result should be cached rather than calling this method multiple times.
	 * @see #createSprite(String) */
	public Sprite? createSprite (String name, int index) {
		for (int i = 0, n = regions.size; i < n; i++) {
			AtlasRegion region = regions.get(i);
			if (region.index != index) continue;
			if (!region.name.Equals(name)) continue;
			return newSprite(regions.get(i));
		}
		return null;
	}

	/** Returns all regions with the specified name as sprites, ordered by smallest to largest {@link AtlasRegion#index index}.
	 * This method uses string comparison to find the regions and constructs new sprites, so the result should be cached rather
	 * than calling this method multiple times.
	 * @see #createSprite(String) */
	public Array<Sprite> createSprites (String name) {
		Array<Sprite> matched = new (typeof(Sprite));
		for (int i = 0, n = regions.size; i < n; i++) {
			AtlasRegion region = regions.get(i);
			if (region.name.Equals(name)) matched.add(newSprite(region));
		}
		return matched;
	}

	private Sprite newSprite (AtlasRegion region) {
		if (region.packedWidth == region.originalWidth && region.packedHeight == region.originalHeight) {
			if (region.rotate) {
				Sprite sprite = new Sprite(region);
				sprite.setBounds(0, 0, region.getRegionHeight(), region.getRegionWidth());
				sprite.rotate90(true);
				return sprite;
			}
			return new Sprite(region);
		}
		return new AtlasSprite(region);
	}

	/** Returns the first region found with the specified name as a {@link NinePatch}. The region must have been packed with
	 * ninepatch splits. This method uses string comparison to find the region and constructs a new ninepatch, so the result should
	 * be cached rather than calling this method multiple times. */
	public NinePatch? createPatch (String name) {
		for (int i = 0, n = regions.size; i < n; i++) {
			AtlasRegion region = regions.get(i);
			if (region.name.Equals(name)) {
				int[] splits = region.findValue("split");
				if (splits == null) throw new IllegalArgumentException("Region does not have ninepatch splits: " + name);
				NinePatch patch = new NinePatch(region, splits[0], splits[1], splits[2], splits[3]);
				int[] pads = region.findValue("pad");
				if (pads != null) patch.setPadding(pads[0], pads[1], pads[2], pads[3]);
				return patch;
			}
		}
		return null;
	}

	/** @return the textures of the pages, unordered */
	public ObjectSet<Texture> getTextures () {
		return textures;
	}

	/** Releases all resources associated with this TextureAtlas instance. This releases all the textures backing all
	 * TextureRegions and Sprites, which should no longer be used after calling dispose. */
	public void dispose () {
		foreach (Texture texture in textures)
			texture.dispose();
		textures.clear(0);
	}

	public class TextureAtlasData {
		internal readonly Array<Page> pages = new ();
		internal readonly Array<Region> regions = new ();

		public TextureAtlasData () {
		}

		public TextureAtlasData (FileHandle packFile, FileHandle imagesDir, bool flip) {
			load(packFile, imagesDir, flip);
		}

		public void load (FileHandle packFile, FileHandle imagesDir, bool flip) {
			String[] entry = new String[5];

			ObjectMap<String, Field<Page>> pageFields = new (15, 0.99f); // Size needed to avoid collisions.
			pageFields.put("size", new Field<Page>() {
				parse = (Page page) =>{
					page.width = int.Parse(entry[1]);
					page.height = int.Parse(entry[2]);
				}
			});
			pageFields.put("format", new Field<Page>() {
				 parse = (Page page) => {
					page.format = Enum.Parse<Pixmap.Format>(entry[1]);
				}
			});
			pageFields.put("filter", new Field<Page>() {
				 parse = (Page page) =>{
					page.minFilter = Enum.Parse < Texture.TextureFilter>(entry[1]);
					page.magFilter = Enum.Parse < Texture.TextureFilter>(entry[2]);
					page.useMipMaps = Texture.TextureFilterUtils.isMipMap(page.minFilter);
				}
			});
			pageFields.put("repeat", new Field<Page>() {
				parse =(Page page) => {
					if (entry[1].IndexOf('x') != -1) page.uWrap = Texture.TextureWrap.Repeat;
					if (entry[1].IndexOf('y') != -1) page.vWrap = Texture.TextureWrap.Repeat;
				}
			});
			pageFields.put("pma", new Field<Page>() {
				parse = (Page page) => {
					page.pma = entry[1].Equals("true");
				}
			});

			bool[] hasIndexes = new[] {false};
			ObjectMap<String, Field<Region>> regionFields = new (127, 0.99f); // Size needed to avoid collisions.
			regionFields.put("bounds", new Field<Region>() {
				parse =(Region region) => {
					region.left = int.Parse(entry[1]);
					region.top = int.Parse(entry[2]);
					region.width = int.Parse(entry[3]);
					region.height = int.Parse(entry[4]);
				}
			});
			regionFields.put("offsets", new Field<Region>() {
				 parse =(Region region) =>{
					region.offsetX = int.Parse(entry[1]);
					region.offsetY = int.Parse(entry[2]);
					region.originalWidth = int.Parse(entry[3]);
					region.originalHeight = int.Parse(entry[4]);
				}
			});
			regionFields.put("rotate", new Field<Region>() {
				parse = (Region region) =>{
					String value = entry[1];
					if (value.Equals("true"))
						region.degrees = 90;
					else if (!value.Equals("false")) //
						region.degrees = int.Parse(value);
					region.rotate = region.degrees == 90;
				}
			});
			regionFields.put("index", new Field<Region>() {
				 parse = (Region region) =>{
					region.index = int.Parse(entry[1]);
					if (region.index != -1) hasIndexes[0] = true;
				}
			});

			BufferedReader reader = packFile.reader(1024);
			try {
				String line = reader.readLine();
				// Ignore empty lines before first entry.
				while (line != null && line.Trim().Length == 0)
					line = reader.readLine();
				// Header entries.
				while (true) {
					if (line == null || line.Trim().Length == 0) break;
					if (readEntry(entry, line) == 0) break; // Silently ignore all header fields.
					line = reader.readLine();
				}
				// Page and region entries.
				Page page = null;
				Array<Object> names = null, values = null;
				while (true) {
					if (line == null) break;
					if (line.Trim().Length == 0) {
						page = null;
						line = reader.readLine();
					} else if (page == null) {
						page = new Page();
						page.textureFile = imagesDir.child(line);
						while (true) {
							if (readEntry(entry, line = reader.readLine()) == 0) break;
							var field = pageFields.get(entry[0]);
							if (field != null) field.parse(page); // Silently ignore unknown page fields.
						}
						pages.add(page);
					} else {
						Region region = new Region();
						region.page = page;
						region.name = line.Trim();
						if (flip) region.flip = true;
						while (true) {
							int count = readEntry(entry, line = reader.readLine());
							if (count == 0) break;
							Field<Region> field = regionFields.get<string>(entry[0]);
							if (field != null)
								field.parse(region);
							else {
								if (names == null) {
									names = new (8);
									values = new (8);
								}
								names.add(entry[0]);
								int[] entryValues = new int[count];
								for (int i = 0; i < count; i++) {
									try {
										entryValues[i] = int.Parse(entry[i + 1]);
									} catch (FormatException ignored) { // Silently ignore non-integer values.
									}
								}
								values.add(entryValues);
							}
						}
						if (region.originalWidth == 0 && region.originalHeight == 0) {
							region.originalWidth = region.width;
							region.originalHeight = region.height;
						}
						if (names != null && names.size > 0) {
							region.names = names.toArray<string>(typeof(String));
							region.values = values.toArray<int[]>(typeof(int[]));
							names.clear();
							values.clear();
						}
						regions.add(region);
					}
				}
			} catch (Exception ex) {
				throw new GdxRuntimeException("Error reading texture atlas file: " + packFile, ex);
			} finally {
				StreamUtils.closeQuietly(reader);
			}

			if (hasIndexes[0]) {
				regions.sort(new RegionComparer() );
			}
		}

		private class RegionComparer : IComparer<Region>
			{
					public int Compare(Region region1, Region region2)
					{
						int i1 = region1.index;
						if (i1 == -1) i1 = int.MaxValue;
						int i2 = region2.index;
						if (i2 == -1) i2 = int.MaxValue;
						return i1 - i2;
					}
		}

		public Array<Page> getPages () {
			return pages;
		}

		public Array<Region> getRegions () {
			return regions;
		}

		static private int readEntry (String[] entry,  String? line) // TODO: throws IOException
                                                               {
			if (line == null) return 0;
			line = line.Trim();
			if (line.Length == 0) return 0;
			int colon = line.IndexOf(':');
			if (colon == -1) return 0;
			entry[0] = line.Substring(0, colon).Trim();
			for (int i = 1, lastMatch = colon + 1;; i++) {
				int comma = line.IndexOf(',', lastMatch);
				if (comma == -1) {
					entry[i] = line.Substring(lastMatch).Trim();
					return i;
				}
				entry[i] = line.Substring(lastMatch, comma).Trim();
				lastMatch = comma + 1;
				if (i == 4) return 4;
			}
		}

		private class Field<T>
		{
			// TODO: Move to constructor to ensure set. -RP
			private Action<T> _parse;
			public Action<T> parse
			{
				get => _parse ?? throw new NotImplementedException("Parse was null");
				set => _parse = value;
			}
		}

		public class Page {
			/** May be null if this page isn't associated with a file. In that case, {@link #texture} must be set. */
			public  FileHandle? textureFile;
			/** May be null if the texture is not yet loaded. */
			public Texture? texture;
			public float width, height;
			public bool useMipMaps;
			public Pixmap.Format format = Pixmap.Format.RGBA8888;
			public Texture.TextureFilter minFilter = Texture.TextureFilter.Nearest, magFilter = Texture.TextureFilter.Nearest;
			public Texture.TextureWrap uWrap = Texture.TextureWrap.ClampToEdge, vWrap = Texture.TextureWrap.ClampToEdge;
			public bool pma;
		}

		public class Region {
			public Page page;
			public String name;
			public int left, top, width, height;
			public float offsetX, offsetY;
			public int originalWidth, originalHeight;
			public int degrees;
			public bool rotate;
			public int index = -1;
			public String[]? names;
			public int[][]? values;
			public bool flip;

			public int[]? findValue (String name) {
				if (names != null) {
					for (int i = 0, n = names.Length; i < n; i++)
						if (name.Equals(names[i])) return values[i];
				}
				return null;
			}
		}
	}

	/** Describes the region of a packed image and provides information about the original image before it was packed. */
	public class AtlasRegion : TextureRegion {
		/** The number at the end of the original image file name, or -1 if none.<br>
		 * <br>
		 * When sprites are packed, if the original file name ends with a number, it is stored as the index and is not considered as
		 * part of the sprite's name. This is useful for keeping animation frames in order.
		 * @see TextureAtlas#findRegions(String) */
		public int index = -1;

		/** The name of the original image file, without the file's extension.<br>
		 * If the name ends with an underscore followed by only numbers, that part is excluded: underscores denote special
		 * instructions to the texture packer. */
		public String name;

		/** The offset from the left of the original image to the left of the packed image, after whitespace was removed for
		 * packing. */
		public float offsetX;

		/** The offset from the bottom of the original image to the bottom of the packed image, after whitespace was removed for
		 * packing. */
		public float offsetY;

		/** The width of the image, after whitespace was removed for packing. */
		public int packedWidth;

		/** The height of the image, after whitespace was removed for packing. */
		public int packedHeight;

		/** The width of the image, before whitespace was removed and rotation was applied for packing. */
		public int originalWidth;

		/** The height of the image, before whitespace was removed for packing. */
		public int originalHeight;

		/** If true, the region has been rotated 90 degrees counter clockwise. */
		public bool rotate;

		/** The degrees the region has been rotated, counter clockwise between 0 and 359. Most atlas region handling deals only with
		 * 0 or 90 degree rotation (enough to handle rectangles). More advanced texture packing may support other rotations (eg, for
		 * tightly packing polygons). */
		public int degrees;

		/** Names for name/value pairs other than the fields provided on this class, each entry corresponding to {@link #values}. */
		public String[]? names;

		/** Values for name/value pairs other than the fields provided on this class, each entry corresponding to {@link #names}. */
		public int[][]? values;

		public AtlasRegion (Texture texture, int x, int y, int width, int height) 
		: base(texture, x, y, width, height)
		{
			
			originalWidth = width;
			originalHeight = height;
			packedWidth = width;
			packedHeight = height;
		}

		public AtlasRegion (AtlasRegion region) {
			setRegion(region);
			index = region.index;
			name = region.name;
			offsetX = region.offsetX;
			offsetY = region.offsetY;
			packedWidth = region.packedWidth;
			packedHeight = region.packedHeight;
			originalWidth = region.originalWidth;
			originalHeight = region.originalHeight;
			rotate = region.rotate;
			degrees = region.degrees;
			names = region.names;
			values = region.values;
		}

		public AtlasRegion (TextureRegion region) {
			setRegion(region);
			packedWidth = region.getRegionWidth();
			packedHeight = region.getRegionHeight();
			originalWidth = packedWidth;
			originalHeight = packedHeight;
		}

		/** Flips the region, adjusting the offset so the image appears to be flipped as if no whitespace has been removed for
		 * packing. */
		public override void flip (bool x, bool y) {
			base.flip(x, y);
			if (x) offsetX = originalWidth - offsetX - getRotatedPackedWidth();
			if (y) offsetY = originalHeight - offsetY - getRotatedPackedHeight();
		}

		/** Returns the packed width considering the {@link #rotate} value, if it is true then it returns the packedHeight,
		 * otherwise it returns the packedWidth. */
		public float getRotatedPackedWidth () {
			return rotate ? packedHeight : packedWidth;
		}

		/** Returns the packed height considering the {@link #rotate} value, if it is true then it returns the packedWidth,
		 * otherwise it returns the packedHeight. */
		public float getRotatedPackedHeight () {
			return rotate ? packedWidth : packedHeight;
		}

		public int[]? findValue (String name) {
			if (names != null) {
				for (int i = 0, n = names.Length; i < n; i++)
					if (name.Equals(names[i])) return values[i];
			}
			return null;
		}

		public override String ToString () {
			return name;
		}
	}

	/** A sprite that, if whitespace was stripped from the region when it was packed, is automatically positioned as if whitespace
	 * had not been stripped. */
	public class AtlasSprite : Sprite {
		readonly AtlasRegion region;
		float originalOffsetX, originalOffsetY;

		public AtlasSprite (AtlasRegion region) {
			this.region = new AtlasRegion(region);
			originalOffsetX = region.offsetX;
			originalOffsetY = region.offsetY;
			setRegion(region);
			setOrigin(region.originalWidth / 2f, region.originalHeight / 2f);
			int width = region.getRegionWidth();
			int height = region.getRegionHeight();
			if (region.rotate) {
				base.rotate90(true);
				base.setBounds(region.offsetX, region.offsetY, height, width);
			} else
				base.setBounds(region.offsetX, region.offsetY, width, height);
			setColor(1, 1, 1, 1);
		}

		public AtlasSprite (AtlasSprite sprite) {
			region = sprite.region;
			this.originalOffsetX = sprite.originalOffsetX;
			this.originalOffsetY = sprite.originalOffsetY;
			set(sprite);
		}

		public override void setPosition (float x, float y) {
			base.setPosition(x + region.offsetX, y + region.offsetY);
		}

		public override void setX (float x) {
			base.setX(x + region.offsetX);
		}

		public override void setY (float y) {
			base.setY(y + region.offsetY);
		}

		public override void setBounds (float x, float y, float width, float height) {
			float widthRatio = width / region.originalWidth;
			float heightRatio = height / region.originalHeight;
			region.offsetX = originalOffsetX * widthRatio;
			region.offsetY = originalOffsetY * heightRatio;
			int packedWidth = region.rotate ? region.packedHeight : region.packedWidth;
			int packedHeight = region.rotate ? region.packedWidth : region.packedHeight;
			base.setBounds(x + region.offsetX, y + region.offsetY, packedWidth * widthRatio, packedHeight * heightRatio);
		}

		public override void setSize (float width, float height) {
			setBounds(getX(), getY(), width, height);
		}

		public override void setOrigin (float originX, float originY) {
			base.setOrigin(originX - region.offsetX, originY - region.offsetY);
		}

		public override void setOriginCenter () {
			base.setOrigin(width / 2 - region.offsetX, height / 2 - region.offsetY);
		}

		public override void flip (bool x, bool y) {
			// Flip texture.
			if (region.rotate)
				base.flip(y, x);
			else
				base.flip(x, y);

			float oldOriginX = getOriginX();
			float oldOriginY = getOriginY();
			float oldOffsetX = region.offsetX;
			float oldOffsetY = region.offsetY;

			float widthRatio = getWidthRatio();
			float heightRatio = getHeightRatio();

			region.offsetX = originalOffsetX;
			region.offsetY = originalOffsetY;
			region.flip(x, y); // Updates x and y offsets.
			originalOffsetX = region.offsetX;
			originalOffsetY = region.offsetY;
			region.offsetX *= widthRatio;
			region.offsetY *= heightRatio;

			// Update position and origin with new offsets.
			translate(region.offsetX - oldOffsetX, region.offsetY - oldOffsetY);
			setOrigin(oldOriginX, oldOriginY);
		}
		
		public override void rotate90 (bool clockwise) {
			// Rotate texture.
			base.rotate90(clockwise);

			float oldOriginX = getOriginX();
			float oldOriginY = getOriginY();
			float oldOffsetX = region.offsetX;
			float oldOffsetY = region.offsetY;

			float widthRatio = getWidthRatio();
			float heightRatio = getHeightRatio();

			if (clockwise) {
				region.offsetX = oldOffsetY;
				region.offsetY = region.originalHeight * heightRatio - oldOffsetX - region.packedWidth * widthRatio;
			} else {
				region.offsetX = region.originalWidth * widthRatio - oldOffsetY - region.packedHeight * heightRatio;
				region.offsetY = oldOffsetX;
			}

			// Update position and origin with new offsets.
			translate(region.offsetX - oldOffsetX, region.offsetY - oldOffsetY);
			setOrigin(oldOriginX, oldOriginY);
		}

		public override float getX () {
			return base.getX() - region.offsetX;
		}

		public override float getY () {
			return base.getY() - region.offsetY;
		}

		public override float getOriginX () {
			return base.getOriginX() + region.offsetX;
		}

		public override float getOriginY () {
			return base.getOriginY() + region.offsetY;
		}

		public override float getWidth () {
			return base.getWidth() / region.getRotatedPackedWidth() * region.originalWidth;
		}

		public override float getHeight () {
			return base.getHeight() / region.getRotatedPackedHeight() * region.originalHeight;
		}

		public float getWidthRatio () {
			return base.getWidth() / region.getRotatedPackedWidth();
		}

		public float getHeightRatio () {
			return base.getHeight() / region.getRotatedPackedHeight();
		}

		public AtlasRegion getAtlasRegion () {
			return region;
		}

		public override String ToString() {
			return region.ToString();
		}
	}
}
}