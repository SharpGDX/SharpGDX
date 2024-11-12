using SharpGDX.Shims;

namespace SharpGDX;

/// <summary>
///     A Preference instance is a hash map holding different values.
/// </summary>
/// <remarks>
///     <para>
///         It is stored alongside your application (SharedPreferences on Android, LocalStorage on GWT, on the desktop a
///         Java Preferences file in a ".prefs" directory will be created, and on iOS an NSMutableDictionary will be
///         written to the given file).
///     </para>
///     <para>
///         <b>CAUTION:</b> On the desktop platform, all SharpGDX applications share the same ".prefs"
///         directory. To avoid collisions use specific names like "com.myname.game1.settings" instead of "settings".
///     </para>
///     <para>
///         To persist changes made to a preferences instance <see cref="Flush()" /> has to be invoked. Except for Android,
///         changes are cached in memory prior to flushing. On iOS changes are not synchronized between different
///         preferences instances.
///     </para>
///     <para>
///         Use <see cref="IApplication.GetPreferences" /> to look up a specific preferences instance. Note that on
///         several backends the preferences name will be used as the filename, so make sure the name is valid for a
///         filename.
///     </para>
/// </remarks>
public interface IPreferences
{
    public void Clear();

    public bool Contains(string key);

    /// <summary>
    ///     Makes sure the preferences are persisted.
    /// </summary>
    public void Flush();

    /// <summary>
    ///     Returns a read only <see cref="Map{string, object}" /> with all the key, objects of the preferences.
    /// </summary>
    /// <returns></returns>
    public Map<string, object> Get();

    public bool GetBoolean(string key);

    public bool GetBoolean(string key, bool defValue);

    public float GetFloat(string key);

    public float GetFloat(string key, float defValue);

    public int GetInteger(string key);

    public int GetInteger(string key, int defValue);

    public long GetLong(string key);

    public long GetLong(string key, long defValue);

    public string GetString(string key);

    public string GetString(string key, string defValue);

    public IPreferences Put(Map<string, object> vals);

    public IPreferences PutBoolean(string key, bool val);

    public IPreferences PutFloat(string key, float val);

    public IPreferences PutInteger(string key, int val);

    public IPreferences PutLong(string key, long val);

    public IPreferences PutString(string key, string val);

    public void Remove(string key);
}